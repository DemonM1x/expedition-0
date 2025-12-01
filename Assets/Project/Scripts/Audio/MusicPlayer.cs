using UnityEngine;
using System.Collections;

namespace Expedition0.Audio
{
    [System.Serializable]
    public class MusicTrack
    {
        public AudioClip intro; // may be null
        public AudioClip loop; // must be loopable
        [Range(0f, 1f)] public float volume = 1f; // per-track target volume
    }

    public class MusicPlayer : MonoBehaviour
    {
        [Header("Audio Sources (assign in Inspector)")]
        public AudioSource mainSrc; // plays current loop (and later the new loop)
        public AudioSource auxSrc; // plays intro (or the next intro)

        [Header("Defaults")] public bool playOnAwake = true;
        // public bool persistAcrossScenes = true;
        public MusicTrack defaultTrack;

        [Tooltip("Scheduling lead time in seconds (DSP). 0.03–0.08 is typical.")]
        public double leadTime = 0.05;

        [Tooltip("Default cross-fade duration (seconds).")]
        public float defaultCrossFade = 1.0f;
        
        // Add near your other fields:
        private Coroutine _fadePauseCo, _fadeResumeCo;
        private float _savedMainVol = 1f, _savedAuxVol = 1f;

        // State
        public MusicTrack CurrentTrack { get; private set; }
        public MusicTrack NextTrack { get; private set; }
        private MusicTrackAsset _currentMusicTrackAsset;
        public MusicTrackAsset CurrentMusicTrackAsset => _currentMusicTrackAsset;
        
        public static MusicPlayer Instance;

        // Utility
        private static double ClipLen(AudioClip c)
        {
            return c == null ? 0.0 : (double)c.samples / c.frequency;
        }
        
        private bool IsAnyPlaying() => (mainSrc != null && mainSrc.isPlaying) || (auxSrc != null && auxSrc.isPlaying);


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Safe defaults
            mainSrc.playOnAwake = false;
            mainSrc.spatialBlend = 0f;
            mainSrc.loop = true;
            auxSrc.playOnAwake = false;
            auxSrc.spatialBlend = 0f;
            auxSrc.loop = false;
        }

        private void Start()
        {
            if (playOnAwake && defaultTrack != null)
                Play(defaultTrack);
        }

        // ---------------------------
        // Core public API
        // ---------------------------

        /// <summary>Starts playing a track from its intro (if any) into its loop.</summary>
        public void Play(MusicTrack track)
        {
            StopAllCoroutines();
            CurrentTrack = track;
            NextTrack = null;

            // reset volumes (no crossfade here)
            mainSrc.volume = 0f;
            auxSrc.volume = track != null ? track.volume : 1f;

            var start = AudioSettings.dspTime + leadTime;

            // 1) Schedule intro on aux (if any)
            var introLen = 0.0;
            if (track != null && track.intro != null)
            {
                auxSrc.clip = track.intro;
                auxSrc.loop = false;
                auxSrc.PlayScheduled(start);
                introLen = ClipLen(track.intro);
            }

            // 2) Schedule loop on main exactly after intro (or immediately if no intro)
            var loopStart = start + introLen;
            if (track != null && track.loop != null)
            {
                mainSrc.clip = track.loop;
                mainSrc.loop = true;
                mainSrc.PlayScheduled(loopStart);
            }

            // 3) Clean stop of intro at the join
            if (introLen > 0.0) auxSrc.SetScheduledEndTime(loopStart);

            // 4) At the loop start, set main volume to track target
            StartCoroutine(SetVolumeAtDspTime(mainSrc, track?.volume ?? 1f, loopStart));
        }

        /// <summary>Pause both sources (resume with Resume()).</summary>
        public void Pause()
        {
            mainSrc.Pause();
            auxSrc.Pause();
        }

        /// <summary>Resume after Pause().</summary>
        public void Resume()
        {
            mainSrc.UnPause();
            auxSrc.UnPause();
        }

        /// <summary>Stops with optional fade-out.</summary>
        public void Stop(float fadeOutSeconds = 0.5f)
        {
            StopAllCoroutines();
            if (fadeOutSeconds > 0f)
            {
                StartCoroutine(FadeAndStop(mainSrc, fadeOutSeconds));
                StartCoroutine(FadeAndStop(auxSrc, fadeOutSeconds));
            }
            else
            {
                mainSrc.Stop();
                auxSrc.Stop();
            }

            CurrentTrack = null;
            NextTrack = null;
        }

        /// <summary>Reassigns the default/next track without playing immediately.</summary>
        public void Reassign(MusicTrack track)
        {
            NextTrack = track;
        }

        // ---------------------------
        // Helpers (coroutines)
        // ---------------------------

        private IEnumerator Fade(AudioSource src, float from, float to, float dur)
        {
            if (dur <= 0f)
            {
                src.volume = to;
                yield break;
            }

            var t = 0f;
            src.volume = from;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                src.volume = Mathf.Lerp(from, to, t / dur);
                yield return null;
            }

            src.volume = to;
        }

        private IEnumerator FadeAndStop(AudioSource src, float dur)
        {
            var startVol = src.volume;
            yield return Fade(src, startVol, 0f, dur);
            src.Stop();
            src.volume = startVol; // restore logical volume for next start
        }
        
        // Fades out then pauses; does nothing if already paused
        public void FadeOutWithPause(float dur = 1f)
        {
            // If neither source is playing, we consider it already paused.
            if (!IsAnyPlaying())
                return;

            // Stop any running fade coroutines to avoid conflicts.
            if (_fadeResumeCo != null) { StopCoroutine(_fadeResumeCo); _fadeResumeCo = null; }
            if (_fadePauseCo  != null) { StopCoroutine(_fadePauseCo);  _fadePauseCo  = null; }

            // Remember current volumes to restore on fade-in.
            _savedMainVol = mainSrc != null ? mainSrc.volume : 1f;
            _savedAuxVol  = auxSrc  != null ? auxSrc.volume  : 1f;

            _fadePauseCo = StartCoroutine(FadeOutThenPauseCo(dur));
        }

        private IEnumerator FadeOutThenPauseCo(float dur)
        {
            float m0 = mainSrc != null ? mainSrc.volume : 0f;
            float a0 = auxSrc  != null ? auxSrc.volume  : 0f;

            if (dur <= 0f)
            {
                if (mainSrc) mainSrc.volume = 0f;
                if (auxSrc)  auxSrc.volume  = 0f;
                Pause();
                _fadePauseCo = null;
                yield break;
            }

            float t = 0f;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / dur);
                if (mainSrc) mainSrc.volume = Mathf.Lerp(m0, 0f, k);
                if (auxSrc)  auxSrc.volume  = Mathf.Lerp(a0, 0f, k);
                yield return null;
            }

            if (mainSrc) mainSrc.volume = 0f;
            if (auxSrc)  auxSrc.volume  = 0f;

            Pause();
            _fadePauseCo = null;
        }

        // Resumes then fades in; does nothing if already playing
        public void FadeInWithResume(float dur = 1f)
        {
            // If any source is already playing, do nothing.
            if (IsAnyPlaying())
                return;

            // Stop opposite fade if it’s still running.
            if (_fadePauseCo  != null) { StopCoroutine(_fadePauseCo);  _fadePauseCo  = null; }
            if (_fadeResumeCo != null) { StopCoroutine(_fadeResumeCo); _fadeResumeCo = null; }

            // Resume both sources (UnPause is idempotent even if they were stopped).
            Resume();

            // Choose targets: use saved volumes if available, fall back to track.volume.
            float target = CurrentTrack != null ? CurrentTrack.volume : 1f;
            float targetMain = (_savedMainVol > 0f) ? _savedMainVol : target;
            float targetAux  = (_savedAuxVol  > 0f) ? _savedAuxVol  : target;

            _fadeResumeCo = StartCoroutine(FadeInAfterResumeCo(dur, targetMain, targetAux));
        }

        private IEnumerator FadeInAfterResumeCo(float dur, float targetMain, float targetAux)
        {
            float m0 = mainSrc != null ? mainSrc.volume : 0f;
            float a0 = auxSrc  != null ? auxSrc.volume  : 0f;

            if (dur <= 0f)
            {
                if (mainSrc) mainSrc.volume = targetMain;
                if (auxSrc)  auxSrc.volume  = targetAux;
                _fadeResumeCo = null;
                yield break;
            }

            float t = 0f;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Clamp01(t / dur);
                if (mainSrc) mainSrc.volume = Mathf.Lerp(m0, targetMain, k);
                if (auxSrc)  auxSrc.volume  = Mathf.Lerp(a0, targetAux,  k);
                yield return null;
            }

            if (mainSrc) mainSrc.volume = targetMain;
            if (auxSrc)  auxSrc.volume  = targetAux;
            _fadeResumeCo = null;
        }

        private IEnumerator SetVolumeAtDspTime(AudioSource src, float target, double dspTime)
        {
            while (AudioSettings.dspTime < dspTime) yield return null;
            src.volume = target;
        }

        public void Play(MusicTrackAsset a)
        {
            _currentMusicTrackAsset = a;
            Play(a ? new MusicTrack { intro = a.intro, loop = a.loop, volume = a.volume } : null);
        }

        public void Reassign(MusicTrackAsset a)
        {
            _currentMusicTrackAsset = a;
            Reassign(a ? new MusicTrack { intro = a.intro, loop = a.loop, volume = a.volume } : null);
        }
    }
}
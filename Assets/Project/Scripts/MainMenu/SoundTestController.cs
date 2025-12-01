using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Expedition0.Save;
using Expedition0.Audio;
using TMPro;

namespace Expedition0.MainMenu
{
    public sealed class SoundTestController : MonoBehaviour
    {
        [Serializable]
        public struct Entry
        {
            public MusicTrackAsset track;
            public GameProgress required; // 0 = always available
        }

        [Header("Tracks")] [SerializeField] private List<Entry> tracks = new();

        [Header("UI")] [SerializeField] private TMP_Text titleLabel;

        [Header("Options")] [SerializeField] private bool filterByProgress = true;

        private readonly List<MusicTrackAsset> _pool = new();
        private int _index;
        private MusicTrackAsset _beforeTest;

        // Call this from the panel's OnEnable event, or a menu button "Open Sound Test"
        public void Open()
        {
            BuildPool();
            CacheCurrent();
            Show(_index);
        }

        // Call this from the panel's OnDisable event, or a "Back" button
        public void Close()
        {
            if (MusicPlayer.Instance && _beforeTest)
                MusicPlayer.Instance.Play(_beforeTest);
        }

        // — Buttons hook these directly —
        public void Prev()
        {
            if (_pool.Count == 0) return;
            _index = (_index - 1 + _pool.Count) % _pool.Count;
            Show(_index);
        }
        
        public void PrevAndPlay()
        {
            Prev();
            Debug.Log($"Sound Test: Switched to music track '{_pool[_index]}'");
            PlayCurrent();
        }
        
        public void Next()
        {
            if (_pool.Count == 0) return;
            _index = (_index + 1) % _pool.Count;
            Show(_index);
        }
        
        public void NextAndPlay()
        {
            Next();
            Debug.Log($"Sound Test: Switched to music track '{_pool[_index]}'");
            PlayCurrent();
        }

        public void RandomPick()
        {
            if (_pool.Count == 0) return;
            _index = UnityEngine.Random.Range(0, _pool.Count);
            Show(_index);
        }

        public void PlayCurrent()
        {
            if (_pool.Count == 0 || !MusicPlayer.Instance) return;
            MusicPlayer.Instance.Play(_pool[_index]); // hard switch, no crossfade
        }

        public void StopPlayback()
        {
            if (MusicPlayer.Instance) MusicPlayer.Instance.Stop();
        }

        // — Internals —
        private void BuildPool()
        {
            _pool.Clear();
            var p = SaveManager.LoadProgress();
            foreach (var e in tracks)
            {
                if (!e.track) continue;
                if (!filterByProgress || (p & e.required) == e.required)
                    _pool.Add(e.track);
            }

            if (_pool.Count == 0)
                foreach (var e in tracks)
                    if (e.track)
                    {
                        _pool.Add(e.track);
                        break;
                    }

            _index = Mathf.Clamp(_index, 0, Mathf.Max(0, _pool.Count - 1));
            
            Debug.Log($"Sound Test: Built pool of size {_pool.Count}");
        }

        private void CacheCurrent()
        {
            /* if you track current music elsewhere, set _beforeTest here */
        }

        private void Show(int i)
        {
            if (!titleLabel) return;
            titleLabel.text = _pool.Count > 0 ? _pool[i].EffectiveName : "(no tracks)";
        }
    }
}
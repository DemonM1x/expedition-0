using UnityEngine;

namespace Expedition0.Audio
{
    [CreateAssetMenu(fileName = "MusicTrack", menuName = "Audio/Music Track")]
    public class MusicTrackAsset : ScriptableObject
    {
        [Header("Playback Clips")]
        public AudioClip intro;                 // may be null
        public AudioClip loop;                  // must be loopable
        [Range(0f, 1f)] public float volume = 1f;

        [Header("Metadata")]
        [Tooltip("Human-friendly name. If empty, the asset's file name is used.")]
        public string displayName;

        public string EffectiveName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;

#if UNITY_EDITOR
        [Header("Developer Notes (Editor-only)")]
        [TextArea(2, 4)]
        [SerializeField] private string developerDescription; // editor-only
        public string DeveloperDescription => developerDescription;

        const int MaxDescLen = 128;

        // Runs whenever you edit the asset in the Inspector
        void OnValidate()
        {
            if (!string.IsNullOrEmpty(developerDescription) && developerDescription.Length > MaxDescLen)
                developerDescription = developerDescription.Substring(0, MaxDescLen);
        }
#endif
    }
}
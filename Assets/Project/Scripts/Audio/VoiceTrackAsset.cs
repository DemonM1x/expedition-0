using UnityEngine;
using UnityEngine.Audio;

namespace Expedition0.Audio
{
    [CreateAssetMenu(fileName = "VoiceTrack", menuName = "Audio/Voice Track Data")]
    public class VoiceTrackAsset: ScriptableObject
    {
        [Header("Voice Line Audio")]
        public AudioResource voiceAudio;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.5f, 1.5f)] public float pitch = 1f;
        public bool interruptible = true;
        public float preDelay = 0f;
        
        [Header("Voice Line Text")]
        public VoiceCharacter character;
        [TextArea]
        public string text;
    }
}
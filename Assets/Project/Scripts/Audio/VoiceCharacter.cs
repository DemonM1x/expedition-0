using UnityEngine;

namespace Expedition0.Audio
{
    [CreateAssetMenu(fileName = "VoiceCharacter", menuName = "Audio/Voice Character Data")]
    public class VoiceCharacter: ScriptableObject
    {
        public string characterName;
    }
}
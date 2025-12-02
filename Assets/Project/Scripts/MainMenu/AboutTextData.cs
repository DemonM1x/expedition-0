using UnityEngine;

namespace Expedition0.MainMenu
{
    [CreateAssetMenu(fileName = "AboutText", menuName = "Text/About Text Data")]
    public class AboutTextData: ScriptableObject
    {
        [Multiline(32)]
        public string aboutText;
    }
}
using UnityEngine;
using System.IO;
using TMPro;

namespace Expedition0.MainMenu
{
    public class InsertText : MonoBehaviour
    {
        [SerializeField] private TMP_Text targetText;
        [SerializeField] private AboutTextData aboutTextData;
        [TextArea] [SerializeField] private string defaultText;

        void Start()
        {
            string text = (aboutTextData != null) ? aboutTextData.aboutText : defaultText;
            targetText.text = text;
        }
    }
}

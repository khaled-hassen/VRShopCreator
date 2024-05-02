using System.IO;
using TMPro;
using UnityEngine;

namespace UI.AssetImporter
{
    public abstract class AssetImporter : UIScreen
    {
        public delegate void OnTabClickedCallback(string filePath);

        [SerializeField] private TMP_InputField inputField;

        protected string _filePath;
        public event OnTabClickedCallback OnTabClicked;

        public void OnTabClickedEvent() => OnTabClicked?.Invoke(_filePath);

        public void LoadUI(string filePath)
        {
            _filePath = filePath;
            inputField.text = Path.GetFileNameWithoutExtension(filePath);
        }
    }
}
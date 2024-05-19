using System;
using System.IO;
using JetBrains.Annotations;
using StoreAsset;
using TMPro;
using UnityEngine;

namespace UI.AssetImporter
{
    public abstract class AssetImporter : UIScreen
    {
        [SerializeField] protected TMP_InputField assetNameInput;

        private string _filePath;
        [CanBeNull] private StoreAssetData _oldAssetData;
        private SaveManager _saveManager;

        private void Awake()
        {
            _saveManager = FindObjectOfType<SaveManager>();
            if (_saveManager is null) throw new Exception("SaveManager not found in the scene! Application cannot continue.");
        }

        public event Action<string> OnTabClicked;
        public event Action OnAssetImported;

        public void OnTabClickedEvent() => OnTabClicked?.Invoke(_filePath);

        public void LoadUI(string filePath)
        {
            _filePath = filePath;
            assetNameInput.text = Path.GetFileNameWithoutExtension(filePath);
        }

        public virtual void LoadUI(StoreAssetData assetData)
        {
            assetNameInput.text = assetData.assetName;
            _oldAssetData = assetData;
        }

        public void ImportAsset()
        {
            var assetName = assetNameInput.text;
            var assetData = new StoreAssetData { assetName = assetName };
            AddDataToAsset(assetData);

            _saveManager.SaveAsset(UniqueId.GenerateUniqueId(), assetData, _oldAssetData is null ? _filePath : null);

            OnAssetImported?.Invoke();
            OnCloseWindowClick();
        }

        protected abstract void AddDataToAsset(StoreAssetData assetData);
    }
}
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

            var saveDirectory = Path.Combine(_saveManager.GetSaveDirectoryPath(), assetName);
            if (Directory.Exists(saveDirectory) && _oldAssetData is null)
            {
                OnCloseWindowClick();
                return;
            }

            if (_oldAssetData is not null && _oldAssetData.assetName != assetName)
                Directory.Move(Path.Combine(_saveManager.GetSaveDirectoryPath(), _oldAssetData.assetName), saveDirectory);

            if (_oldAssetData is null) Directory.CreateDirectory(saveDirectory);
            // save asset data}
            var formatter = new BinaryFormatter();
            var dataFilePath = Path.Combine(saveDirectory, "data.dat");

            if (_oldAssetData is not null) File.Delete(dataFilePath);
            var file = File.Create(dataFilePath);
            formatter.Serialize(file, assetData);
            file.Close();
            Debug.Log("Data file saved to: " + dataFilePath);

            if (_oldAssetData is not null && _filePath is not null)
            {
                // save asset 3d model
                var objSavePath = Path.Combine(saveDirectory, "mesh.obj");
                File.Copy(_filePath, objSavePath);
                Debug.Log("OBJ file saved to: " + objSavePath);
            }

            OnAssetImported?.Invoke();
            OnCloseWindowClick();
        }

        protected abstract void AddDataToAsset(StoreAssetData assetData);
    }
}
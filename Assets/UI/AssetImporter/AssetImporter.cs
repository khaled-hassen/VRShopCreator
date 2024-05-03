using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Dummiesman;
using StoreAsset;
using TMPro;
using UnityEngine;

namespace UI.AssetImporter
{
    public abstract class AssetImporter : UIScreen
    {
        public delegate void OnTabClickedCallback(string filePath);

        [SerializeField] protected TMP_InputField assetNameInput;

        private string _filePath;
        private SaveManager _saveManager;

        private void Awake()
        {
            _saveManager = FindObjectOfType<SaveManager>();
            if (_saveManager is null) throw new Exception("SaveManager not found in the scene! Application cannot continue.");
        }

        public event OnTabClickedCallback OnTabClicked;

        public void OnTabClickedEvent() => OnTabClicked?.Invoke(_filePath);

        public void LoadUI(string filePath)
        {
            _filePath = filePath;
            assetNameInput.text = Path.GetFileNameWithoutExtension(filePath);
        }

        protected void ImportAsset()
        {
            var assetName = assetNameInput.text;
            var assetData = new StoreAssetData { assetName = assetName };
            AddDataToAsset(assetData);

            // Create a save directory if it doesn't exist
            var saveDirectory = Path.Combine(_saveManager.GetSaveDirectoryPath(), assetName);
            if (!Directory.Exists(saveDirectory)) Directory.CreateDirectory(saveDirectory);

            var formatter = new BinaryFormatter();
            var dataFilePath = Path.Combine(saveDirectory, "data.dat");
            var file = File.Create(dataFilePath);
            formatter.Serialize(file, assetData);
            file.Close();
            Debug.Log("Data file saved to: " + dataFilePath);

            // copy the fbx from the original path to the save directory
            var objSavePath = Path.Combine(saveDirectory, "mesh.obj");
            File.Copy(_filePath, objSavePath);
            Debug.Log("OBJ file saved to: " + objSavePath);
        }

        protected abstract void AddDataToAsset(StoreAssetData assetData);

        // TODO: use this method to import the asset (in AssetExplorer window)
        private GameObject LoadAsset(string filePath)
        {
            var model = new OBJLoader().Load(filePath);
            if (model is null)
            {
                Debug.LogError("Failed to load Asset file at path: " + filePath);
                return null;
            }

            // fix scale
            model.transform.localScale = Vector3.one * 0.01f;

            // fix materials
            for (var i = 0; i < model.transform.childCount; i++)
            {
                var child = model.transform.GetChild(i);
                var childRenderer = child.GetComponent<Renderer>();
                if (childRenderer is null) continue;
                childRenderer.material.shader = Shader.Find("Universal Render Pipeline/Lit");
            }

            return model;
        }

        // TODO: use this method to import the asset data (in AssetExplorer window)
        // public void Load(string filename)
        // {
        //     if (File.Exists(filename))
        //     {
        //         BinaryFormatter formatter = new BinaryFormatter();
        //         FileStream file = File.Open(filename, FileMode.Open);
        //         StoreAssetData data = (StoreAssetData)formatter.Deserialize(file);
        //         file.Close();
        //
        //         assetName = data.assetName;
        //         assetSpecs.Clear();
        //         assetSpecs.AddRange(data.assetSpecs);
        //     }
        //     else
        //     {
        //         Debug.LogError("File not found: " + filename);
        //     }
        // }
    }
}
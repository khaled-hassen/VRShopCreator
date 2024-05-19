using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Dummiesman;
using JetBrains.Annotations;
using StoreAsset;
using UnityEngine;

namespace SaveManager
{
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] private string assetsSaveFileName = "assets.json";
        [SerializeField] private string storeStateSaveFileName = "store.json";

        private void Awake() => LoadStoreState();

        private string GetSaveDirectoryPath() => Application.persistentDataPath;

        private string GetAssetsSaveFilePath() => Path.Combine(GetSaveDirectoryPath(), assetsSaveFileName);

        private string GetStoreStateSaveFilePath() => Path.Combine(GetSaveDirectoryPath(), storeStateSaveFileName);

        public void SaveAsset(UniqueId id, StoreAssetData assetData, [CanBeNull] string modelPath)
        {
            var saveDirectory = GetSaveDirectoryPath();
            string objSavePath = null;
            if (modelPath is not null)
            {
                // save asset 3d model
                objSavePath = Path.Combine(saveDirectory, $"{id.uuid}.obj");
                File.Copy(modelPath, objSavePath);
                Debug.Log("OBJ file saved to: " + objSavePath);
            }

            var formatter = new BinaryFormatter();
            var dataFilePath = Path.Combine(saveDirectory, $"{id.uuid}.dat");

            var file = File.Create(dataFilePath);
            formatter.Serialize(file, assetData);
            file.Close();
            Debug.Log("Data file saved to: " + dataFilePath);

            var saveFile = GetAssetsSaveFilePath();
            var saveItems = File.Exists(saveFile)
                ? JsonUtility.FromJson<AssetsSaveFile>(File.ReadAllText(saveFile))
                : new AssetsSaveFile();

            var index = saveItems.items.FindIndex(item => item.id == id.uuid);
            if (index == -1) saveItems.items.Add(new AssetSaveFileItem(id.uuid, assetData.assetName, dataFilePath, objSavePath));
            else
                saveItems.items[index] = new AssetSaveFileItem(saveItems.items[index].id, assetData.assetName, saveItems.items[index].dataPath,
                    saveItems.items[index].modelPath ?? objSavePath);

            var json = JsonUtility.ToJson(saveItems);
            File.WriteAllText(saveFile, json);

            Debug.Log($"File saved at: {saveFile}");
        }

        public AssetsSaveFile LoadAssets()
        {
            var saveFile = GetAssetsSaveFilePath();
            return !File.Exists(saveFile) ? new AssetsSaveFile() : JsonUtility.FromJson<AssetsSaveFile>(File.ReadAllText(saveFile));
        }

        public void LoadAsset(string id, out StoreAssetData assetData)
        {
            var saveFile = GetAssetsSaveFilePath();
            if (!File.Exists(saveFile))
            {
                assetData = null;
                return;
            }

            var saveItems = JsonUtility.FromJson<AssetsSaveFile>(File.ReadAllText(saveFile));
            var item = saveItems.items.Find(i => i.id == id);
            if (item is null)
            {
                assetData = new StoreAssetData();
                return;
            }

            var formatter = new BinaryFormatter();
            var file = File.Open(item.dataPath, FileMode.Open);
            assetData = (StoreAssetData)formatter.Deserialize(file);
            assetData.modelPath = item.modelPath;
            file.Close();
        }

        public void DeleteAsset(string id)
        {
            var saveFile = GetAssetsSaveFilePath();
            if (!File.Exists(saveFile)) return;

            var saveItems = JsonUtility.FromJson<AssetsSaveFile>(File.ReadAllText(saveFile));
            var item = saveItems.items.Find(i => i.id == id);
            if (item is null) return;

            saveItems.items.Remove(item);
            File.WriteAllText(saveFile, JsonUtility.ToJson(saveItems));

            if (item.modelPath is not null) File.Delete(item.modelPath);
            File.Delete(item.dataPath);
        }

        public void SaveStoreState()
        {
            var assets = FindObjectsOfType<StoreAsset.StoreAsset>();
            if (assets.Length == 0) return;
            var savePath = GetStoreStateSaveFilePath();
            var saveFile = new StoreStateSaveFile();
            foreach (var asset in assets)
                saveFile.items.Add(new StoreStateSaveFileItem(asset.gameObject.GetComponent<UniqueId>().uuid, asset.transform.position,
                    asset.transform.rotation, asset.assetData.modelPath));

            File.WriteAllText(savePath, JsonUtility.ToJson(saveFile));
        }

        public void LoadStoreState()
        {
            var savePath = GetStoreStateSaveFilePath();
            if (!File.Exists(savePath)) return;
            var saveFile = JsonUtility.FromJson<StoreStateSaveFile>(File.ReadAllText(savePath));
            var assets = FindObjectsOfType<StoreAsset.StoreAsset>();
            foreach (var item in saveFile.items)
            {
                var asset = Array.Find(assets, a => a.gameObject.GetComponent<UniqueId>().uuid == item.id);
                if (asset is null)
                {
                    if (item.modelPath is null) continue;

                    var model = new OBJLoader().Load(item.modelPath);
                    if (model is null) continue;
                    var id = model.AddComponent<UniqueId>();
                    id.uuid = item.id;
                    asset = model.AddComponent<StoreAsset.StoreAsset>();
                }

                asset.transform.position = item.position;
                asset.transform.rotation = item.rotation;
            }
        }
    }
}
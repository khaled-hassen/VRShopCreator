using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using JetBrains.Annotations;
using StoreAsset;
using UnityEngine;

[Serializable]
public record SaveFileItem
{
    public string dataPath;
    [CanBeNull] public string modelPath;
    public string name;
    public string id;

    public SaveFileItem(string id, string name, string dataPath, [CanBeNull] string modelPath)
    {
        this.id = id;
        this.name = name;
        this.dataPath = dataPath;
        this.modelPath = modelPath;
    }
}

[Serializable]
public class SaveFile
{
    public List<SaveFileItem> items = new();
}

public class SaveManager : MonoBehaviour
{
    [SerializeField] private string saveDirectoryName = "SavedAssets";
    [SerializeField] private string saveFileName = "data.json";

    public string GetSaveDirectoryPath() => Path.Combine(Application.persistentDataPath, saveDirectoryName);

    public string GetSaveFilePath() => Path.Combine(GetSaveDirectoryPath(), saveFileName);

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

        var saveFile = GetSaveFilePath();
        var saveItems = File.Exists(saveFile)
            ? JsonUtility.FromJson<SaveFile>(File.ReadAllText(saveFile))
            : new SaveFile();

        var index = saveItems.items.FindIndex(item => item.id == id.uuid);
        if (index == -1) saveItems.items.Add(new SaveFileItem(id.uuid, assetData.assetName, dataFilePath, objSavePath));
        else
            saveItems.items[index] = new SaveFileItem(saveItems.items[index].id, assetData.assetName, saveItems.items[index].dataPath,
                saveItems.items[index].modelPath ?? objSavePath);

        var json = JsonUtility.ToJson(saveItems);
        File.WriteAllText(saveFile, json);

        Debug.Log($"File saved at: {saveFile}");
    }

    public SaveFile LoadAssets()
    {
        var saveFile = GetSaveFilePath();
        return !File.Exists(saveFile) ? new SaveFile() : JsonUtility.FromJson<SaveFile>(File.ReadAllText(saveFile));
    }

    public void LoadAsset(string id, out StoreAssetData assetData, out string modelPath)
    {
        var saveFile = GetSaveFilePath();
        if (!File.Exists(saveFile))
        {
            assetData = null;
            modelPath = null;
            return;
        }

        var saveItems = JsonUtility.FromJson<SaveFile>(File.ReadAllText(saveFile));
        var item = saveItems.items.Find(i => i.id == id);
        if (item is null)
        {
            assetData = new StoreAssetData();
            modelPath = null;
            return;
        }

        var formatter = new BinaryFormatter();
        var file = File.Open(item.dataPath, FileMode.Open);
        assetData = (StoreAssetData)formatter.Deserialize(file);
        file.Close();

        modelPath = item.modelPath;
    }

    public void DeleteAsset(string id)
    {
        var saveFile = GetSaveFilePath();
        if (!File.Exists(saveFile)) return;

        var saveItems = JsonUtility.FromJson<SaveFile>(File.ReadAllText(saveFile));
        var item = saveItems.items.Find(i => i.id == id);
        if (item is null) return;

        saveItems.items.Remove(item);
        File.WriteAllText(saveFile, JsonUtility.ToJson(saveItems));

        if (item.modelPath is not null) File.Delete(item.modelPath);
        File.Delete(item.dataPath);
    }
}
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private string saveDirectoryName = "SavedAssets";

    public string GetSaveDirectoryPath() => Path.Combine(Application.persistentDataPath, saveDirectoryName);
}
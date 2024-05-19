using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SaveManager
{
    [Serializable]
    public record AssetSaveFileItem
    {
        public string dataPath;
        [CanBeNull] public string modelPath;
        public string name;
        public string id;

        public AssetSaveFileItem(string id, string name, string dataPath, [CanBeNull] string modelPath)
        {
            this.id = id;
            this.name = name;
            this.dataPath = dataPath;
            this.modelPath = modelPath;
        }
    }

    [Serializable]
    public class AssetsSaveFile
    {
        public List<AssetSaveFileItem> items = new();
    }
}
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace SaveManager
{
    [Serializable]
    public record StoreStateSaveFileItem
    {
        public string id;
        public Vector3 position;
        public Quaternion rotation;
        [CanBeNull] public string modelPath;

        public StoreStateSaveFileItem(string id, Vector3 position, Quaternion rotation, [CanBeNull] string modelPath = null)
        {
            this.id = id;
            this.position = position;
            this.rotation = rotation;
            this.modelPath = modelPath;
        }
    }

    [Serializable]
    public class StoreStateSaveFile
    {
        public List<StoreStateSaveFileItem> items = new();
    }
}
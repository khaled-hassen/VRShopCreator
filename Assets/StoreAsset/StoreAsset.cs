using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace StoreAsset
{
    [Serializable]
    public class StoreAssetSpec
    {
        public string name;
        public string value;
    }

    [Serializable]
    public class StoreAssetData
    {
        public string assetName { get; set; }
        public List<StoreAssetSpec> assetSpecs { get; } = new();

        public void AddSpecification(string name, string value)
        {
            assetSpecs.Add(new StoreAssetSpec { name = name, value = value });
        }
    }

    public class StoreAsset : MonoBehaviour
    {
        public StoreAssetData assetData { get; } = new();

        private void Awake()
        {
            gameObject.AddComponent<BoxCollider>();
            gameObject.AddComponent<Rigidbody>();
            gameObject.AddComponent<XRGrabInteractable>();
        }
    }
}
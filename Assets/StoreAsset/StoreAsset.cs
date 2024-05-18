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
        public StoreAssetData assetData { get; set; } = new();

        private void Awake()
        {
            var currentRotation = gameObject.transform.rotation;
            var currentScale = gameObject.transform.localScale;

            // fix scale
            gameObject.transform.localScale = Vector3.one * 0.01f;
            gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            var bounds = new Bounds(gameObject.transform.position, Vector3.zero);
            foreach (var child in gameObject.GetComponentsInChildren<Renderer>())
                bounds.Encapsulate(child.bounds);

            var localCenter = bounds.center - gameObject.transform.position;
            bounds.center = localCenter;

            gameObject.transform.rotation = currentRotation;
            gameObject.transform.localScale = currentScale;

            var collider = gameObject.AddComponent<BoxCollider>();
            collider.center = new Vector3(bounds.center.x, bounds.center.y * 100f, bounds.center.z);
            collider.size = bounds.size * 100f;

            gameObject.AddComponent<Rigidbody>();
            var interactable = gameObject.AddComponent<XRGrabInteractable>();
            interactable.interactionLayers = 2;
            interactable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        }
    }
}
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace StoreAsset
{
    [CustomEditor(typeof(StoreAsset))]
    public class StoreAssetEditor : Editor
    {
        private UniqueId _id;
        private SaveManager.SaveManager _saveManager;

        private void OnEnable()
        {
            _saveManager = FindObjectOfType<SaveManager.SaveManager>();
            if (_saveManager is null) throw new Exception("SaveManager not found in the scene! Application cannot continue.");

            _id = target.GetComponent<UniqueId>();
            if (_id is null) throw new Exception("Missing UniqueId component on the object! Application cannot continue.");

            var storeAsset = (StoreAsset)target;
            _saveManager.LoadAsset(_id.uuid, out var loadedData);
            storeAsset.assetData = loadedData;
        }


        public override void OnInspectorGUI()
        {
            var storeAsset = (StoreAsset)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            // Display asset name
            storeAsset.assetData.assetName = EditorGUILayout.TextField("Asset Name", storeAsset.assetData.assetName);

            // Display asset specifications
            EditorGUILayout.LabelField("Asset Specifications", EditorStyles.boldLabel);

            for (var i = 0; i < storeAsset.assetData.assetSpecs.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name", GUILayout.Width(50));
                storeAsset.assetData.assetSpecs[i].name =
                    EditorGUILayout.TextField(storeAsset.assetData.assetSpecs[i].name, GUILayout.ExpandWidth(true));

                EditorGUILayout.LabelField("Value", GUILayout.Width(50));
                storeAsset.assetData.assetSpecs[i].value =
                    EditorGUILayout.TextField(storeAsset.assetData.assetSpecs[i].value, GUILayout.ExpandWidth(true));

                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    storeAsset.assetData.assetSpecs.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Specification")) storeAsset.assetData.assetSpecs.Add(new StoreAssetSpec());

            if (GUILayout.Button("Save to disk")) _saveManager.SaveAsset(_id, storeAsset.assetData, null);

            EditorGUI.EndDisabledGroup();
        }
    }
}
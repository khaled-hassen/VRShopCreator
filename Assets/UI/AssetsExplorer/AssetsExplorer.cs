using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Dummiesman;
using StoreAsset;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AssetsExplorer
{
    public class AssetsExplorer : UIScreen
    {
        private const int ItemsPerRow = 3;

        [SerializeField] private GameObject contentContainer;
        [SerializeField] private GameObject rowPrefab;
        [SerializeField] private GameObject itemContainerPrefab;

        private SaveManager _saveManager;

        private void Awake()
        {
            _saveManager = FindObjectOfType<SaveManager>();
            if (_saveManager is null) throw new Exception("SaveManager not found in the scene! Application cannot continue.");
        }

        public event Action OnOpenAddNewAssetScreen;
        public event Action<StoreAssetData> OnEditItem;

        public void LoadUI() => RenderScreenContent(_saveManager.GetSaveDirectoryPath());

        private void RenderScreenContent(string path)
        {
            var assets = Directory.GetDirectories(path);
            var rows = (assets.Length + ItemsPerRow - 1) / ItemsPerRow;

            for (var i = 0; i < rows; i++)
            {
                var panelInstance = Instantiate(
                    rowPrefab,
                    contentContainer.transform.position,
                    contentContainer.transform.rotation,
                    contentContainer.transform
                );
                RenderRowItems(assets, i, panelInstance);

                var contentFitter = panelInstance.AddComponent<ContentSizeFitter>();
                contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                // needs to be called twice to force the layout to update
                LayoutRebuilder.ForceRebuildLayoutImmediate(panelInstance.transform as RectTransform);
                LayoutRebuilder.ForceRebuildLayoutImmediate(panelInstance.transform as RectTransform);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(contentContainer.transform as RectTransform);
        }

        private void RenderRowItems(IReadOnlyList<string> assets, int rowIndex, GameObject panelInstance)
        {
            Enumerable.Range(rowIndex * ItemsPerRow, ItemsPerRow)
                .TakeWhile(index => index < assets.Count)
                .ToList()
                .ForEach(index => RenderDirectoryItem(panelInstance, assets[index]));
        }

        private void RenderDirectoryItem(GameObject panel, string path)
        {
            var itemContainer = Instantiate(
                itemContainerPrefab,
                panel.transform.position,
                panel.transform.rotation,
                panel.transform
            );


            var assetName = itemContainer.transform.Find("AssetContainer/AssetName");
            if (assetName is not null)
            {
                var text = assetName.GetComponent<TextMeshProUGUI>();
                if (text is not null) text.text = Path.GetFileName(path);
            }

            var actionsContainer = itemContainer.transform.Find("ActionsContainer");
            if (actionsContainer is null) return;

            var importBtnContainer = actionsContainer.Find("Import");
            if (importBtnContainer is not null) importBtnContainer.GetComponent<Button>()?.onClick.AddListener(() => AddAssetToStore(path));

            var editBtnContainer = actionsContainer.Find("Edit");
            if (editBtnContainer is not null) editBtnContainer.GetComponent<Button>()?.onClick.AddListener(() => EditAsset(path));

            var deleteBtnContainer = actionsContainer.Find("Delete");
            if (deleteBtnContainer is not null) deleteBtnContainer.GetComponent<Button>()?.onClick.AddListener(() => DeleteAsset(path));
        }

        private StoreAssetData LoadAssetData(string path)
        {
            var formatter = new BinaryFormatter();
            var file = File.Open(Path.Combine(path, "data.dat"), FileMode.Open);
            var data = (StoreAssetData)formatter.Deserialize(file);
            file.Close();
            return data;
        }

        private void AddAssetToStore(string path)
        {
            if (!Directory.Exists(path))
            {
                Debug.LogError("File not found: " + path);
                return;
            }

            var data = LoadAssetData(path);
            var model = new OBJLoader().Load(Path.Combine(path, "mesh.obj"));
            if (model is null)
            {
                Debug.LogError("Failed to load Asset file at path: " + Path.Combine(path, "mesh.obj"));
                return;
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

            var storeAsset = model.AddComponent<StoreAsset.StoreAsset>();
            storeAsset.assetData = data;
            OnCloseWindowClick();
        }

        private void EditAsset(string path) => OnEditItem?.Invoke(LoadAssetData(path));

        private void DeleteAsset(string path)
        {
            Directory.Delete(path, true);
            UpdateUI();
        }

        public void UpdateUI()
        {
            foreach (Transform child in contentContainer.transform) Destroy(child.gameObject);
            LoadUI();
        }

        public void OpenAddNewAssetClick() => OnOpenAddNewAssetScreen?.Invoke();

        public void SaveStoreToDisk()
        {
        }
    }
}
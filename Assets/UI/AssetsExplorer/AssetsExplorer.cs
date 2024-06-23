using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Dummiesman;
using SaveManager;
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

        private SaveManager.SaveManager _saveManager;

        private void Awake()
        {
            _saveManager = FindObjectOfType<SaveManager.SaveManager>();
            if (_saveManager is null) throw new Exception("SaveManager not found in the scene! Application cannot continue.");
        }

        public event Action OnOpenAddNewAssetScreen;
        public event Action<StoreAssetData> OnEditItem;

        public void LoadUI() => RenderScreenContent(_saveManager.LoadAssets());

        private void RenderScreenContent(AssetsSaveFile assetsSaveFile)
        {
            var assets = assetsSaveFile.items;
            var rows = (assets.Count + ItemsPerRow - 1) / ItemsPerRow;

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

        private void RenderRowItems(IReadOnlyList<AssetSaveFileItem> assets, int rowIndex, GameObject panelInstance)
        {
            Enumerable.Range(rowIndex * ItemsPerRow, ItemsPerRow)
                .TakeWhile(index => index < assets.Count)
                .ToList()
                .ForEach(index => RenderDirectoryItem(panelInstance, assets[index]));
        }

        private void RenderDirectoryItem(GameObject panel, AssetSaveFileItem asset)
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
                if (text is not null) text.text = asset.name;
            }

            var actionsContainer = itemContainer.transform.Find("ActionsContainer");
            if (actionsContainer is null) return;

            var importBtnContainer = actionsContainer.Find("Import");
            if (importBtnContainer is not null) importBtnContainer.GetComponent<Button>()?.onClick.AddListener(() => AddAssetToStore(asset));

            var editBtnContainer = actionsContainer.Find("Edit");
            if (editBtnContainer is not null) editBtnContainer.GetComponent<Button>()?.onClick.AddListener(() => EditAsset(asset));

            var deleteBtnContainer = actionsContainer.Find("Delete");
            if (deleteBtnContainer is not null) deleteBtnContainer.GetComponent<Button>()?.onClick.AddListener(() => DeleteAsset(asset));
        }

        private StoreAssetData LoadAssetData(string path)
        {
            var formatter = new BinaryFormatter();
            var file = File.Open(path, FileMode.Open);
            var data = (StoreAssetData)formatter.Deserialize(file);
            file.Close();
            return data;
        }

        private void AddAssetToStore(AssetSaveFileItem asset)
        {
            var model = new OBJLoader().Load(asset.modelPath);
            if (model is null)
            {
                Debug.LogError("Failed to load Asset file at path: " + asset.modelPath);
                return;
            }

            var id = model.AddComponent<UniqueId>();
            id.uuid = asset.id;
            model.AddComponent<StoreAsset.StoreAsset>();
            model.AddComponent<AssetPopup.AssetPopup>();
            OnCloseWindowClick();
        }

        private void EditAsset(AssetSaveFileItem asset) => OnEditItem?.Invoke(LoadAssetData(asset.dataPath));

        private void DeleteAsset(AssetSaveFileItem asset)
        {
            _saveManager.DeleteAsset(asset.id);
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
            _saveManager.SaveStoreState();
            OnCloseWindowClick();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public Action OnOpenAddNewAssetScreen;

        private void Awake()
        {
            _saveManager = FindObjectOfType<SaveManager>();
            if (_saveManager is null) throw new Exception("SaveManager not found in the scene! Application cannot continue.");
        }

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

        private void AddAssetToStore(string path)
        {
            Debug.Log("Add asset to store: " + path);
        }

        private void EditAsset(string path)
        {
            Debug.Log("Edit asset: " + path);
        }

        private void DeleteAsset(string path)
        {
            Debug.Log("Delete asset: " + path);
        }

        public void OpenAddNewAssetClick() => OnOpenAddNewAssetScreen?.Invoke();

        public void SaveStoreToDisk()
        {
        }
    }
}
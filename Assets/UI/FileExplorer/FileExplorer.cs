using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FileExplorer
{
    public class FileExplorer : UIScreen
    {
        public delegate void OnImportAssetCallback(string filePath);

        private const int FileWidth = 88;
        private const int FolderWidth = 140;
        private const int ItemHeight = 100;
        private const int ItemsPerRow = 5;

        [SerializeField] private GameObject contentContainer;
        [SerializeField] private GameObject importAssetButton;
        [SerializeField] private Sprite folderSprite;
        [SerializeField] private Sprite fileSprite;
        [SerializeField] private GameObject rowPrefab;
        [SerializeField] private TextMeshProUGUI locationText;
        [SerializeField] private int textCharacterLimit = 27;
        private string _currentPath;
        private string _selectedFilePath;

        public event OnImportAssetCallback OnImportAsset;

        public void LoadUI() =>
            RenderScreenContent(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

        private void RenderScreenContent(string path)
        {
            const string supportedFileTypes = "obj";
            _currentPath = path;
            var folders = Directory.GetDirectories(_currentPath);
            var files = Directory.GetFiles(_currentPath)
                .Where(file => supportedFileTypes.Contains(Path.GetExtension(file).TrimStart('.').ToLower()))
                .ToArray();

            path = _currentPath.Replace(Path.DirectorySeparatorChar, '/');
            locationText.text = path.Length > textCharacterLimit
                ? "..." + path.Substring(path.Length - textCharacterLimit + 3)
                : path;

            RenderDirectoryContent(folders, files);
        }

        private void RenderDirectoryContent(string[] folders, string[] files)
        {
            var totalItems = folders.Length + files.Length;
            var rows = (totalItems + ItemsPerRow - 1) / ItemsPerRow; // Calculate the total number of rows needed

            for (var i = 0; i < rows; i++)
            {
                var panelInstance = Instantiate(
                    rowPrefab,
                    contentContainer.transform.position,
                    contentContainer.transform.rotation,
                    contentContainer.transform
                );
                RenderRowItems(folders, files, i, totalItems, panelInstance);

                var contentFitter = panelInstance.AddComponent<ContentSizeFitter>();
                contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                // needs to be called twice to force the layout to update
                LayoutRebuilder.ForceRebuildLayoutImmediate(panelInstance.transform as RectTransform);
                LayoutRebuilder.ForceRebuildLayoutImmediate(panelInstance.transform as RectTransform);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(contentContainer.transform as RectTransform);
        }

        private (Sprite sprite, int width, string path) GetDirectoryItemData(int index, string[] folders, string[] files)
        {
            if (index < folders.Length) return (folderSprite, FolderWidth, folders[index]);
            if (index < folders.Length + files.Length) return (fileSprite, FileWidth, files[index - folders.Length]);
            return (null, 0, "");
        }

        private void RenderRowItems(string[] folders, string[] files, int rowIndex, int totalItems, GameObject panelInstance)
        {
            Enumerable.Range(rowIndex * ItemsPerRow, ItemsPerRow)
                .TakeWhile(index => index < totalItems)
                .Select(index => GetDirectoryItemData(index, folders, files))
                .Where(item => item.sprite is not null)
                .ToList()
                .ForEach(item => RenderDirectoryItem(panelInstance, item.path, item.sprite, item.width));
        }

        private void RenderDirectoryItem(GameObject parent, string path, Sprite sprite, int width)
        {
            var itemContainer = new GameObject("ItemContainer");
            itemContainer.transform.SetParent(parent.transform, false);

            var rectTransform = itemContainer.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(FolderWidth, 0);

            var verticalLayoutGroup = itemContainer.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.spacing = 12;
            verticalLayoutGroup.childControlWidth = false;
            verticalLayoutGroup.childControlHeight = false;
            verticalLayoutGroup.childForceExpandWidth = true;
            verticalLayoutGroup.childForceExpandHeight = false;
            verticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
            verticalLayoutGroup.padding = new RectOffset(10, 10, 10, 10);

            var imageObject = new GameObject("Image");
            imageObject.transform.SetParent(itemContainer.transform, false);

            var image = imageObject.AddComponent<Image>();
            image.sprite = sprite;
            image.rectTransform.sizeDelta = new Vector2(width, ItemHeight);

            var textObject = new GameObject("Text");
            textObject.transform.SetParent(itemContainer.transform, false);

            var text = textObject.AddComponent<TextMeshProUGUI>();
            text.text = Path.GetFileName(path);
            text.fontSize = 20;
            text.alignment = TextAlignmentOptions.Center;
            text.fontStyle = FontStyles.Bold;
            text.font = font;
            text.rectTransform.sizeDelta = new Vector2(width, 0);

            var textContent = textObject.AddComponent<ContentSizeFitter>();
            textContent.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var contentFitter = itemContainer.AddComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var btnImage = itemContainer.AddComponent<Image>();
            var button = itemContainer.AddComponent<Button>();
            button.targetGraphic = btnImage;
            ColorUtility.TryParseHtmlString("#747474", out var color);
            button.colors = new ColorBlock
            {
                normalColor = Color.clear,
                highlightedColor = color,
                pressedColor = color,
                selectedColor = color,
                disabledColor = Color.clear,
                fadeDuration = 0.3f,
                colorMultiplier = 1
            };
            button.onClick.AddListener(() => OnItemClick(path));
        }

        private void UpdateScreenContent(string path)
        {
            foreach (Transform child in contentContainer.transform) Destroy(child.gameObject);
            RenderScreenContent(path);
        }

        private void OnItemClick(string path)
        {
            if (Directory.Exists(path))
            {
                UpdateScreenContent(path);
                SelectFile(null);
            }
            else
            {
                SelectFile(path);
            }
        }

        private void SelectFile([CanBeNull] string path)
        {
            _selectedFilePath = path;
            importAssetButton.SetActive(path is not null);
        }

        public void OnBackClick()
        {
            var parentPath = Directory.GetParent(_currentPath)?.FullName;
            if (parentPath is not null) UpdateScreenContent(parentPath);
        }

        public void OnImportClick()
        {
            if (_selectedFilePath is null) return;
            OnImportAsset?.Invoke(_selectedFilePath);
            SelectFile(null);
        }
    }
}
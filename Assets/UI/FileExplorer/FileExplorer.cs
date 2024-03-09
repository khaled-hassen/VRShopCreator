using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FileExplorer
{
    public class FileExplorer : UIScreen
    {
        [SerializeField] private GameObject contentContainer;
        [SerializeField] private Sprite folderSprite;
        [SerializeField] private Sprite fileSprite;
        [SerializeField] private GameObject rowPrefab;
        [SerializeField] private TextMeshProUGUI locationText;
        [SerializeField] private int textCharacterLimit = 27;
        private readonly int _fileWidth = 88;
        private readonly int _folderWidth = 140;
        private readonly int _itemHeight = 100;
        private readonly int _itemsPerRow = 5;
        private string _currentPath;

        public override void LoadUI() => RenderScreenContent(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

        private void RenderScreenContent(string path)
        {
            const string supportedFileTypes = "fbx";
            _currentPath = path;
            string[] folders = Directory.GetDirectories(_currentPath);
            string[] files = Directory.GetFiles(_currentPath)
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
            int totalItems = folders.Length + files.Length;
            int rows = (totalItems + _itemsPerRow - 1) / _itemsPerRow; // Calculate the total number of rows needed

            for (var i = 0; i < rows; i++)
            {
                GameObject panelInstance = Instantiate(rowPrefab, Vector3.zero, Quaternion.identity, contentContainer.transform);
                Vector3 localPosition = panelInstance.transform.localPosition;
                localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
                panelInstance.transform.localPosition = localPosition;

                RenderRowItems(folders, files, i, totalItems, panelInstance);

                var contentFitter = panelInstance.AddComponent<ContentSizeFitter>();
                contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            Canvas.ForceUpdateCanvases();
        }

        private (Sprite sprite, int width, string path) GetDirectoryItemData(int index, string[] folders, string[] files)
        {
            if (index < folders.Length) return (folderSprite, _folderWidth, folders[index]);
            if (index < folders.Length + files.Length) return (fileSprite, _fileWidth, files[index - folders.Length]);
            return (null, 0, "");
        }

        private void RenderRowItems(string[] folders, string[] files, int rowIndex, int totalItems, GameObject panelInstance)
        {
            Enumerable.Range(rowIndex * _itemsPerRow, _itemsPerRow)
                .TakeWhile(index => index < totalItems)
                .Select(index => GetDirectoryItemData(index, folders, files))
                .Where(item => item.sprite is not null)
                .ToList()
                .ForEach(item => RenderDirectoryItem(panelInstance, item.path, item.sprite, item.width));
            Canvas.ForceUpdateCanvases();
        }

        private void RenderDirectoryItem(GameObject parent, string path, Sprite sprite, int width)
        {
            var itemContainer = new GameObject("ItemContainer");
            itemContainer.transform.SetParent(parent.transform, false);

            var rectTransform = itemContainer.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(_folderWidth, 0);

            var verticalLayoutGroup = itemContainer.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.spacing = 12;
            verticalLayoutGroup.childControlWidth = false;
            verticalLayoutGroup.childControlHeight = false;
            verticalLayoutGroup.childForceExpandWidth = true;
            verticalLayoutGroup.childForceExpandHeight = false;
            verticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;

            var imageObject = new GameObject("Image");
            imageObject.transform.SetParent(itemContainer.transform, false);

            var image = imageObject.AddComponent<Image>();
            image.sprite = sprite;
            image.rectTransform.sizeDelta = new Vector2(width, _itemHeight);

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

            var button = itemContainer.AddComponent<Button>();
            button.onClick.AddListener(() => OnItemClick(path));
        }

        private void UpdateScreenContent(string path)
        {
            foreach (Transform child in contentContainer.transform) Destroy(child.gameObject);
            RenderScreenContent(path);
        }

        private void OnItemClick(string path)
        {
            if (Directory.Exists(path)) UpdateScreenContent(path);
        }

        public void OnBackClick()
        {
            string parentPath = Directory.GetParent(_currentPath)?.FullName;
            if (parentPath is not null) UpdateScreenContent(parentPath);
        }
    }
}
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
        private readonly int _fileWidth = 88;
        private readonly int _folderWidth = 140;
        private readonly int _itemHeight = 100;
        private readonly int _itemsPerRow = 5;

        public override void LoadUI()
        {
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string[] folders = Directory.GetDirectories(documents);
            string[] files = Directory.GetFiles(documents);
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
        }

        private (Sprite sprite, int width, string title) GetDirectoryItemData(int index, string[] folders, string[] files)
        {
            if (index < folders.Length) return (folderSprite, _folderWidth, Path.GetFileName(folders[index]));
            if (index < folders.Length + files.Length) return (fileSprite, _fileWidth, Path.GetFileName(files[index - folders.Length]));
            return (null, 0, "");
        }

        private void RenderRowItems(string[] folders, string[] files, int rowIndex, int totalItems, GameObject panelInstance)
        {
            Enumerable.Range(rowIndex * _itemsPerRow, _itemsPerRow)
                .TakeWhile(index => index < totalItems)
                .Select(index => GetDirectoryItemData(index, folders, files))
                .Where(item => item.sprite is not null)
                .ToList()
                .ForEach(item => RenderDirectoryItem(panelInstance, item.title, item.sprite, item.width));
            Canvas.ForceUpdateCanvases();
        }

        private void RenderDirectoryItem(GameObject parent, string title, Sprite sprite, int width)
        {
            var itemContainer = new GameObject("ItemContainer");
            itemContainer.transform.SetParent(parent.transform, false);

            var rectTransform = itemContainer.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(width, 0);

            var verticalLayoutGroup = itemContainer.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.spacing = 12;
            verticalLayoutGroup.childControlWidth = true;
            verticalLayoutGroup.childControlHeight = false;
            verticalLayoutGroup.childForceExpandWidth = true;
            verticalLayoutGroup.childForceExpandHeight = false;

            var imageObject = new GameObject("Image");
            imageObject.transform.SetParent(itemContainer.transform, false);
            var image = imageObject.AddComponent<Image>();
            image.sprite = sprite;
            image.rectTransform.sizeDelta = new Vector2(0, _itemHeight);

            var textObject = new GameObject("Text");
            textObject.transform.SetParent(itemContainer.transform, false);

            var text = textObject.AddComponent<TextMeshProUGUI>();
            text.text = title;
            text.fontSize = 20;
            text.alignment = TextAlignmentOptions.Center;
            text.fontStyle = FontStyles.Bold;
            text.font = font;

            var textContent = textObject.AddComponent<ContentSizeFitter>();
            textContent.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var contentFitter = itemContainer.AddComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}
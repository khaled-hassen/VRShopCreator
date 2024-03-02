using System;
using System.IO;
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
            foreach (string folder in folders)
                Debug.Log(folder);
            foreach (string file in files)
                Debug.Log(file);

            int totalItems = folders.Length + files.Length;
            int rows = (totalItems + _itemsPerRow - 1) / _itemsPerRow; // Calculate the total number of rows needed

            for (var i = 0; i < rows; i++)
            {
                GameObject panelInstance = Instantiate(rowPrefab, Vector3.zero, Quaternion.identity, contentContainer.transform);
                Vector3 localPosition = panelInstance.transform.localPosition;
                localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
                panelInstance.transform.localPosition = localPosition;

                for (var j = 0; j < _itemsPerRow; j++)
                {
                    int index = i * _itemsPerRow + j;
                    if (index >= totalItems) break;

                    Sprite sprite = null;
                    var width = 0;
                    var title = "";

                    if (index < folders.Length)
                    {
                        sprite = folderSprite;
                        width = _folderWidth;
                        title = Path.GetFileName(folders[index]);
                    }
                    else if (index < folders.Length + files.Length)
                    {
                        sprite = fileSprite;
                        width = _fileWidth;
                        title = Path.GetFileName(files[index - folders.Length]);
                    }

                    if (sprite is not null)
                        RenderDirectoryItem(panelInstance, title, sprite, width);
                }

                var contentFitter = panelInstance.AddComponent<ContentSizeFitter>();
                contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
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


            var contentFitter = itemContainer.AddComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

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
        }
    }
}
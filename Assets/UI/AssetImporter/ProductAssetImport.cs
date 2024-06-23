using StoreAsset;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AssetImporter
{
    public class ProductAssetImport : AssetImporter
    {
        [SerializeField] protected GameObject contentContainer;
        [SerializeField] protected GameObject specRowPrefab;

        public override void LoadUI(StoreAssetData assetData)
        {
            base.LoadUI(assetData);
            for (var i = 2; i < assetData.assetSpecs.Count; i++) OnNewSpecClicked();

            var stockInput = contentContainer.transform.Find("ItemsInStock/InputContainer/Input");
            if (stockInput is not null)
                stockInput.GetComponent<TMP_InputField>().text = assetData.assetSpecs.Find(i => i.name == "Items in Stock")?.value;

            var assetDataIndex = 1;
            for (var i = 0; i < contentContainer.transform.childCount; i++)
            {
                var child = contentContainer.transform.GetChild(i);
                var specNameInput = child.Find("Inputs/Name/InputContainer/SpecName");
                var specValueInput = child.Find("Inputs/Value/InputContainer/SpecValue");
                if (specNameInput is null || specValueInput is null) continue;

                specNameInput.GetComponent<TMP_InputField>().text = assetData.assetSpecs[assetDataIndex].name;
                specValueInput.GetComponent<TMP_InputField>().text = assetData.assetSpecs[assetDataIndex].value;
                assetDataIndex++;
            }
        }

        public void OnNewSpecClicked()
        {
            var specRow = Instantiate(
                specRowPrefab,
                contentContainer.transform.position,
                contentContainer.transform.rotation,
                contentContainer.transform
            );

            var removeBtn = specRow.transform.Find("RemoveBtn").GetComponent<Button>();
            removeBtn.onClick.AddListener(() => Destroy(specRow));
            LayoutRebuilder.ForceRebuildLayoutImmediate(specRow.transform as RectTransform);
        }

        protected override void AddDataToAsset(StoreAssetData assetData)
        {
            var stockInput = contentContainer.transform.Find("ItemsInStock/InputContainer/Input");
            if (stockInput is not null)
                assetData.AddSpecification("Items in Stock", stockInput.GetComponent<TMP_InputField>().text);

            for (var i = 0; i < contentContainer.transform.childCount; i++)
            {
                var child = contentContainer.transform.GetChild(i);
                var specNameInput = child.Find("Inputs/Name/InputContainer/SpecName");
                var specValueInput = child.Find("Inputs/Value/InputContainer/SpecValue");
                if (specNameInput is null || specValueInput is null) continue;

                var specName = specNameInput.GetComponent<TMP_InputField>().text;
                var specValue = specValueInput.GetComponent<TMP_InputField>().text;
                assetData.AddSpecification(specName, specValue);
            }
        }
    }
}
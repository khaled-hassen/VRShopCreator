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
        }

        protected override void AddDataToAsset(StoreAssetData assetData)
        {
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
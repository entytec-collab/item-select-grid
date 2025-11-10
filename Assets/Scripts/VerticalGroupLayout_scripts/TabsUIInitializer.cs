using System.Collections.Generic;
using UnityEngine;

public class TabsUIInitializer : MonoBehaviour
{
    [SerializeField] private SectionsContainerController sectionsController;
    [SerializeField] Categories categoriesScriptableObject;
    [SerializeField] int columnsNumber;
    [SerializeField] private GameObject samplePrefab; //Only for testing

    private void Start()
    {
        var categories = categoriesScriptableObject.CategoriesList;                
        sectionsController.SetData(categories, columnsNumber); // Loads categories and items to UI
        sectionsController.OnItemSelected.AddListener(OnItemSelected); // Object selection subscription demo       
    }

    // Object selection subscription demo
    private void OnItemSelected(ItemData item)
    {
        //Code to execute on item selection
        Debug.Log($"[DemoBootSections] Seleccionado: {item.id} - {item.itemName}");
    }

    #region Testing methods
    private void CreateTestingItems()
    {
        // Construimos categorías de ejemplo
        var categories = new List<CategoryData>
        {
            new CategoryData
            {
                id = "camisas",
                categoryName = "Camisas",
                items = MakeItems("C", 7)
            },
            new CategoryData
            {
                id = "pantalones",
                categoryName = "Pantalones",
                items = MakeItems("P", 10)
            },
            new CategoryData
            {
                id = "accesorios",
                categoryName = "Accesorios",
                items = MakeItems("A", 5)
            },
            new CategoryData
            {
                id = "zapatos",
                categoryName = "Zapatos",
                items = MakeItems("Z", 8)
            }
        };
    }
    private List<ItemData> MakeItems(string prefix, int count)
    {
        var list = new List<ItemData>();
        for (int i = 0; i < count; i++)
        {
             var data = new ItemData
             {
                id = $"{prefix}_{i}",
                itemName = $"{prefix} Item {i}",
                thumbnail = samplePrefab,                
            };

            list.Add(data);
        }
        return list;
    }
    #endregion

}
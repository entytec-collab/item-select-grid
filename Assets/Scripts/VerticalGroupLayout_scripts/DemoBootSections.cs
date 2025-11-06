using System.Collections.Generic;
using UnityEngine;

public class DemoBootSections : MonoBehaviour
{
    [SerializeField] private SectionsContainerController sectionsController;
    [SerializeField] private GameObject samplePrefab;
    [SerializeField] Categories categoriesScriptableObject;

    private void Start()
    {
        var categories = categoriesScriptableObject.CategoriesList;

        //// Construimos categorías de ejemplo
        //var categories = new List<CategoryData>
        //{
        //    new CategoryData
        //    {
        //        id = "camisas",
        //        categoryName = "Camisas",
        //        items = MakeItems("C", 7)
        //    },
        //    new CategoryData
        //    {
        //        id = "pantalones",
        //        categoryName = "Pantalones",
        //        items = MakeItems("P", 10)
        //    },
        //    new CategoryData
        //    {
        //        id = "accesorios",
        //        categoryName = "Accesorios",
        //        items = MakeItems("A", 5)
        //    },
        //    new CategoryData
        //    {
        //        id = "zapatos",
        //        categoryName = "Zapatos",
        //        items = MakeItems("Z", 8)
        //    }
        //};

        // Suscribirse al evento de selección
        sectionsController.OnItemSelected.AddListener(OnItemSelected);

        // Cargar datos en el contenedor, con 4 columnas por sección
        sectionsController.SetData(categories, 4);
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

    private void OnItemSelected(ItemData item)
    {
        Debug.Log($"[DemoBootSections] Seleccionado: {item.id} - {item.itemName}");
    }
}
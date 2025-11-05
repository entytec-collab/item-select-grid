using System.Collections.Generic;
using UnityEngine;

public class DemoBootSections : MonoBehaviour
{
    [SerializeField] private SectionsContainerController sectionsController;
    [SerializeField] private Sprite sampleSprite; // asigna un sprite de prueba en el inspector

    private void Start()
    {
        // Construimos categorías de ejemplo
        var categorias = new List<CategoryData>
        {
            new CategoryData
            {
                id = "camisas",
                nombre = "Camisas",
                items = MakeItems("C", 7)
            },
            new CategoryData
            {
                id = "pantalones",
                nombre = "Pantalones",
                items = MakeItems("P", 10)
            },
            new CategoryData
            {
                id = "accesorios",
                nombre = "Accesorios",
                items = MakeItems("A", 5)
            },
            new CategoryData
            {
                id = "zapatos",
                nombre = "Zapatos",
                items = MakeItems("Z", 8)
            }
        };

        // Suscribirse al evento de selección
        sectionsController.OnItemSelected.AddListener(OnItemSelected);

        // Cargar datos en el contenedor, con 4 columnas por sección
        sectionsController.SetData(categorias, 4);
    }

    private ItemData[] MakeItems(string prefix, int count)
    {
        var arr = new ItemData[count];
        for (int i = 0; i < count; i++)
        {
            arr[i] = new ItemData
            {
                id = $"{prefix}_{i}",
                nombre = $"{prefix} Item {i}",
                thumbnail = sampleSprite,
                metadataJson = "{}"
            };
        }
        return arr;
    }

    private void OnItemSelected(ItemData item)
    {
        Debug.Log($"[DemoBootSections] Seleccionado: {item.id} - {item.nombre}");
    }
}
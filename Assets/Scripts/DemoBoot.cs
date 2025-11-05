using System.Collections.Generic;
using UnityEngine;

public class DemoBoot : MonoBehaviour
{
    [SerializeField] private CategoryGridController gridCtrl;
    [SerializeField] private Sprite sampleSprite;
    [SerializeField] private int columns;

    private void Start()
    {
        var cats = new List<CategoryData>
        {
            new CategoryData {
                id = "camisas", nombre = "Camisas",
                items = MakeItems("C", 7)
            },
            new CategoryData {
                id = "pantalones", nombre = "Pantalones",
                items = MakeItems("P", 10)
            },
            new CategoryData {
                id = "accesorios", nombre = "Accesorios",
                items = MakeItems("A", 4)
            }
        };

        gridCtrl.OnItemSelected.AddListener(OnItemSelected);
        gridCtrl.SetData(cats, columns);
    }

    private ItemData[] MakeItems(string prefix, int count)
    {
        var arr = new ItemData[count];
        for (int i = 0; i < count; i++)
        {
            arr[i] = new ItemData
            {
                id = $"{prefix}_{i}",
                nombre = $"{prefix} {i}",
                thumbnail = sampleSprite,
                metadataJson = "{}"
            };
        }
        return arr;
    }

    private void OnItemSelected(ItemData item)
    {
        Debug.Log($"Seleccionado: {item.id} - {item.nombre}");
    }
}
using System.Collections.Generic;
using UnityEngine;

public class DemoBoot : MonoBehaviour
{
    [SerializeField] private CategoryGridController gridCtrl;
    [SerializeField] private GameObject samplePrefab;
    [SerializeField] private int columns;

    private void Start()
    {
        var cats = new List<CategoryData>
        {
            new CategoryData {
                id = "camisas", categoryName = "Camisas",
                items = MakeItems("C", 7)
            },
            new CategoryData {
                id = "pantalones", categoryName = "Pantalones",
                items = MakeItems("P", 10)
            },
            new CategoryData {
                id = "accesorios", categoryName = "Accesorios",
                items = MakeItems("A", 4)
            }
        };

        gridCtrl.OnItemSelected.AddListener(OnItemSelected);
        gridCtrl.SetData(cats, columns);
    }

    private List<ItemData> MakeItems(string prefix, int count)
    {
        var list = new List<ItemData>();
        for (int i = 0; i < count; i++)
        {
            list[i] = new ItemData
            {
                id = $"{prefix}_{i}",
                itemName = $"{prefix} {i}",
                thumbnail = samplePrefab,
            };
        }
        return list;
    }

    private void OnItemSelected(ItemData item)
    {
        Debug.Log($"Seleccionado: {item.id} - {item.itemName}");
    }
}
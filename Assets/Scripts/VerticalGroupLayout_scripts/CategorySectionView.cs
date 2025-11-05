using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CategorySectionView : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private TMP_Text titleText;

    [Header("Grid")]
    [SerializeField] private RectTransform itemsGridRoot;
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private int columns = 4;

    [Header("Prefabs")]
    [SerializeField] private ItemCellView itemCellPrefab;

    public string CategoryId { get; private set; }

    public UnityEvent<ItemData> OnItemSelected = new UnityEvent<ItemData>();

    private readonly List<ItemCellView> _spawned = new();

    public void Build(CategoryData data, int columnCount)
    {
        CategoryId = data.id;
        columns = Mathf.Max(1, columnCount);
        if (titleText) titleText.text = data.nombre;

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;

        Clear();

        foreach (var item in data.items)
        {
            var cell = Instantiate(itemCellPrefab, itemsGridRoot);
            cell.Bind(item);
            cell.OnClick.AddListener(HandleItemClicked);
            _spawned.Add(cell);
        }

        // Si el grid o el Content usan auto layout, fuerza un rebuild para que la altura preferida se calcule.
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }

    private void HandleItemClicked(ItemData item) => OnItemSelected.Invoke(item);

    public void Clear()
    {
        for (int i = _spawned.Count - 1; i >= 0; i--)
        {
            if (_spawned[i] != null)
            {
                _spawned[i].OnClick.RemoveListener(HandleItemClicked);
                Destroy(_spawned[i].gameObject);
            }
        }
        _spawned.Clear();
    }
}
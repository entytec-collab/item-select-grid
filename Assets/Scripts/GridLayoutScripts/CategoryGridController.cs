using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class CategoryGridController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<CategoryData> categories = new List<CategoryData>();

    [Header("Grid")]
    [SerializeField] private RectTransform content;
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private int columns = 4;

    [Header("Prefabs")]
    [SerializeField] private ItemCellView itemCellPrefab;
    [SerializeField] private RectTransform spacerCellPrefab;

    [Header("Tabs")]
    [SerializeField] private CategoryTabsController tabsController;

    [Header("Selection")]
    public UnityEvent<ItemData> OnItemSelected = new UnityEvent<ItemData>();

    private ScrollRect _scrollRect;
    private readonly List<RectTransform> _spawned = new List<RectTransform>();
    private readonly List<(string categoryId, int startIndexInclusive, int endIndexInclusive)> _categoryRanges = new();

    // Cache métricas de celda para cálculos de scroll/rango
    private float _cellHeight;
    private float _verticalSpacing;

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        if (!grid) grid = content.GetComponent<GridLayoutGroup>();
        UpdateGridMetricsCache();
    }

    private void OnEnable()
    {
        _scrollRect.onValueChanged.AddListener(OnScrollChanged);
    }

    private void OnDisable()
    {
        _scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
    }

    public void SetData(List<CategoryData> data, int columnCount)
    {
        categories = data ?? new List<CategoryData>();
        columns = Mathf.Max(1, columnCount);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;

        RebuildGrid();
        BuildTabs();
    }

    private void UpdateGridMetricsCache()
    {
        _cellHeight = grid.cellSize.y;
        _verticalSpacing = grid.spacing.y;
    }

    private void RebuildGrid()
    {
        ClearContent();

        _categoryRanges.Clear();
        int globalIndex = 0;

        for (int c = 0; c < categories.Count; c++)
        {
            var cat = categories[c];

            // Relleno para asegurar nueva fila
            int remainder = globalIndex % columns;
            if (remainder != 0)
            {
                int fillers = columns - remainder;
                for (int f = 0; f < fillers; f++)
                {
                    var spacer = Instantiate(spacerCellPrefab, content);
                    _spawned.Add(spacer);
                    globalIndex++;
                }
            }

            int categoryStart = globalIndex;

            // Opcional: agregar título como fila extra “full-width”
            // Si deseas títulos visibles por categoría, mejor usa un VerticalLayoutGroup con secciones.
            // Aquí mantenemos grid plano: títulos no se recomiendan en esta variante.

            // Instanciar ítems de la categoría
            for (int i = 0; i < cat.items.Length; i++)
            {
                var cell = Instantiate(itemCellPrefab, content);
                cell.Bind(cat.items[i]);
                // Enlazar evento de selección
                cell.OnClick.AddListener(HandleItemSelected);
                _spawned.Add(cell.GetComponent<RectTransform>());
                globalIndex++;
            }

            int categoryEnd = globalIndex - 1;
            _categoryRanges.Add((cat.id, categoryStart, categoryEnd));
        }

        // Ajustar ContentSize si usas ContentSizeFitter o recalcular altura estimada
        // El GridLayoutGroup se encarga del orden; el ScrollRect mostrará correctamente.
    }

    private void HandleItemSelected(ItemData selected)
    {
        OnItemSelected.Invoke(selected);
    }

    private void ClearContent()
    {
        for (int i = _spawned.Count - 1; i >= 0; i--)
        {
            var rt = _spawned[i];
            if (!rt) continue;

            // Desuscribir si era ItemCellView
            var view = rt.GetComponent<ItemCellView>();
            if (view) view.OnClick.RemoveListener(HandleItemSelected);

            Destroy(rt.gameObject);
        }
        _spawned.Clear();
    }

    private void BuildTabs()
    {
        if (!tabsController) return;

        // Map para saber nombre y salto exacto
        var tabModels = new List<CategoryTabModel>();
        foreach (var cat in categories)
        {
            var range = _categoryRanges.Find(r => r.categoryId == cat.id);
            tabModels.Add(new CategoryTabModel
            {
                categoryId = cat.id,
                displayName = cat.nombre,
                startIndex = range.startIndexInclusive
            });
        }

        tabsController.BuildTabs(tabModels, JumpToCategory);
        // Sincroniza pestaña inicial con la primera categoría (si lo deseas)
        tabsController.SetActiveByCategoryId(categories.Count > 0 ? categories[0].id : null);
    }

    private void JumpToCategory(string categoryId)
    {
        var range = _categoryRanges.Find(r => r.categoryId == categoryId);
        if (range.startIndexInclusive < 0) return;

        // Calcular fila objetivo
        int targetRow = range.startIndexInclusive / columns;

        // Altura por fila = cellHeight + spacing
        float rowHeight = _cellHeight + _verticalSpacing;

        // Posición Y objetivo (anchoredPosition.y)
        float targetY = targetRow * rowHeight;

        // Clampear contra límites del content según viewport
        var contentRT = content;
        var viewportRT = _scrollRect.viewport ? _scrollRect.viewport : (RectTransform)contentRT.parent;

        float contentHeight = contentRT.rect.height;
        float viewportHeight = viewportRT.rect.height;
        float maxY = Mathf.Max(0f, contentHeight - viewportHeight);
        targetY = Mathf.Clamp(targetY, 0f, maxY);

        var pos = contentRT.anchoredPosition;
        pos.y = targetY;
        contentRT.anchoredPosition = pos;

        // Actualizar pestaña activa
        tabsController.SetActiveByCategoryId(categoryId);
    }

    private void OnScrollChanged(Vector2 normalizedPos)
    {
        // Determinar fila superior visible y la categoría correspondiente
        var contentRT = content;
        float currentY = contentRT.anchoredPosition.y;

        float rowHeight = _cellHeight + _verticalSpacing;
        int topRow = Mathf.FloorToInt(currentY / Mathf.Max(1f, rowHeight));
        int topIndex = Mathf.Clamp(topRow * columns, 0, _spawned.Count - 1);

        string visibleCategory = FindCategoryForIndex(topIndex);
        if (!string.IsNullOrEmpty(visibleCategory))
        {
            tabsController.SetActiveByCategoryId(visibleCategory);
        }
    }

    private string FindCategoryForIndex(int index)
    {
        for (int i = 0; i < _categoryRanges.Count; i++)
        {
            var r = _categoryRanges[i];
            if (index >= r.startIndexInclusive && index <= r.endIndexInclusive)
                return r.categoryId;
        }
        // Si caes en espaciadores entre categorías, toma la siguiente categoría activa más cercana
        for (int i = 0; i < _categoryRanges.Count; i++)
        {
            var r = _categoryRanges[i];
            if (index < r.startIndexInclusive)
                return r.categoryId;
        }
        return _categoryRanges.Count > 0 ? _categoryRanges[^1].categoryId : null;
    }
}
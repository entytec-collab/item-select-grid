using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class SectionsContainerController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<CategoryData> categories = new();

    [Header("Hierarchy")]
    [SerializeField] private RectTransform content; // el Content con VerticalLayoutGroup
    [SerializeField] private CategorySectionView sectionPrefab;

    [Header("Tabs")]
    [SerializeField] private CategoryTabsController tabsController;

    [Header("Grid config")]
    [SerializeField] private int columns = 4;

    public UnityEvent<ItemData> OnItemSelected = new UnityEvent<ItemData>();

    private ScrollRect _scrollRect;
    private readonly List<CategorySectionView> _sections = new();

    // Rangos de desplazamiento por sección (topY, bottomY en coordenadas del content)
    private readonly List<(string categoryId, float topY, float bottomY)> _sectionRanges = new();

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
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

        RebuildSections();
        BuildTabs();
        ComputeSectionRanges();
    }

    private void RebuildSections()
    {
        ClearSections();

        foreach (var cat in categories)
        {
            var section = Instantiate(sectionPrefab, content);
            section.Build(cat, columns);
            section.OnItemSelected.AddListener(OnItemSelected.Invoke);
            _sections.Add(section);
        }

        // Asegurar que el Content calcule su altura total
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    private void ClearSections()
    {
        foreach (var s in _sections)
        {
            if (!s) continue;
            s.OnItemSelected.RemoveAllListeners();
            s.Clear();
            Destroy(s.gameObject);
        }
        _sections.Clear();
        _sectionRanges.Clear();
    }

    private void BuildTabs()
    {
        if (!tabsController) return;

        var tabs = new List<CategoryTabModel>();
        foreach (var s in _sections)
        {
            tabs.Add(new CategoryTabModel
            {
                categoryId = s.CategoryId,
                displayName = s.name != null ? s.name : s.CategoryId,
                startIndex = 0 // no lo usamos aquí, pero el modelo lo requiere
            });
        }

        tabsController.BuildTabs(tabs, JumpToCategory);
        if (categories.Count > 0)
            tabsController.SetActiveByCategoryId(categories[0].id);
    }

    private void ComputeSectionRanges()
    {
        _sectionRanges.Clear();

        float cursorY = 0f;
        float spacing = content.GetComponent<VerticalLayoutGroup>()?.spacing ?? 0f;
        var vlg = content.GetComponent<VerticalLayoutGroup>();
        float topPadding = vlg ? vlg.padding.top : 0f;
        float bottomPadding = vlg ? vlg.padding.bottom : 0f;

        cursorY += topPadding;

        for (int i = 0; i < _sections.Count; i++)
        {
            var rt = (RectTransform)_sections[i].transform;
            float height = rt.rect.height;
            float top = cursorY;
            float bottom = cursorY + height;
            _sectionRanges.Add((_sections[i].CategoryId, top, bottom));
            cursorY = bottom + spacing;
        }

        // Añade el padding inferior para el límite máximo
        cursorY += bottomPadding;
    }

    private void JumpToCategory(string categoryId)
    {
        int idx = _sections.FindIndex(s => s.CategoryId == categoryId);
        if (idx < 0) return;

        var range = _sectionRanges[idx];
        // Desplaza el content para que el top de la sección quede al principio del viewport
        float targetY = range.topY;

        // Clampear para que no pase del final
        var viewport = _scrollRect.viewport ? _scrollRect.viewport : (RectTransform)content.parent;
        float maxY = Mathf.Max(0f, content.rect.height - viewport.rect.height);
        targetY = Mathf.Clamp(targetY, 0f, maxY);

        var pos = content.anchoredPosition;
        pos.y = targetY;
        content.anchoredPosition = pos;

        tabsController.SetActiveByCategoryId(categoryId);
    }

    private void OnScrollChanged(Vector2 _)
    {
        float currentY = content.anchoredPosition.y;
        var viewport = _scrollRect.viewport ? _scrollRect.viewport : (RectTransform)content.parent;
        float viewportTop = currentY;
        float viewportBottom = currentY + viewport.rect.height;

        // Categoría activa: la sección cuyo top esté dentro o justo por encima del viewport
        string active = null;
        for (int i = 0; i < _sectionRanges.Count; i++)
        {
            var r = _sectionRanges[i];
            // Si la parte superior de la sección está en la ventana o es la más cercana por encima
            if (r.topY >= viewportTop && r.topY <= viewportBottom)
            {
                active = r.categoryId;
                break;
            }
            if (r.topY < viewportTop) active = r.categoryId; // la última que quedó por encima
        }

        if (!string.IsNullOrEmpty(active))
            tabsController.SetActiveByCategoryId(active);
    }
}
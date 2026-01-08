using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SectionsController : MonoBehaviour
{
    [SerializeField] TabsItemSelectorManager tabsItemSelectorManager;

    // Advanced settings
    TabsItemSelectorSettings tabsItemSelectorSettings = new TabsItemSelectorSettings();

    List<GameObject> sectionsList = new List<GameObject>();
    // Used to jump between sections and change the active tab
    // The tuple fields represent (sectionName, startPos, endPos)
    List<(string sectionName, float startY, float endY, float startX, float endX)> _sectionRanges = new();

    ScrollRect _scrollRect;
    RectTransform _content;

    private void Awake()
    {
        tabsItemSelectorSettings = tabsItemSelectorManager.TabsItemSelectorSettings;
        _scrollRect = tabsItemSelectorManager.SectionsScrollView;
        _content = tabsItemSelectorManager.SectionsContent.GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        if (_scrollRect != null)
            _scrollRect.onValueChanged.AddListener(OnScrollChanged);
    }
    private void OnDisable()
    {
        if (_scrollRect != null)
            _scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
    }

    private void OnScrollChanged(Vector2 _) // Handles both vertical and horizontal scrolling
    {
        if (_content == null || _sectionRanges == null || _sectionRanges.Count == 0) return;

        var viewport = _scrollRect.viewport ? _scrollRect.viewport : (RectTransform)_content.parent;

        // Determine whether the sections content is arranged vertically or horizontally.
        bool isVertical = _scrollRect.vertical;

        string active = null;
        GameObject furtherActiveSection = null;

        if (isVertical)
        {
            float currentY = _content.anchoredPosition.y;
            float viewportTop = currentY;
            float viewportBottom = currentY + viewport.rect.height;

            var verticalLayout = _content.GetComponent<VerticalLayoutGroup>();
            float topPadding = verticalLayout ? verticalLayout.padding.top : 0f;
            float bottomPadding = verticalLayout ? verticalLayout.padding.bottom : 0f;
            float spacing = verticalLayout ? verticalLayout.spacing : 0f;

            Debug.Log($"currentY: {currentY}");

            for (int i = 0; i < _sectionRanges.Count; i++)
            {
                var range = _sectionRanges[i];

                // range.startPos and range.endPos represent top and bottom for vertical layout
                if (range.startY <= viewportTop + tabsItemSelectorSettings.TabChangeTolerance + topPadding + spacing)
                {
                    active = range.sectionName;
                    Debug.Log($"StartY: {range.startY}");

                }
                else
                {
                    Debug.Log($"Breaking at _sectionRange#: {i+1}");
                    break;
                }
            }

            // If scrolled to bottom, ensure last section is active
            float maxY = Mathf.Max(0f, _content.rect.height - viewport.rect.height);

            foreach (var section in sectionsList)
            {
                if (section.activeSelf) furtherActiveSection = section;
            }

            if (_content.anchoredPosition.y >= maxY - 0.001f && sectionsList.Count > 0)
                active = furtherActiveSection.name;
        }
        else // horizontal scroll handling
        {
            float currentX = -_content.anchoredPosition.x;
            float viewportLeft = currentX;
            float viewportRight = currentX + viewport.rect.width;

            var horizontalLayout = _content.GetComponent<HorizontalLayoutGroup>();
            float leftPadding = horizontalLayout ? horizontalLayout.padding.left : 0f;
            float rightPadding = horizontalLayout ? horizontalLayout.padding.right : 0f;
            float spacing = horizontalLayout ? horizontalLayout.spacing : 0f;

            Debug.Log($"currentX: {currentX}, viewport left: {viewportLeft}");

            for (int i = 0; i < _sectionRanges.Count; i++)
            {
                var range = _sectionRanges[i];
                Debug.Log($"StartX: {range.startX}");
                // range.startPos and range.endPos represent left and right for horizontal layout
                if (range.startX <= viewportLeft + tabsItemSelectorSettings.TabChangeTolerance + leftPadding + spacing)
                {
                    active = range.sectionName;
                }
                else
                {
                    Debug.Log($"Breaking at _sectionRange#: {i + 1}");

                    break;
                }
            }

            // If scrolled to far right (content end), ensure last section is active, only if 
            float maxX = Mathf.Max(0f, _content.rect.width - viewport.rect.width);

            foreach (var section in sectionsList)
            {
                if (section.activeSelf) furtherActiveSection = section;
            }

            if (currentX >= maxX - 0.001f && sectionsList.Count > 0)
                active = furtherActiveSection.name;
        }

        if (!string.IsNullOrEmpty(active))
            tabsItemSelectorManager.TabsController.SetActiveSection(active);

        // Toggle scroll indicators (optional)
        ToggleScrollIndicators();
    }

    private void ToggleScrollIndicators()
    {
        if (tabsItemSelectorManager == null) return;

        // Vertical indicators
        var up = tabsItemSelectorManager.ScrollUpIndicator;
        var down = tabsItemSelectorManager.ScrollDownIndicator;
        if (up || down)
        {
            if (_scrollRect.vertical)
            {
                float norm = _scrollRect.verticalNormalizedPosition;
                if (up) up.SetActive(norm < 0.99f);     // show up when not at top
                if (down) down.SetActive(norm > 0.01f); // show down when not at bottom
            }
        }

        // Horizontal indicators
        var left = tabsItemSelectorManager.ScrollLeftIndicator;
        var right = tabsItemSelectorManager.ScrollRightIndicator;
        if (left || right)
        {
            if (!_scrollRect.vertical)
            {
                float normH = _scrollRect.horizontalNormalizedPosition;
                if (left) left.SetActive(normH < 0.99f);    // show left when not at leftmost
                if (right) right.SetActive(normH > 0.01f); // show right when not at rightmost
            }
        }
    }

    public void PopulateSections(List<Category> categories, GameObject sectionPrefab, ScrollRect scrollView)
    {
        sectionsList.Clear();
        foreach (Category category in categories)
        {
            GameObject section = Instantiate(sectionPrefab, scrollView.content);
            tabsItemSelectorManager.UIBuilder.InitializeGroupLayoutUI(section, tabsItemSelectorManager.ItemsOrientation);
            section.name = category.CategoryName;
            sectionsList.Add(section);

            var sectionText = section.GetComponentInChildren<TextMeshProUGUI>();
            if (sectionText != null) { sectionText.text = category.CategoryName; }
            if (sectionText != null) { sectionText.gameObject.SetActive(tabsItemSelectorSettings.ShowCategoryLabels); }

            var itemsGridLayoutGroup = section.GetComponentInChildren<GridLayoutGroup>();
            var itemsToggleGroup = section.GetComponentInChildren<ToggleGroup>();

            if (itemsGridLayoutGroup)
            {
                if (itemsToggleGroup != null && tabsItemSelectorSettings.UseToggleGroups)
                    PopulateItems(itemsGridLayoutGroup.transform, category, itemsToggleGroup);
                else
                    PopulateItems(itemsGridLayoutGroup.transform, category);
            }
        }

        // Recalculate layout then compute ranges next frame
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
        StartCoroutine(ComputeRangesNextFrame());
    }

    private IEnumerator ComputeRangesNextFrame()
    {
        yield return null; // wait 1 frame for Layout to settle
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
        ComputeSectionRanges();

        // Set initial active section to first if available
        if (sectionsList.Count > 0)
            tabsItemSelectorManager.TabsController.SetActiveSection(sectionsList[0].name);

        ToggleScrollIndicators();
    }

    void PopulateItems(Transform transform, Category category)
    {
        foreach (Item item in tabsItemSelectorManager.Items)
        {
            var itemCategory = item.Category;
            if (itemCategory != category) continue;

            GameObject itemCell = Instantiate(tabsItemSelectorManager.ItemCellPrefab, transform);
            if (!itemCell.TryGetComponent<ItemBuilder>(out ItemBuilder itemBuilder))
                Debug.LogError("SectionsController: ItemCellPrefab does not have an ItemBuilder component. Add it to load items correctly");
            else
                itemBuilder.LoadItemPrefab(item);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(tabsItemSelectorManager.SectionsContent.transform as RectTransform);
    }

    void PopulateItems(Transform transform, Category category, ToggleGroup toggleGroup)
    {
        foreach (Item item in tabsItemSelectorManager.Items)
        {
            var itemCategory = item.Category;
            if (itemCategory != category) continue;

            GameObject itemCell = Instantiate(tabsItemSelectorManager.ItemCellPrefab, transform);
            if (!itemCell.TryGetComponent<ItemBuilder>(out ItemBuilder itemBuilder))
                Debug.LogError("SectionsController: ItemCellPrefab does not have an ItemBuilder component. Add it to load items correctly");
            else
                itemBuilder.LoadItemPrefab(item, toggleGroup);
        }

        // Recalculates the elements size sum to recalculate and reset the content size
        LayoutRebuilder.ForceRebuildLayoutImmediate(tabsItemSelectorManager.SectionsContent.transform as RectTransform);
    }
    
    private void ComputeSectionRanges()
    {
        // Computes sction ranges in the content by adding spacing, padding and height of each section to the list _sectionRanges, cursorY is used to temporarily store the current height of the calculations
        _sectionRanges.Clear();

        float cursorY = 0f;
        float cursorX = 0f;
        float spacing = 0f;
        if (_content.TryGetComponent<VerticalLayoutGroup>(out VerticalLayoutGroup verticalLayoutGroup))
        {
            spacing = verticalLayoutGroup.spacing;

        }
        else
        { 
            if (_content.TryGetComponent<HorizontalLayoutGroup>(out HorizontalLayoutGroup horizontalLayoutGroup)) 
            {
                spacing = horizontalLayoutGroup.spacing;
            }
        }

        var layoutGroup = _content.GetComponent<LayoutGroup>();
        float topPadding = layoutGroup ? layoutGroup.padding.top : 0f;
        float bottomPadding = layoutGroup ? layoutGroup.padding.bottom : 0f;
        float leftPadding = layoutGroup ? layoutGroup.padding.left : 0f;
        float rightPadding = layoutGroup ? layoutGroup.padding.right : 0f;

        cursorY += topPadding;
        cursorX += leftPadding;

        foreach (GameObject section in sectionsList)
        {
            if (section.activeSelf)
            {
                var rectTransform = (RectTransform)section.transform;
                float height = rectTransform.rect.height;
                float width = rectTransform.rect.width;
                float top = cursorY;
                float bottom = cursorY + height;
                float left = cursorX;
                float right = cursorX + width;
                _sectionRanges.Add((section.name, top, bottom, left, right));
                cursorY = bottom + spacing;
                cursorX = right + spacing;
            }
        }

        // Adds bottom padding for the bottom limit of the content 
        cursorY += bottomPadding;
        cursorX += leftPadding;
    }
    public void JumpToSection(string sectionName)
    {
        // Finds index of sectionsList by categoryName, depends on section GameObjects having same names as categories and being in the same order.
        // This should be ok since both are populated from the same Categories list in TabsItemSelectorManager and section GameObjects are named after category names when instantiated in this SectionsController script.

        int idx = _sectionRanges.FindIndex(s => s.sectionName == sectionName);
        if (idx < 0) return;
        var range = _sectionRanges[idx];

        var viewport = tabsItemSelectorManager.SectionsScrollView.viewport ? tabsItemSelectorManager.SectionsScrollView.viewport : (RectTransform)_content.parent;
        bool isVertical = _scrollRect.vertical;

        if (isVertical)
        {
            float targetY = range.startY;
            float maxY = Mathf.Max(0f, _content.rect.height - viewport.rect.height);
            targetY = Mathf.Clamp(targetY, 0f, maxY);
            var pos = _content.anchoredPosition;
            pos.y = targetY;
            _content.anchoredPosition = pos;
        }
        else
        {
            float targetX = range.startX;
            float maxX = Mathf.Max(0f, _content.rect.width - viewport.rect.width);
            targetX = Mathf.Clamp(targetX, 0f, maxX);
            var pos = _content.anchoredPosition;
            pos.x = -targetX;
            _content.anchoredPosition = pos;
        }

        tabsItemSelectorManager.TabsController.SetActiveSection(sectionName);
    }

    public void FilterSections(string sectionName, bool isActive)
    {
        int idx = sectionsList.FindIndex(s => s.name == sectionName);
        sectionsList[idx].SetActive(isActive);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);        
        ComputeSectionRanges();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_content);        
        
    }
}

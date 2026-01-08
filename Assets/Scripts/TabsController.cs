using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
    [SerializeField] TabsItemSelectorManager tabsItemSelectorManager;

    // Advanced settings variables
    TabsItemSelectorSettings tabsItemSelectorSettings;

    // tabs variables
    List<GameObject> tabsList = new List<GameObject>();

    // Struct replaces Component casting
    private struct TabComponents
    {
        public Button button;
        public Toggle toggle;
        public Image image;
    }

    Dictionary<string, TabComponents> _tabsDictionary = new Dictionary<string, TabComponents>();

    string _activeSectionName;

    bool isButton = false;
    bool isToggle = false;
    bool isImage = false;

    bool _swapImageWhenSelected;
    Sprite _imageUnselected;
    Sprite _imageSelected;

    bool _forceColors = false;
    Color _normalColor;
    Color _selectedColor;

    // coroutine handle for smooth scrolling
    Coroutine _currentScrollCoroutine;

    // Getters and setters
    public List<GameObject> TabsList { get { return tabsList; } set { tabsList = value; } }

    public void AddTabButtonAction(Button button, Action<string> onTabClicked, string sectionName)
    {
        button.onClick.AddListener(() => { onTabClicked?.Invoke(sectionName); });
    }
    public void AddTabToggleAction(Toggle toggle, Action<string> onTabClicked, string sectionName)
    {
        toggle.onValueChanged.AddListener((isOn) => { onTabClicked?.Invoke(sectionName); });
    }

    public void PopulateTabs()
    {
        // Read settings from manager if available
        if (tabsItemSelectorManager != null && tabsItemSelectorManager.TabsItemSelectorSettings != null)
            tabsItemSelectorSettings = tabsItemSelectorManager.TabsItemSelectorSettings;

        _forceColors = tabsItemSelectorSettings.ForceColors;
        _normalColor = tabsItemSelectorSettings.NormalColor;
        _selectedColor = tabsItemSelectorSettings.SelectedColor;

        _swapImageWhenSelected = tabsItemSelectorSettings.SwapImageWhenSelected;
        if (_swapImageWhenSelected)
        {
            _imageUnselected = tabsItemSelectorSettings.ImageUnselected;
            _imageSelected = tabsItemSelectorSettings.ImageSelected;
        }

        foreach (Category category in tabsItemSelectorManager.Categories)
        {
            GameObject tab = Instantiate(tabsItemSelectorManager.TabPrefab, tabsItemSelectorManager.TabsContent.transform);
            tab.name = category.CategoryName;
            tabsList.Add(tab);

            if (!tab.TryGetComponent<TabBuilder>(out TabBuilder tabBuilder))
                Debug.LogError("TabsController: TabPrefab does not have a TabBuilder component. Add it to load tabs correctly");
            else
                tabBuilder.LoadTabPrefab(category);

            // Create struct instance
            TabComponents components = new TabComponents();

            // BUTTON
            if (tab.TryGetComponent<Button>(out Button tabButton))
            {
                isButton = true;
                components.button = tabButton;

                AddTabButtonAction(tabButton, tabsItemSelectorManager.SectionsController.JumpToSection, tab.name);

                if (_swapImageWhenSelected)
                {
                    if (tab.TryGetComponent<Image>(out Image img))
                        components.image = img;
                }
                else
                {
                    // no image swap → use button directly
                }
            }
            // TOGGLE
            else if (tab.TryGetComponent<Toggle>(out Toggle tabToggle))
            {
                isToggle = true;
                components.toggle = tabToggle;


                if (_swapImageWhenSelected)
                {
                    if (tab.TryGetComponent<Image>(out Image img))
                        components.image = img;
                }
            }
            // IMAGE ONLY
            else if (tab.TryGetComponent<Image>(out Image tabImage))
            {
                isImage = true;
                components.image = tabImage;
            }

            _tabsDictionary[tab.name] = components;

            LayoutRebuilder.ForceRebuildLayoutImmediate(tabsItemSelectorManager.TabsContent.transform as RectTransform);
        }
    }

    public void SetActiveSection(string activeSection)
    {
        if (_activeSectionName == activeSection)
            return;

        _activeSectionName = activeSection;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        foreach (var tab in _tabsDictionary)
        {
            var components = tab.Value;
            bool isActive = tab.Key == _activeSectionName;

            // IMAGE SWAP MODE
            if (_swapImageWhenSelected && components.image != null)
            {
                components.image.sprite = isActive ? _imageSelected : _imageUnselected;
                continue;
            }

            // BUTTON MODE
            if (isButton && components.button != null)
            {
                var button = components.button;
                var colors = button.colors;
                var targetGraphic = button.targetGraphic;

                if (!_forceColors)
                    targetGraphic.color = isActive ? colors.highlightedColor : colors.normalColor;
                else
                    targetGraphic.color = isActive ? _selectedColor : _normalColor;

                continue;
            }

            // TOGGLE MODE
            if (isToggle && components.toggle != null)
            {
                var toggle = components.toggle;
                var colors = toggle.colors;
                var targetGraphic = toggle.targetGraphic;

                if (!_forceColors)
                    targetGraphic.color = isActive ? colors.highlightedColor : colors.normalColor;
                else
                    targetGraphic.color = isActive ? _selectedColor : _normalColor;

                continue;
            }

            // IMAGE MODE
            if (isImage && components.image != null)
            {
                components.image.color = isActive ? _selectedColor : _normalColor;
                continue;
            }
        }

        // Ensure the active tab is visible in the tabs scroll view (smooth)
        if (!string.IsNullOrEmpty(_activeSectionName))
            EnsureTabVisible(_activeSectionName);
    }

    private void EnsureTabVisible(string sectionName)
    {
        if (tabsItemSelectorManager == null || tabsItemSelectorManager.TabsContent == null || tabsItemSelectorManager.TabsScrollView == null)
            return;

        Transform tabTransform = tabsItemSelectorManager.TabsContent.transform.Find(sectionName);
        if (tabTransform == null) return;

        RectTransform contentRect = tabsItemSelectorManager.TabsContent.GetComponent<RectTransform>();
        RectTransform tabRect = tabTransform.GetComponent<RectTransform>();
        RectTransform viewport = tabsItemSelectorManager.TabsScrollView.viewport ? tabsItemSelectorManager.TabsScrollView.viewport : (RectTransform)contentRect.parent;

        // Force rebuild so rects have correct sizes
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

        Vector2 targetAnchored = contentRect.anchoredPosition;

        // Vertical scrolling
        if (tabsItemSelectorManager.TabsScrollView.vertical)
        {
            float tabHeight = tabRect.rect.height;
            float tabTop = -(tabRect.anchoredPosition.y + tabHeight/2);
            float tabBottom = tabTop + tabRect.rect.height;
            float viewportTop = contentRect.anchoredPosition.y;
            float viewportBottom = viewportTop + viewport.rect.height;

            float desiredY = contentRect.anchoredPosition.y;

            if (tabTop < viewportTop)
                desiredY = tabTop;
            else if (tabBottom > viewportBottom )
                desiredY = tabBottom - viewport.rect.height;

            float maxY = Mathf.Max(0f, contentRect.rect.height - viewport.rect.height);
            desiredY = Mathf.Clamp(desiredY, 0f, maxY);

            targetAnchored = new Vector2(contentRect.anchoredPosition.x, desiredY);
        }
        // Horizontal scrolling
        else if (tabsItemSelectorManager.TabsScrollView.horizontal)
        {
            float tabWidth = tabRect.rect.width;
            float tabLeft = tabRect.anchoredPosition.x - tabWidth/2;
            float tabRight = tabLeft + tabRect.rect.width;
            float viewportLeft = -contentRect.anchoredPosition.x;
            float viewportRight = viewportLeft + viewport.rect.width;


            float desiredX = contentRect.anchoredPosition.x;

            // Swipe left (move content right)
            if (tabLeft < viewportLeft)
                desiredX = (-tabLeft);
            //Swipe right (move content left)
            else if (tabRight > viewportRight )
                desiredX = -( (tabRight - viewport.rect.width));

            float maxX = Mathf.Max(0f, contentRect.rect.width - viewport.rect.width);
            desiredX = Mathf.Clamp(desiredX, -maxX, 0f);

            targetAnchored = new Vector2(desiredX, contentRect.anchoredPosition.y);
        }

        // Start smooth coroutine to move content to targetAnchored
        if (_currentScrollCoroutine != null) StopCoroutine(_currentScrollCoroutine);
        _currentScrollCoroutine = StartCoroutine(SmoothScrollContentTo(contentRect, targetAnchored, tabsItemSelectorSettings.TabScrollSmoothTime));
            Debug.Log("Content position: " +contentRect.anchoredPosition);
    
    }

    IEnumerator SmoothScrollContentTo(RectTransform contentRect, Vector2 targetAnchored, float duration)
    {
        if (contentRect == null) yield break;

        Vector2 start = contentRect.anchoredPosition;
        if (duration <= 0f)
        {
            contentRect.anchoredPosition = targetAnchored;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // Smooth step for nicer easing
            float s = t * t * (3f - 2f * t);
            contentRect.anchoredPosition = Vector2.Lerp(start, targetAnchored, s);
            yield return null;
        }

        contentRect.anchoredPosition = targetAnchored;
        _currentScrollCoroutine = null;
    }
}
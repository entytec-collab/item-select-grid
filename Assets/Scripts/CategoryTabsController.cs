using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class CategoryTabModel
{
    public string categoryId;
    public string displayName;
    public int startIndex;
}

public class CategoryTabsController : MonoBehaviour
{
    [SerializeField] private RectTransform tabsContainer;
    [SerializeField] private Button tabButtonPrefab;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color activeColor = Color.yellow;

    private readonly Dictionary<string, Button> _tabByCategory = new();
    private string _activeCategory;

    public void BuildTabs(List<CategoryTabModel> tabs, Action<string> onTabClicked)
    {
        ClearTabs();

        foreach (var t in tabs)
        {
            var btn = Instantiate(tabButtonPrefab, tabsContainer);
            var label = btn.GetComponentInChildren<TMP_Text>();
            if (label) label.text = t.displayName;

            string catId = t.categoryId;
            btn.onClick.AddListener(() => onTabClicked?.Invoke(catId));
            _tabByCategory[catId] = btn;
        }

        UpdateVisuals();
    }

    public void SetActiveByCategoryId(string categoryId)
    {
        if (string.IsNullOrEmpty(categoryId)) return;
        if (_activeCategory == categoryId) return;

        _activeCategory = categoryId;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        foreach (var kvp in _tabByCategory)
        {
            var btn = kvp.Value;
            var colors = btn.colors;
            // Alternativamente: cambiar Image color del targetGraphic
            var targetGraphic = btn.targetGraphic;
            if (targetGraphic) targetGraphic.color = kvp.Key == _activeCategory ? activeColor : normalColor;
        }
    }

    private void ClearTabs()
    {
        foreach (Transform child in tabsContainer)
        {
            var b = child.GetComponent<Button>();
            if (b) b.onClick.RemoveAllListeners();
            Destroy(child.gameObject);
        }
        _tabByCategory.Clear();
        _activeCategory = null;
    }
}
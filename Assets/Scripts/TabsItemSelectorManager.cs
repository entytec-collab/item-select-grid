using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct GridLayoutSettings
{
    public RectOffset padding;
    public Vector2 cellSize;
    public Vector2 spacing;
    public GridLayoutGroup.Corner startCorner;
    public GridLayoutGroup.Axis startAxis;
    public TextAnchor childAlignment;
    public GridLayoutGroup.Constraint constraint;
    public int constraintCount;
}
[System.Serializable]
public class TabsItemSelectorSettings
{
    [Header("Tabs settings")]

    [SerializeField] bool swapImageWhenSelected = false;
    [SerializeField] Sprite imageUnselected;
    [SerializeField] Sprite imageSelected;
    [SerializeField] bool forceColors = false;
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color selectedColor = Color.lightGoldenRodYellow;
    
    [Header("Tab scrolling")]
    [SerializeField] float tabChangeTolerance = 0;
    [SerializeField] float tabScrollSmoothTime = 0.18f; // seconds for smooth scroll


    [Header("Sections settings")]
    [SerializeField] bool showCategoryLabels = true;

    [Header("Items settings")]
    [SerializeField] bool showItemText = true;
    [SerializeField] bool useToggleGroups = true;

    [Header("Sections Oriented layout group settings")]
    [SerializeField] bool sectionsChildForceExpandWidth = true;
    [SerializeField] bool sectionsChildForceExpandHeight = true;
    [SerializeField] bool sectionsChildControlWidth = true;
    [SerializeField] bool sectionsChildControlHeight = true;

    [Header("Tabs Oriented layout group settings")]
    [SerializeField] bool tabsChildForceExpandWidth = true;
    [SerializeField] bool tabsChildForceExpandHeight = true;
    [SerializeField] bool tabsChildControlWidth = true;
    [SerializeField] bool tabsChildControlHeight = true;

    [Header("Sections Grid layout group settings")]
    [SerializeField] bool setGridLayoutSettings = false;
    [SerializeField] GridLayoutSettings itemGridLayoutSettings;
       
#region Getters and setters
    public bool SwapImageWhenSelected { get { return swapImageWhenSelected; } set { swapImageWhenSelected = value; } }
    public Sprite ImageUnselected { get { return imageUnselected; } set { imageUnselected = value; } }
    public Sprite ImageSelected { get { return imageSelected; } set { imageSelected = value; } }
    public bool ForceColors { get { return forceColors; } set { forceColors = value; } }
    public Color NormalColor { get { return normalColor; } set { normalColor = value; } }
    public Color SelectedColor { get { return selectedColor; } set { selectedColor = value; } }
    public float TabChangeTolerance { get { return tabChangeTolerance; } set { tabChangeTolerance = value; } }
    public float TabScrollSmoothTime { get => tabScrollSmoothTime; set => tabScrollSmoothTime = value; }

    public bool ShowCategoryLabels { get { return showCategoryLabels; } set { showCategoryLabels = value; } }

    public bool ShowItemText { get { return showItemText; } set { showItemText = value; } }
    public bool UseToggleGroups { get { return useToggleGroups; } set { useToggleGroups = value; } }

    public bool SectionsChildForceExpandWidth { get { return sectionsChildForceExpandWidth; } set { sectionsChildForceExpandWidth = value; } }
    public bool SectionsChildForceExpandHeight { get { return sectionsChildForceExpandHeight; } set { sectionsChildForceExpandHeight = value; } }
    public bool SectionsChildControlWidth { get { return sectionsChildControlWidth; } set { sectionsChildControlWidth = value; } }
    public bool SectionsChildControlHeight { get { return sectionsChildControlHeight; } set { sectionsChildControlHeight = value; } }

    public bool TabsChildForceExpandWidth { get { return tabsChildForceExpandWidth; } set { tabsChildForceExpandWidth = value; } }
    public bool TabsChildForceExpandHeight { get { return tabsChildForceExpandHeight; } set { tabsChildForceExpandHeight = value; } }
    public bool TabsChildControlWidth { get { return tabsChildControlWidth; } set { tabsChildControlWidth = value; } }
    public bool TabsChildControlHeight { get { return tabsChildControlHeight; } set { tabsChildControlHeight = value; } }

    public bool SetGridLayoutSettings { get { return setGridLayoutSettings; } set { setGridLayoutSettings = value; } }
    public GridLayoutSettings ItemGridLayoutSettings { get { return itemGridLayoutSettings; } set { itemGridLayoutSettings = value; } }

    #endregion

}
public enum ContentOrientation { Vertical, Horizontal }
   public enum LayoutType { SectionsLayout, GridLayout } // Pending to add functionality for GridLayout without sections separating in different lines


public class TabsItemSelectorManager : MonoBehaviour
{    
    [Header("General settings")]
    [SerializeField] ContentOrientation tabsOrientation = ContentOrientation.Vertical;
    [SerializeField] ContentOrientation itemsOrientation = ContentOrientation.Vertical;

    [Header("Layout references")]
    [SerializeField] ScrollRect tabsScrollView;
    [SerializeField] ScrollRect sectionsScrollView;
    [SerializeField] GameObject tabsContent;
    [SerializeField] GameObject sectionsContent;

    [Header("Categories and prefab References")]
    [SerializeField] List<Category> categories = new List<Category>();
    [SerializeField] List<Item> items = new List<Item>();
    [SerializeField] GameObject tabPrefab;
    [SerializeField] TabsController tabsController;
    [SerializeField] GameObject sectionPrefab;
    [SerializeField] SectionsController sectionsController;
    [SerializeField] GameObject itemCellPrefab;

    [Header("Script references")]
    [SerializeField] UIBuilder uIBuilder;

    [Header("Advanced Settings")]
    [SerializeField] TabsItemSelectorSettings tabsItemSelectorSettings = new TabsItemSelectorSettings();

    [Header("Optional scroll indicators (assign in inspector if needed)")]
    [SerializeField] GameObject scrollUpIndicator;
    [SerializeField] GameObject scrollDownIndicator;
    [SerializeField] GameObject scrollLeftIndicator;
    [SerializeField] GameObject scrollRightIndicator;

    // Tabs variables
    List<GameObject> tabsList = new List<GameObject>();

    // Sections variables
    List<GameObject> sectionsList = new List<GameObject>();

#region Getters and setters
    public ContentOrientation TabsOrientation { get { return tabsOrientation; } set { tabsOrientation = value; } }
    public ContentOrientation ItemsOrientation { get { return itemsOrientation; } set { itemsOrientation = value; } }

    public ScrollRect TabsScrollView { get { return tabsScrollView; } set { tabsScrollView = value; } }
    public ScrollRect SectionsScrollView { get { return sectionsScrollView; } set { sectionsScrollView = value; } }

    public GameObject TabsContent { get { return tabsContent; } set { tabsContent = value; } }
    public GameObject SectionsContent { get { return sectionsContent; } set { sectionsContent = value; } }

    public List<Category> Categories { get { return categories; } set { categories = value; } }
    public List<Item> Items { get { return items; } set { items = value; } }

    public GameObject TabPrefab { get { return tabPrefab; } set { tabPrefab = value; } }
    public TabsController TabsController { get { return tabsController; } set { tabsController = value; } }
    public GameObject SectionPrefab { get { return sectionPrefab; } set { sectionPrefab = value; } }
    public SectionsController SectionsController { get { return sectionsController; } set { sectionsController = value; } }
    public GameObject ItemCellPrefab { get { return itemCellPrefab; } set { itemCellPrefab = value; } }

    public UIBuilder UIBuilder { get { return uIBuilder; } set { uIBuilder = value; } }

    public TabsItemSelectorSettings TabsItemSelectorSettings { get { return tabsItemSelectorSettings; } set { tabsItemSelectorSettings = value; } }

    public GameObject ScrollUpIndicator { get { return scrollUpIndicator; } }
    public GameObject ScrollDownIndicator { get { return scrollDownIndicator; } }
    public GameObject ScrollLeftIndicator { get { return scrollLeftIndicator; } }
    public GameObject ScrollRightIndicator { get { return scrollRightIndicator; } }

    public List<GameObject> TabsList { get { return tabsList; } set { tabsList = value; } }
    public List<GameObject> SectionsList { get { return sectionsList; } set { sectionsList = value; } }
#endregion
        
    private void Start()
    {
        CreateTabsItemSelector();
    }
    public void CreateTabsItemSelector()
    {
        uIBuilder.PopulateUI();
        InitializeUI();
    }
    public void InitializeUI() // Initializes a Scroll rect for tabs, sections of items categories, and items themselves
    {
        // Initialize Content Size Fitters and Layout Groups
        uIBuilder. InitializeScrollUI(LayoutType.SectionsLayout, tabsScrollView, tabsContent, tabsOrientation); //Initialize tabs
        uIBuilder. InitializeScrollUI(LayoutType.SectionsLayout, sectionsScrollView, sectionsContent, itemsOrientation); //Initialize sections
    }    
}


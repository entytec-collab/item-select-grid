using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TabBuilder : MonoBehaviour
{
    [SerializeField] Image tabIconImage;
    [SerializeField] TextMeshProUGUI tabLabel;
    [SerializeField] Color tabColor;
    [SerializeField] Selectable clickableComponent;
    [SerializeField] string tabManagerGameObjectName;

    Category _category;

    SectionsController sectionsController;

    public UnityEvent<Category> OnClick = new UnityEvent<Category>();

    // ------------------------------------------------------
    // Public APIs:
    void AssignTabIcons()
    {
        if (_category.CategoryIcon != null)
            tabIconImage.sprite = _category.CategoryIcon;
    }
    void AssignTabLabels()
    {
        if (tabLabel != null)
            tabLabel.text = _category.CategoryName;
    }
    void SetButtonActions(Button button)
    {
        // button.onClick.RemoveAllListeners(); // Uncomment to override other previous listeners        
        button.onClick.AddListener(HandleClick);
    }
    void SetToggleActions(Toggle toggle)
    {
        // toggle.onValueChanged.RemoveAllListeners(); // Uncomment to override other previous listeners  
        toggle.onValueChanged.AddListener((isOn) => { if (isOn) HandleClick(); });

        // Logic to use tabs as item filters
        var tabsManagerGO = GameObject.Find(tabManagerGameObjectName);

        if (tabsManagerGO != null)
        {
            var tabsManager = tabsManagerGO.GetComponent<TabsItemSelectorManager>();

            if (tabsManagerGO != null)
            {

                 sectionsController = tabsManager.SectionsController;

                toggle.onValueChanged.AddListener((isOn) =>
                {
                    ManageToggleFilter(isOn);
                    
                });
            }
        }
        else
        {
            Debug.LogError("Tab Manager not found with the name " + tabManagerGameObjectName);
        }

    }
    void HandleClick()
    {
        OnClick.Invoke(_category);
        Debug.Log($"TabBuilder: Clicked on tab {_category.CategoryName}");
    }
    // ------------------------------------------------------
    // ------------------------------------------------------

    public void LoadTabPrefab(Category category)
    {        
        _category = category;

        AssignTabIcons();
        AssignTabLabels();
        SetClickableComponent();
    }    
    void SetClickableComponent()
    {
        if (clickableComponent == null) { Debug.LogWarning("TabBuilder: Clickable component is not assigned."); return; }

        if (clickableComponent is Button button)
            SetButtonActions(button);
        else if (clickableComponent is Toggle toggle)
            SetToggleActions(toggle);        
    }    
    void ManageToggleFilter(bool isOn)
    {
        if (isOn)
        {
            sectionsController.FilterSections(_category.CategoryName, isOn);
            StartCoroutine(WaitAFrameToJump());
        }
        else
        {
            sectionsController.FilterSections(_category.CategoryName, isOn);
        }
    }
    IEnumerator WaitAFrameToJump()
    {
        yield return null;
        sectionsController.JumpToSection(_category.CategoryName);

    }
    private void OnDestroy()
    {
        if (clickableComponent is Button button)
        {
            button.onClick.RemoveAllListeners();
        }
        else if (clickableComponent is Toggle toggle)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
    }
}

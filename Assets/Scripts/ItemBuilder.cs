using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemBuilder : MonoBehaviour
{
    [SerializeField] Image itemIconImage;
    [SerializeField] TextMeshProUGUI itemLabel;
    [SerializeField] Selectable clickableComponent;

    Item _item;

    public UnityEvent<Item> OnClick = new UnityEvent<Item>();

    // ------------------------------------------------------
    // Public APIs:
    void AssignItemIcons()
    {
        if (_item.ItemIcon != null)
            itemIconImage.sprite = _item.ItemIcon;
    }
    void AssignItemLabels()
    {
        if (itemLabel != null)
            itemLabel.text = _item.ItemName;
    }
    void AssignGameObject()
    {
        if (_item.ItemGameObject != null)
        {
            // Instantiate or assign the GameObject as needed
            GameObject itemGameObject = Instantiate(_item.ItemGameObject, this.transform);
        }
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
    }
    void HandleClick()
    {
        OnClick.Invoke(_item);
        Debug.Log($"ItemBuilder: Clicked on item {_item.ItemName}");
    }
    // ------------------------------------------------------
    // ------------------------------------------------------

    public void LoadItemPrefab(Item item)
    {
        if (item == null) { Debug.LogError("ItemBuilder: Item is null."); return; }
        
        _item = item;
        AssignItemIcons();
        AssignItemLabels();
        AssignGameObject();
        SetClickableComponent();
    }
    public void LoadItemPrefab(Item item, ToggleGroup toggleGroup)
    {
        if (item == null) { Debug.LogError("ItemBuilder: Item is null."); return; }
        
        _item = item;
        AssignItemIcons();
        AssignItemLabels();
        AssignGameObject();
        SetClickableComponent(toggleGroup);
    }
    void SetClickableComponent()
    {
        if (clickableComponent == null) { Debug.LogWarning("ItemBuilder: Clickable component is not assigned."); return; }

        if (clickableComponent is Button button)
            SetButtonActions(button);
        else if (clickableComponent is Toggle toggle)
            SetToggleActions(toggle);        
    }
    void SetClickableComponent(ToggleGroup toggleGroup)
    {
        if (clickableComponent == null) { Debug.LogWarning("ItemBuilder: Clickable component is not assigned."); return; }

        if (clickableComponent is Button button)
            SetButtonActions(button);
        else if (clickableComponent is Toggle toggle)
        {
            toggle.group = toggleGroup;
            SetToggleActions(toggle);
        }
    }

    

    private void OnValidate()
    {
        if (clickableComponent != null &&
            !(clickableComponent is Button) &&
            !(clickableComponent is Toggle))
        {
            Debug.LogError($"Item Builder{name}: Only Button or Toggle are allowed as clicable component in the Item prefab's ItemBuilder component");
            clickableComponent = null;
        }
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

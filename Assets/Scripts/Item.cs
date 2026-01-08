using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private int itemID;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private GameObject itemGameObject;
    [SerializeField] private Category category;

    // Getters and setters
    public int ItemID { get { return itemID; } set { itemID = value; } }
    public string ItemName { get { return itemName; } set { itemName = value; } }
    public Sprite ItemIcon { get { return itemIcon; } set { itemIcon = value; } }
    public GameObject ItemGameObject { get { return itemGameObject; } set { itemGameObject = value; } }
    public Category Category { get { return category; } set { category = value; } }
}
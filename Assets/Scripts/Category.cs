using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Category", menuName = "Scriptable Objects/Category")]
public class Category : ScriptableObject
{
    [SerializeField] string categoryName;
    [Tooltip("Optional")]
    [SerializeField] Sprite categoryIcon;
    [Tooltip("Optional")]
    [SerializeField] Color categoryColor;

    // Getters and setters
    public string CategoryName { get { return categoryName; } set { categoryName = value; } }
    public Sprite CategoryIcon { get { return categoryIcon; } set { categoryIcon = value; } }
    public Color CategoryColor { get { return categoryColor; } set { categoryColor = value; } }

    private void Awake()
    {
        if (categoryName == null || categoryName.Trim() == "")
        {
            Debug.LogWarning("Category: Category name is null or empty, all categories must have an unique name as ID.");
        }
    }
}

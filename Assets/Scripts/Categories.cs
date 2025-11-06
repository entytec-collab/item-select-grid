using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string id;
    public string itemName;
    public GameObject thumbnail;     
}

[System.Serializable]
public class CategoryData
{
    public string id;
    public string categoryName;
    public List<ItemData> items = new List<ItemData>();
}

[CreateAssetMenu(fileName = "Categories", menuName = "Scriptable Objects/Categories")]
public class Categories : ScriptableObject
{
    [SerializeField] List<CategoryData> categoriesList = new List<CategoryData>();

    public List<CategoryData> CategoriesList {  get { return categoriesList; } set { categoriesList = value; } }

}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;


[System.Serializable]
public class ItemData
{
    public string id;
    public string nombre;
    public Sprite thumbnail; // o referencia a prefab si usas 3D, cambia a GameObject si aplica
    public string metadataJson; // opcional: puedes estructurar mejor si lo necesitas
}

[System.Serializable]
public class CategoryData
{
    public string id;
    public string nombre;
    public ItemData[] items;
}


public class ItemCellView : MonoBehaviour
{
    [SerializeField] private Image thumbnailImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button button;

    private ItemData _data;

    public UnityEvent<ItemData> OnClick = new UnityEvent<ItemData>();

    public void Bind(ItemData data)
    {
        _data = data;
        if (thumbnailImage) thumbnailImage.sprite = data.thumbnail;
        if (nameText) nameText.text = data.nombre;
    }

    private void Awake()
    {
        if (button) button.onClick.AddListener(HandleClick);
    }

    private void OnDestroy()
    {
        if (button) button.onClick.RemoveListener(HandleClick);
    }

    private void HandleClick()
    {
        OnClick.Invoke(_data);
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;





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
        if (thumbnailImage) thumbnailImage.sprite = data.thumbnail.GetComponent<SpriteRenderer>().sprite;
        if (nameText) nameText.text = data.itemName;
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
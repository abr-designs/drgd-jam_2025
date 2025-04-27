using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Animations;

public class ItemUIElement : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private TMP_Text countText;

    [SerializeField]
    private TransformAnimator transformAnimator;

    public void SetItem(InventoryItemSO inventoryItemSo)
    {
        itemImage.sprite = inventoryItemSo.inventoryIconSprite;
    }

    public void SetCount(int count, bool animate)
    {
        countText.text = count.ToString();
        
        if(animate && transformAnimator)
            transformAnimator.Play();
    }
}

using System;
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

    private Vector3 m_startScale;
    private Quaternion m_startRotation;

    //Unity Functions
    //============================================================================================================//

    private void Start()
    {
        m_startScale = transform.localScale;
        m_startRotation = transform.localRotation;
    }

    private void OnDisable()
    {
        transformAnimator?.Stop();
        
        transform.localScale = m_startScale;
        transform.localRotation = m_startRotation;
    }

    //============================================================================================================//

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

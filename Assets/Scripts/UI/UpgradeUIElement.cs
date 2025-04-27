using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIElement : MonoBehaviour
{
    private static CraftingSystem s_craftingSystem;
    
    [SerializeField]
    private Button buyButton;
    [SerializeField]
    private TMP_Text titleText;
    [SerializeField]
    private TMP_Text descriptionText;
    [SerializeField]
    private ItemUIElement costUIElementPrefab;
    [SerializeField]
    private RectTransform elementContainerRectTransform;

    private bool m_initialized;

    private UpgradeRecipeSO m_craftingRecipeSo;
    private Action<UpgradeRecipeSO> m_onBuyPressed;
    private Dictionary<InventoryItemSO, ItemUIElement> m_itemUIElements;
    
    //============================================================================================================//

    public void Init(UpgradeRecipeSO craftingRecipeSo, Action<UpgradeRecipeSO> onBuyPressed)
    {
        if(m_initialized)
            return;

        gameObject.name = $"UpgradeUIElement_Instance_{craftingRecipeSo.name}";
        s_craftingSystem ??= FindAnyObjectByType<CraftingSystem>();
        
        m_onBuyPressed = onBuyPressed;
        buyButton.onClick.AddListener(() => m_onBuyPressed?.Invoke(m_craftingRecipeSo));

        UpdateUIElement(craftingRecipeSo);
        m_initialized = true;
    }

    public void UpdateUIElement(UpgradeRecipeSO craftingRecipeSo)
    {
        gameObject.SetActive(craftingRecipeSo != null);
        
        titleText.text = craftingRecipeSo.name;
        descriptionText.text = craftingRecipeSo.description;
        
        m_craftingRecipeSo = craftingRecipeSo;
        InitCostElements(craftingRecipeSo.inputItemStackList);
        buyButton.interactable = s_craftingSystem.CheckInventoryQuantityForRecipe(craftingRecipeSo);
    }

    private void InitCostElements(List<ItemStack> items)
    {
        m_itemUIElements ??= new Dictionary<InventoryItemSO, ItemUIElement>();

        //Set existing Inactive
        foreach (var itemUiElement in m_itemUIElements.Values)
        {
            itemUiElement.gameObject.SetActive(false);
        }

        
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (!m_itemUIElements.TryGetValue(item.itemSo, out var existingElement))
            {
                existingElement = Instantiate(costUIElementPrefab, elementContainerRectTransform, false);
                existingElement.SetItem(item.itemSo);
                
                m_itemUIElements.Add(item.itemSo, existingElement);

            }

            existingElement.gameObject.SetActive(true);
            existingElement.SetCount(item.quantity, false);
        }
        
        
    }
}

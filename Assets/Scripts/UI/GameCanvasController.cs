using System.Collections.Generic;
using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Animations;

public class GameCanvasController : HiddenSingleton<GameCanvasController>
{
    [Header("Text Elements")]
    [SerializeField]
    private TMP_Text dayText;
    [SerializeField]
    private TMP_Text timeText;
    [SerializeField]
    private TMP_Text alertText;

    [Header("Health Data")]
    [SerializeField]
    private Sprite hasHealthSprite;
    [SerializeField]
    private Sprite noHealthSprite;
    [SerializeField]
    private Image[] healthStack;
    [SerializeField]
    private TransformAnimator[] healthStackAnimators;

    [SerializeField, Header("Collectible Collection")]
    private ItemUIElement itemUIElementPrefab;
    [SerializeField]
    private RectTransform itemUIContainer;
    private Dictionary<InventoryItemSO, ItemUIElement> m_itemUIElements;
    
    //Unity Functions
    //============================================================================================================//

    private void OnEnable()
    {
        PlayerHealth.OnPlayerHealthChange += OnPlayerHealthChange;
        DayController.OnTimeLeftChanged += OnTimeLeftChanged;
        DayController.OnDayChanged += OnDayChanged;
        
        InventorySystem.OnNewItemStackAdded += OnNewItemStackAdded;
        InventorySystem.OnNewItemStackChanged += OnNewItemStackChanged;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        dayText.text = "";
        timeText.text = "";
        alertText.text = "";
    }
    
    private void OnDisable()
    {
        PlayerHealth.OnPlayerHealthChange -= OnPlayerHealthChange;
        DayController.OnTimeLeftChanged -= OnTimeLeftChanged;
        DayController.OnDayChanged -= OnDayChanged;
        
        InventorySystem.OnNewItemStackAdded -= OnNewItemStackAdded;
        InventorySystem.OnNewItemStackChanged -= OnNewItemStackChanged;
    }

    //Item Inventory
    //============================================================================================================//


    private void TryAddNewItemUIElement(InventoryItemSO inventoryItem, int count)
    {
        m_itemUIElements ??= new Dictionary<InventoryItemSO, ItemUIElement>();

        if (!m_itemUIElements.TryGetValue(inventoryItem, out var existingElement))
        {
            var newInstance = Instantiate(itemUIElementPrefab, itemUIContainer, false);

            m_itemUIElements.Add(inventoryItem, newInstance);
            existingElement = newInstance;
        }
        
        
        existingElement.SetItem(inventoryItem);
        existingElement.SetCount(count, true);
    }

    private void TryUpdateItemUIElement(InventoryItemSO inventoryItem, int count)
    {
        if (!m_itemUIElements.TryGetValue(inventoryItem, out var existingElement))
            return;
        
        existingElement.SetCount(count, true);
    }
    
    
    //Callbacks
    //============================================================================================================//

    private void OnTimeLeftChanged(float seconds)
    {
        timeText.text = $"{seconds:#0.0}s";
    }
    
    private void OnDayChanged(int dayValue)
    {
        dayText.text = $"Day {dayValue}";
    }
    
    private void OnPlayerHealthChange(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < healthStack.Length; i++)
        {
            healthStack[i].gameObject.SetActive(maxHealth >= i);
            var targetSprite = currentHealth >= i ? hasHealthSprite : noHealthSprite;
            
            //If there was a change we should animate the element
            if (healthStack[i].sprite != targetSprite)
            {
                healthStackAnimators[i]?.Play();
            }
            
            healthStack[i].sprite = targetSprite;
        }
    }

    private void OnNewItemStackChanged(ItemStack itemStack) =>
        TryUpdateItemUIElement(itemStack.itemSo, itemStack.quantity);

    private void OnNewItemStackAdded(ItemStack itemStack)
    {
        TryAddNewItemUIElement(itemStack.itemSo, itemStack.quantity);
    }
    
}

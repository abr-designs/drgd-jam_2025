using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class UpgradeStoreController : HiddenSingleton<UpgradeStoreController>
    {
        public static event Action OnStoreClosed;

                
        [SerializeField, Header("Buttons")]
        private Button doneButton;
        
        [SerializeField, Header("Upgrade Elements")]
        private UpgradeUIElement upgradeUIElementPrefab;
        [SerializeField]
        private RectTransform upgradeElementContainTransform;

        [SerializeField]
        private UpgradeType[] upgrades;
        /*[SerializeField, Obsolete]
        private CraftingRecipeSO[] craftingRecipeSos;*/

        private Dictionary<UpgradeType, UpgradeUIElement> m_upgradeUIElements;
        private CraftingSystem m_craftingSystem;
        
        //Unity Functions
        //============================================================================================================//
        private void Start()
        {
            SetupButtons();
            m_craftingSystem = FindAnyObjectByType<CraftingSystem>();
        }

        //Static Functions
        //============================================================================================================//

        public static void ShowStore(bool state)
        {
            Instance?.gameObject.SetActive(state);

            if(state)
                Instance?.SetupStore();
        }

        private void SetupStore()
        {
            m_upgradeUIElements ??= new Dictionary<UpgradeType, UpgradeUIElement>();
            
            for ( int i = 0; i < upgrades.Length; i++)
            {
                var upgradeType = upgrades[i];
                
                //Get current level
                var currentLevel = UpgradeManager.GetUpgradeLevel(upgradeType);
                
                //TODO Get Recipe for level + 1
                var craftingRecipe = UpgradeManager.GetUpgradeRecipe(upgradeType, currentLevel + 1);

                if (!m_upgradeUIElements.TryGetValue(upgradeType, out var existingElement))
                {
                    existingElement = Instantiate(upgradeUIElementPrefab, upgradeElementContainTransform, false);
                    m_upgradeUIElements.Add(upgradeType, existingElement);
                    
                    existingElement.Init(craftingRecipe, OnBuyPressed);
                }
                else
                    existingElement.UpdateUIElement(craftingRecipe);
            }
            
        }

        private void OnBuyPressed(UpgradeRecipeSO craftingRecipeSo)
        {
            if (!m_craftingSystem.TryCraftingUpgrade(craftingRecipeSo))
                throw new Exception();

            SetupStore();
        }
        

        //Upgrade Store Functions
        //============================================================================================================//

        private void SetupButtons()
        {
            doneButton.onClick.AddListener(OnDonePressed);
        }
        
        //Callbacks
        //============================================================================================================//

        private void OnDonePressed()
        {
            OnStoreClosed?.Invoke();
            ShowStore(false);
            
        }
    }
}
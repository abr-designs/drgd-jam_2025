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
        private upgradeType[] upgrades;
        [SerializeField, Obsolete]
        private CraftingRecipeSO[] craftingRecipeSos;

        private Dictionary<upgradeType, UpgradeUIElement> m_upgradeUIElements;
        
        //Unity Functions
        //============================================================================================================//
        private void Start()
        {
            SetupButtons();
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
            m_upgradeUIElements ??= new Dictionary<upgradeType, UpgradeUIElement>();
            
            for ( int i = 0; i < upgrades.Length; i++)
            {
                var upgradeType = upgrades[i];
                //TODO Get current level
                //TODO Get Recipe for level + 1

                if (!m_upgradeUIElements.TryGetValue(upgradeType, out var existingElement))
                {
                    existingElement = Instantiate(upgradeUIElementPrefab, upgradeElementContainTransform, false);
                    m_upgradeUIElements.Add(upgradeType, existingElement);
                    
                    existingElement.Init(craftingRecipeSos[i], OnBuyPressed);
                }
                else
                    existingElement.UpdateUIElement(craftingRecipeSos[i]);
            }
            
        }

        private void OnBuyPressed(CraftingRecipeSO craftingRecipeSo)
        {
            //TODO Purchase Item
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
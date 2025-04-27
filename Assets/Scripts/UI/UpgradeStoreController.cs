using System;
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
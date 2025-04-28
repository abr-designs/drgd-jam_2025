using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using TMPro;

namespace UI
{
    public class DebtCanvasController : HiddenSingleton<DebtCanvasController>
    {
        public static event Action<bool, int> OnDebtWindowClose;


        [SerializeField, Header("Buttons")]
        private Button payButton;
        [SerializeField]
        private Button endButton;

        [SerializeField]
        private InventoryItemSO goldItem;

        private int _debtTotal;
        private int _debtInterest;
        private int _debtRepairs;

        [SerializeField, Header("Debt Elements")]
        private TMP_Text tmDebtTotal;
        [SerializeField]private TMP_Text tmDebtInterest;
        [SerializeField]private TMP_Text tmDebtRepairs;

        //Unity Functions
        //============================================================================================================//
        private void Start()
        {
            SetupButtons();
        }

        //Static Functions
        //============================================================================================================//

        public static void ShowDebt(bool state, int debt = 0, int interest = 0, int repairs = 0)
        {
            Instance?.gameObject.SetActive(state);

            Instance._debtTotal = debt;
            Instance._debtInterest = interest;
            Instance._debtRepairs = repairs;

            if (state)
                Instance?.SetupDebt();
        }

        private void SetupDebt()
        {
            // Check if we have enough to cover payment
            var totalGold = InventorySystem.Instance.GetCountOfItem(goldItem);

            // Disable pay option if we are unable to pay
            payButton.gameObject.SetActive(totalGold >= _debtInterest + _debtRepairs);

            tmDebtTotal.text = _debtTotal.ToString();
            tmDebtInterest.text = _debtInterest.ToString();
            tmDebtRepairs.text = _debtRepairs.ToString();
        }

        //Upgrade Store Functions
        //============================================================================================================//

        private void SetupButtons()
        {
            payButton.onClick.AddListener(OnPayPressed);
            endButton.onClick.AddListener(OnEndPressed);
        }

        //Callbacks
        //============================================================================================================//

        private void OnPayPressed()
        {
            var paid = _debtInterest;
            InventorySystem.Instance.RemoveItem(goldItem, paid);

            OnDebtWindowClose?.Invoke(true, paid);
            ShowDebt(false);

        }
        private void OnEndPressed()
        {
            OnDebtWindowClose?.Invoke(false, 0);
            ShowDebt(false);

        }
    }
}
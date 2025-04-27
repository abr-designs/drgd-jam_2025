using System;
using System.Collections.Generic;
using Interfaces;
using UnityEditor;
using UnityEngine;
using Utilities;

public class UpgradeManager : HiddenSingleton<UpgradeManager>
{
    public int CurrentHealth;
    public int CurrentRoll;
    public int CurrentCapacity;
    [Serializable]
    public class UpgradeData
    {
        public UpgradeType upgradeType;
        public float[] levelValues;
        public UpgradeRecipeSO[] levelRecipes;

    }
    //[Serializable]
    //public class UpgradeRecipeApplication
    //{
    //    public UpgradeType upgradetype;
    //    public int levelValues;

    //}
    public UpgradeData[] upgrades;
    private PlayerHealth playerHealth;
    private InventorySystem inventorySystem;

    [SerializeField] private List<UpgradeRecipeSO> upgradeRecipes;
    private Dictionary<UpgradeType, int> upgradeLevels;
    
    private void Start()
    {
        // find references
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        inventorySystem = FindAnyObjectByType<InventorySystem>();

        // set initial upgrade values
        InitializeUpgradeLevels();
    }

    private void InitializeUpgradeLevels()
    {
        // set initial values to zero
        upgradeLevels = new Dictionary<UpgradeType, int>();
        foreach (UpgradeType value in Enum.GetValues(typeof(UpgradeType)))
        {
            upgradeLevels.Add(value, 0);
        }

        // call apply the upgrade
        foreach (KeyValuePair<UpgradeType, int> pair in upgradeLevels)
        {
            TryApplyUpgrade(pair.Key, pair.Value);
        }
    }

    public int GetUpgradeLevel(UpgradeType upgradetype)
    {
        return upgradeLevels[upgradetype];
    }

    public UpgradeRecipeSO GetUpgradeRecipe(UpgradeType upgradeType, int level)
    {
        foreach(UpgradeRecipeSO upgradeRecipeSO in upgradeRecipes)
        {
            if(upgradeRecipeSO.outputUpgradeType == upgradeType
                && upgradeRecipeSO.level == level)
            {
                return upgradeRecipeSO;
            }
        }

        return null;
    }

    public static void TryApplyUpgrade(UpgradeType upgradeType, int level)
    {
        Instance.ApplyUpgrade(upgradeType, level);

    }
    private void ApplyUpgrade(UpgradeType upgradeType, int level)
    {
        UpgradeData targetUpgrade = FindUpgradeData(upgradeType);
        float multiplier = targetUpgrade.levelValues[level];
        switch (upgradeType)
        {
            case UpgradeType.ShieldUpgrade:
                playerHealth.ApplyUpgrade(multiplier);
                break;
            case UpgradeType.RollUpgrade:
                //playerRoll.ApplyUpgrade(multiplier);
                break;
            case UpgradeType.CapacityUpgrade:
                inventorySystem.ApplyUpgrade(multiplier);
                break;
        }
    }
    private UpgradeData FindUpgradeData(UpgradeType upgradeType)
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            if (upgrades[i].upgradeType == upgradeType)
            {
                return upgrades[i];
            }
        }
        return null;
    }
}

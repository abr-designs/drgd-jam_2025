using System;
using Interfaces;
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
        public upgradeType upgradetype;
        public float[] levelValues;

    }
    public UpgradeData[] upgrades;
    public PlayerHealth playerhealth;
    public InventorySystem inventorysystem;
    public static void applyUpgrade(upgradeType upgradeType, int level)
    {
        Instance.applyUpgrade2(upgradeType, level);

    }
    private void applyUpgrade2(upgradeType upgradeType, int level)
    {
        UpgradeData targetUpgrade = findUpgradeData(upgradeType);
        float multiplier = targetUpgrade.levelValues[level];
        switch (upgradeType)
        {
            case upgradeType.ShieldUpgrade:
                playerhealth.ApplyUpgrade(multiplier);
                break;
            case upgradeType.RollUpgrade:
                //playerRoll.ApplyUpgrade(multiplier);
                break;
            case upgradeType.CapacityUpgrade:
                inventorysystem.ApplyUpgrade(multiplier);
                break;
        }
    }
    private UpgradeData findUpgradeData(upgradeType upgradetype)
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            if (upgrades[i].upgradetype == upgradetype)
            {
                return upgrades[i];
            }
        }
        return null;
    }
}

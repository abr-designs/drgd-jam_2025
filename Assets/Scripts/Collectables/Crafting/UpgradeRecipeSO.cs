using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade Recipe", menuName = "Inventory/Upgrade Recipe")]
public class UpgradeRecipeSO : ScriptableObject {

    public new string name;
    public List<ItemStack> inputItemStackList;
    public UpgradeManager.UpgradeData outputUpgradeData;

}
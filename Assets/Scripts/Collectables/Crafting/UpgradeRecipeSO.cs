using Interfaces;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade Recipe", menuName = "Inventory/Upgrade Recipe")]
public class UpgradeRecipeSO : ScriptableObject {

    public new string name;
    [TextArea]
    public string description;
    public List<ItemStack> inputItemStackList;
    public UpgradeType outputUpgradeType;
    public int level;

}
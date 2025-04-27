using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Inventory/Crafting Recipe")]
public class CraftingRecipeSO : ScriptableObject {

    public new string name;
    [TextArea]
    public string description;
    public List<ItemStack> inputItemStackList;
    public List<ItemStack> outputItemStackList;

}
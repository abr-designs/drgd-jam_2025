using Audio;
using Audio.SoundFX;
using NaughtyAttributes;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    //InventorySystem inventorySystem;

    private void Start() {
        //inventorySystem = GetComponent<InventorySystem>();
    }

    public void TestCraftingSword(CraftingRecipeSO recipe) {
        TryCraftingRecipe(recipe);

        SFX.OBJECT_PICKUP.PlaySound();
    }

    private bool TryCraftingRecipe(CraftingRecipeSO recipe) {

        if (recipe == null)
            return false;

        if (!CheckInventoryQuantityForRecipe(recipe)) {
            Debug.LogWarning($"You do not have the required items to craft {recipe.recipeName}");
            return false;
        }

        // remove used items
        RemoveCraftingRecipeIngredients(recipe);

        // add new item to inventory
        AddCraftingRecipeOutput(recipe);

        return true;

    }

    private bool CheckInventoryQuantityForRecipe(CraftingRecipeSO recipe) {

        bool haveQuantity = true;

        foreach (ItemStack itemStack in recipe.inputItemStackList) {
            if (InventorySystem.Instance.GetCountOfItem(itemStack.itemSo) < itemStack.quantity) {
                haveQuantity = false;
                break;
            }
        }

        return haveQuantity;
    }

    private void RemoveCraftingRecipeIngredients(CraftingRecipeSO recipe) {

        foreach (ItemStack itemStack in recipe.inputItemStackList) {
            InventorySystem.Instance.RemoveItem(itemStack);
        }
    }

    private void AddCraftingRecipeOutput(CraftingRecipeSO recipe) {

        foreach (ItemStack itemStack in recipe.outputItemStackList) {
            InventorySystem.Instance.InsertItemStackToInventory(itemStack);
        }
    }

    //Unity Editor
    //============================================================================================================//

#if UNITY_EDITOR

    [SerializeField, Header("DEBUGGING")]
    private bool requireApplicationPlaying = true;
    [SerializeField] private CraftingRecipeSO debugCraftingRecipe;

    [Button]
    private void CraftGold()
    {
        if (requireApplicationPlaying && Application.isPlaying == false)
            return;

        Debug.Log("Craft Debug Recipe");
        TryCraftingRecipe(debugCraftingRecipe);
    }

#endif
    //============================================================================================================//
}
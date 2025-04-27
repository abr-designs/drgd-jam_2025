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
            Debug.LogWarning($"You do not have the required items to craft {recipe.name}");
            return false;
        }

        // remove used items
        RemoveCraftingRecipeIngredients(recipe);

        // add new item to inventory
        AddCraftingRecipeOutput(recipe);

        SFX.CLICK_UI_BUTTON.PlaySound();

        Debug.Log($"Crafted {recipe.name}");

        return true;

    }

    public bool TryCraftingUpgrade(UpgradeRecipeSO recipe)
    {
        if (recipe == null)
            return false;

        if (!CheckInventoryQuantityForRecipe(recipe))
        {
            Debug.LogWarning($"You do not have the required items to craft {recipe.name}");
            return false;
        }

        // remove used items
        RemoveCraftingRecipeIngredients(recipe);

        // produce upgrade
        UpgradeManager.TryApplyUpgrade(recipe.outputUpgradeType, recipe.level);

        SFX.CLICK_UI_BUTTON.PlaySound();

        Debug.Log($"Consumed resources for upgrade {recipe.name}");

        return true;
    }

    private bool CheckInventoryQuantityForRecipe(CraftingRecipeSO recipe)
    {

        bool haveQuantity = true;

        foreach (ItemStack itemStack in recipe.inputItemStackList)
        {
            if (InventorySystem.Instance.GetCountOfItem(itemStack.itemSo) < itemStack.quantity)
            {
                haveQuantity = false;
                break;
            }
        }

        return haveQuantity;
    }

    public bool CheckInventoryQuantityForRecipe(UpgradeRecipeSO recipe)
    {

        bool haveQuantity = true;

        foreach (ItemStack itemStack in recipe.inputItemStackList)
        {
            if (InventorySystem.Instance.GetCountOfItem(itemStack.itemSo) < itemStack.quantity)
            {
                haveQuantity = false;
                break;
            }
        }

        return haveQuantity;
    }

    private void RemoveCraftingRecipeIngredients(CraftingRecipeSO recipe)
    {
        foreach (ItemStack itemStack in recipe.inputItemStackList)
        {
            InventorySystem.Instance.RemoveItem(itemStack);
        }
    }

    private void RemoveCraftingRecipeIngredients(UpgradeRecipeSO recipe)
    {

        foreach (ItemStack itemStack in recipe.inputItemStackList)
        {
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
    [SerializeField] private UpgradeRecipeSO debugUpgradeRecipe;

    [Button]
    private void CraftUpgradeRecipe()
    {
        if (requireApplicationPlaying && Application.isPlaying == false)
        {
            Debug.LogError("Application must be playing");
            return;
        }

        TryCraftingUpgrade(debugUpgradeRecipe);
    }

#endif
    //============================================================================================================//
}
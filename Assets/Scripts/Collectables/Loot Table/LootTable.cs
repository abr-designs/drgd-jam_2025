using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public struct LootTable
{
    public List<LootItem> lootItemList;

    public void CalculateDropRates() {

        // identify all weight in the loot table
        int totalWeight = 0;
        foreach (LootItem lootItem in lootItemList) {
            totalWeight += lootItem.weight;
        }
        if (totalWeight < 1) return;

        foreach (LootItem lootItem in lootItemList) {
            lootItem.dropRate = (float)lootItem.weight / (float)totalWeight;
        }

    }

    public ItemStack GetRandomLootItem() {

        // identify all weight in the loot table
        int totalWeight = 0;
        foreach (LootItem lootItem in lootItemList) {
            totalWeight += lootItem.weight;
        }

        // select random weight value
        int randomSelectedWeight = UnityEngine.Random.Range(0, totalWeight);

        // loop through loot list again stopping when deducting item weight from selected random weight reaches zero
        LootItem selectedLootItem = null;
        foreach (LootItem lootItem in lootItemList) {
            randomSelectedWeight -= lootItem.weight;
            if (randomSelectedWeight < 0) {
                selectedLootItem = lootItem;
                break;
            }
        }

        ItemStack itemStack = null;
        if (selectedLootItem != null && selectedLootItem.itemType != null)
        {
            itemStack = new ItemStack(selectedLootItem.itemType, selectedLootItem.quantity);
        }

        return itemStack;
    }

}


[Serializable]
public class LootItem {

    public InventoryItemSO itemType;
    [SerializeField] public int quantity;
    [SerializeField] public int weight;
    //[ShowOnly][SerializeField] public float dropRate;
    [SerializeField] public float dropRate;

}


[CustomPropertyDrawer(typeof(LootItem))]
public class LootItemDrawerUIE : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        EditorGUI.PropertyField(position, property, label, true);

    }

    //GetPropertyHeight override
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
        EditorGUI.GetPropertyHeight(property, label, true);// + 10;

}
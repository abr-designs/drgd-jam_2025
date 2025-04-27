using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Level/Tile Type")]
public class TileTypeSO : ScriptableObject
{   
    public new string name;
    public Mesh mesh;
    public LootTable lootTable;

    private void CalculateProbability()
    {
        // identify all weight in the loot table
        int totalWeight = 0;
        foreach (LootItem lootItem in lootTable.lootItemList)
        {
            totalWeight += lootItem.weight;
        }
        if (totalWeight < 1) return;

        foreach (LootItem lootItem in lootTable.lootItemList)
        {
            lootItem.dropRate = (float)lootItem.weight / (float)totalWeight;
        }
    }

    //Editor Functions
    //============================================================================================================//

#if UNITY_EDITOR
    [Button]
    private void EvaluateProbability()
    {
        CalculateProbability();
    }
#endif
}
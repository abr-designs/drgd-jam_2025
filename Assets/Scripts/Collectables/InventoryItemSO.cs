using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Inventory/Inventory Item")]
public class InventoryItemSO : ScriptableObject {
    
    public new string name;
    public int maxHoldCount;
    public Sprite inventoryIconSprite;
    public Transform collectablePrefab;

}
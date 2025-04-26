//using System;
//using UnityEngine;

//[Serializable]
//public class InventoryItem : IEquatable<InventoryItem> {

//    [SerializeField]
//    private InventoryItemSO inventoryItemSO;

//    public Transform collectablePrefab;

//    public InventoryItem(InventoryItemSO inventoryItemSO) {
//        this.inventoryItemSO = inventoryItemSO;

//        // sprite
//        InventoryIconSprite = inventoryItemSO.inventoryIconSprite;
//        this.collectablePrefab = inventoryItemSO.collectablePrefab;
//    }

//    public InventoryItemSO InventoryItemSO {
//        get => inventoryItemSO;
//    }

//    public Sprite InventoryIconSprite { get; protected set; }

//    // this equals functions checks if a passed object is the same as this object type
//    public bool Equals(InventoryItem other)
//    {
//        return other.inventoryItemSO == inventoryItemSO;
//    }

//    public static bool operator ==(InventoryItem a, InventoryItem b)
//    {
//        // This creates funky results
//        if (object.ReferenceEquals(b, null))
//        {
//            return true;
//        }
//        return a.inventoryItemSO == b.inventoryItemSO;
//    }

//    public static bool operator !=(InventoryItem a, InventoryItem b)
//    {
//        return !(a == b);
//    }

//    public override string ToString()
//    {
//        return inventoryItemSO.name;
//    }
//}
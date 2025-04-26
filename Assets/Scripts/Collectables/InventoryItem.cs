using System;
using UnityEngine;

[Serializable]
public class InventoryItem : IEquatable<InventoryItem> {

    [SerializeField]
    private InventoryItemSO inventoryItemSO;

    public Transform collectablePrefab;

    public InventoryItem(InventoryItemSO inventoryItemSO) {
        this.inventoryItemSO = inventoryItemSO;

        // sprite
        InventoryIconSprite = inventoryItemSO.inventoryIconSprite;
        this.collectablePrefab = inventoryItemSO.collectablePrefab;
    }

    public InventoryItemSO InventoryItemSO {
        get => inventoryItemSO;
    }

    public Sprite InventoryIconSprite { get; protected set; }

    // this equals functions checks if a passed object is the same as this object type
    public virtual bool Equals(InventoryItem other) {
        return other.inventoryItemSO == this.inventoryItemSO;
    }

    public override string ToString()
    {
        return inventoryItemSO.name;
    }

}
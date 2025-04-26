using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemStack {
    public InventoryItem item;
    public int quantity;

    public ItemStack(InventoryItem item, int quantity) {
        this.item = item;
        this.quantity = quantity;
    }

    public override string ToString()
    {
        return $"{item}[{quantity}]";
    }
}

public class InventorySystem : MonoBehaviour
{
    private static InventorySystem _instance;
    private List<ItemStack> items = new List<ItemStack>();

    int currentSelectedIndex = 0;

    public static event Action<ItemStack> OnSelectNewItemStackAction;

    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
            return;
        }

        _instance = this;

    }

    public int GetCountOfItem(InventoryItem item) {

        int countOfItems = 0;

        for (int i = 0; i < items.Count; i += 1) {
            if (items[i].item == item) {
                //itemStackIndex = i;
                countOfItems += items[i].quantity;
                break;
            }
        }

        return countOfItems;
    }

    public void InsertItemStackToInventory(ItemStack newItemStack) {

        int itemStackIndex = -1;
        for (int i = 0; i < items.Count; i += 1) {
            if (items[i].item.Equals(newItemStack.item)) {
                // check current quantity of existing item stack
                if(items[i].quantity + newItemStack.quantity > items[i].item.InventoryItemSO.maxQuantity) {
                    itemStackIndex = -1;
                    break;
                }

                itemStackIndex = i;
                break;
            }
        }

        // increment quantity
        if(itemStackIndex > -1) {
            items[itemStackIndex].quantity += newItemStack.quantity;
        } else {
            // check if inventory is empty
            if (items.Count > 0) {
                // insert as the next item
                InsertNewItemStackToInventory(newItemStack, currentSelectedIndex + 1);
            } else {
                // inventory is empty, insert as first item
                InsertNewItemStackToInventoryAtEnd(newItemStack);
            }
        }

        Debug.Log(ToString());
    }

    public void InsertNewItemStackToInventory(ItemStack newItemStack, int index) {
        // insert at index
        items.Insert(index, newItemStack);
    }

    public static void TryInsertNewItemStackToInventoryAtEnd(ItemStack newItemStack) {
        _instance.InsertNewItemStackToInventoryAtEnd(newItemStack);
    }

    public void InsertNewItemStackToInventoryAtEnd(ItemStack newItemStack) {
        items.Add(newItemStack);
    }

    public void RemoveItem(ItemStack itemStack) {

        InventoryItem item = itemStack.item;
        int quantity = itemStack.quantity;

        RemoveItem(item, quantity);
    }

    public void RemoveItem(InventoryItem item, int quantity) {

        // check if item is in inventory
        int itemStackIndex = -1;
        for (int i = 0; i < items.Count; i += 1) {
            if (items[i].item == item) {
                itemStackIndex = i;
                break;
            }
        }

        // increment quantity
        if (itemStackIndex > -1) {
            items[itemStackIndex].quantity -= quantity;

            if(items[itemStackIndex].quantity <= 0) {
                // drop item from inventory
                items.RemoveAt(itemStackIndex);

                if(currentSelectedIndex >= items.Count) {
                    currentSelectedIndex = items.Count - 1;
                }
            }
        }
    }

    public static ItemStack GenerateItemStack(InventoryItem item, int quantity)
    {
        ItemStack newItemStack = new ItemStack(item, quantity);
        return newItemStack;
    }

    public override string ToString()
    {
        string str = $"Inventory System Contains: ";
        foreach(ItemStack itemStack in items)
        {
            str += itemStack.ToString();
        }

        return str;
    }
}
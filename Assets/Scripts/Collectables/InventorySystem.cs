using System;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

[Serializable]
public class ItemStack
{
    public InventoryItemSO itemSo;
    public int quantity;

    public ItemStack(InventoryItemSO itemSo, int quantity)
    {
        this.itemSo = itemSo;
        this.quantity = quantity;
    }

    public override string ToString()
    {
        return $"{itemSo.name}[{quantity}]";
    }
}

public class InventorySystem : MonoBehaviour, IHaveUpgrade
{
    private static InventorySystem _instance;
    public static InventorySystem Instance { get { return _instance; } }

    public float multiplier { get; private set; }

    private List<ItemStack> items = new List<ItemStack>();
    int maxDistinctItems = 3;

    int currentSelectedIndex = 0;

    public static event Action<ItemStack> OnSelectNewItemStackAction;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

    }

    public int GetCountOfItem(InventoryItemSO itemSo)
    {

        int countOfItems = 0;

        for (int i = 0; i < items.Count; i += 1)
        {
            if (items[i].itemSo == itemSo)
            {
                //itemStackIndex = i;
                countOfItems += items[i].quantity;
                break;
            }
        }

        return countOfItems;
    }

    public void InsertItemStackToInventory(ItemStack newItemStack)
    {
        if (items.Count > maxDistinctItems)
            return;
        int itemStackIndex = -1;
        for (int i = 0; i < items.Count; i += 1)
        {
            if (items[i].itemSo == newItemStack.itemSo)
            {
                // check current quantity of existing item stack
                if (items[i].quantity + newItemStack.quantity > items[i].itemSo.maxHoldCount)
                {
                    itemStackIndex = -1;
                    break;
                }

                itemStackIndex = i;
                break;
            }
        }

        // increment quantity
        if (itemStackIndex > -1)
        {
            items[itemStackIndex].quantity += newItemStack.quantity;
        }
        else
        {
            // check if inventory is empty
            if (items.Count > 0)
            {
                // insert as the next item
                InsertNewItemStackToInventory(newItemStack, currentSelectedIndex + 1);
            }
            else
            {
                // inventory is empty, insert as first item
                InsertNewItemStackToInventoryAtEnd(newItemStack);
            }
        }

        //Debug.Log(ToString());
    }

    public void InsertNewItemStackToInventory(ItemStack newItemStack, int index)
    {
        // insert at index
        items.Insert(index, newItemStack);
    }

    public static void TryInsertNewItemStackToInventoryAtEnd(ItemStack newItemStack)
    {
        _instance.InsertNewItemStackToInventoryAtEnd(newItemStack);
    }

    public void InsertNewItemStackToInventoryAtEnd(ItemStack newItemStack)
    {
        items.Add(newItemStack);
    }

    public void RemoveItem(ItemStack itemStack)
    {

        InventoryItemSO itemSo = itemStack.itemSo;
        int quantity = itemStack.quantity;

        RemoveItem(itemSo, quantity);
    }

    public void RemoveItem(InventoryItemSO itemSo, int quantity)
    {

        // check if item is in inventory
        int itemStackIndex = -1;
        for (int i = 0; i < items.Count; i += 1)
        {
            if (items[i].itemSo == itemSo)
            {
                itemStackIndex = i;
                break;
            }
        }

        // increment quantity
        if (itemStackIndex > -1)
        {
            items[itemStackIndex].quantity -= quantity;

            if (items[itemStackIndex].quantity <= 0)
            {
                // drop item from inventory
                items.RemoveAt(itemStackIndex);

                if (currentSelectedIndex >= items.Count)
                {
                    currentSelectedIndex = items.Count - 1;
                }
            }
        }
    }

    public static ItemStack GenerateItemStack(InventoryItemSO itemSo, int quantity)
    {
        ItemStack newItemStack = new ItemStack(itemSo, quantity);
        return newItemStack;
    }

    public override string ToString()
    {
        string str = $"Inventory Contains: ";

        for (int i = 0; i < items.Count; i += 1)
        {
            str += items[i].ToString();

            if (i < items.Count - 1) str += ", ";
        }

        return str;
    }

    public void ApplyUpgrade(float newMultiplier)
    {
        multiplier = newMultiplier;
        maxDistinctItems = 3 + (int)multiplier;
    }
}
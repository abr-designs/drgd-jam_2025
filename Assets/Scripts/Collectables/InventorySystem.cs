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
    public static event Action<ItemStack> OnSelectNewItemStackAction;
    public static event Action<ItemStack> OnNewItemStackAdded;
    public static event Action<ItemStack> OnNewItemStackChanged;
    
    
    private static InventorySystem _instance;
    public static InventorySystem Instance { get { return _instance; } }

    public float upgradeAdditiveCapacity { get; private set; }

    private List<ItemStack> items = new List<ItemStack>();
    public List<ItemStack> GetItems {  get { return items; } }

    [SerializeField] private int maxDistinctItems = 3;

    int currentSelectedIndex = 0;


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

    public bool TryCheckHaveInventorySpace(ItemStack newItemStack)
    {
        if (items.Count > maxDistinctItems)
            return false;

        for (int i = 0; i < items.Count; i += 1)
        {
            if (items[i].itemSo == newItemStack.itemSo)
            {
                // check current quantity of existing item stack
                if (items[i].quantity + newItemStack.quantity > (items[i].itemSo.maxHoldCount + upgradeAdditiveCapacity))
                {
                    return false;
                }
            }
        }

        return true;
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
                //// check current quantity of existing item stack
                //if (items[i].quantity + newItemStack.quantity > items[i].itemSo.maxHoldCount)
                //{
                //    itemStackIndex = -1;
                //    break;
                //}

                itemStackIndex = i;
                break;
            }
        }

        // increment quantity
        if (itemStackIndex > -1)
        {
            items[itemStackIndex].quantity += newItemStack.quantity;
            
            OnNewItemStackChanged?.Invoke(items[itemStackIndex]);
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
            
            OnNewItemStackAdded?.Invoke(newItemStack);
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

            OnNewItemStackChanged?.Invoke(items[itemStackIndex]);
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
        upgradeAdditiveCapacity = newMultiplier;
        maxDistinctItems = 3 + (int)upgradeAdditiveCapacity; // starting value is 3

        //Debug.Log($"Increased inventory capacity bonus quantity to {upgradeAdditiveCapacity}");
    }
}
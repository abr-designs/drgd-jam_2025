using System;
using GGJ.Collectables;
using Interfaces;
using NaughtyAttributes;
using UnityEngine;
using VisualFX;

public class DestructibleTile : MonoBehaviour, IHaveHealth, IHaveLootTable
{
    public static event Action<int> OnYLevelChanged;
    
    public int Health { get; private set; }
    public int StartingHealth => spawnHealth;

    public LootTable LootTable => lootTable;
    [SerializeField]
    private LootTable lootTable;

    [SerializeField] 
    private int spawnHealth = 1;

    [SerializeField]
    private VFX onDestroyVFX;

    //Unity Functions
    //============================================================================================================//

    private void Awake()
    {
        Health = spawnHealth;
    }

    //DestructableTile Functions
    //============================================================================================================//
    
    public void ApplyDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = spawnHealth;
            bool spawnLoot = true;
            DestroyTile(spawnLoot);
        }
    }

    private void DestroyTile(bool spawnLoot = false)
    {
        if (spawnLoot)
        { 
            SpawnCollectable();
        }

        var previousPosition = transform.position;
        // move position down by tile scale
        transform.position = previousPosition + (Vector3.down * LevelController.TileSize);

        // rotate block to shake up monotony
        //RandomizeTileRotation();

        if (onDestroyVFX != VFX.NONE)
            onDestroyVFX.PlayAtLocation(previousPosition);

        // broadcast
        OnYLevelChanged?.Invoke((int)transform.position.y);
        
        TilesController.CheckShouldAdjustNeighors(transform);
    }

    private void SpawnCollectable()
    {
        //CollectableController.TryCreateCollectable(lootTable, transform.position, 1);
        CollectableController.TryCreateCollectable(lootTable, transform.position);
    }

    private void RandomizeTileRotation()
    {
        int randomRotationFactor = UnityEngine.Random.Range(0, 4);
        float rotation = randomRotationFactor * 90f;
        //transform.Rotate(Vector3.up * rotation);
        transform.rotation = Quaternion.Euler(0f, rotation, 0f);
    }


    public void CalculateDropRates()
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
    private void DamageTest()
    {
        ApplyDamage(1);
    }

    /*private void OnMouseDown()
    {
        if (!Application.isPlaying)
            return;
        
        var explosionHitColliders = new Collider[10];
        // Get affected tiles
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, 2f, explosionHitColliders);

        var cutOffY = transform.position.y - ((LevelController.TileSize / 2f) + 0.05f);
        Draw.Circle(transform.position,Vector3.up, Color.green, 2f);

        for (int i = 0; i < hitCount; i++)
        {
            var tile = explosionHitColliders[i].GetComponent<DestructibleTile>();
            if (tile != null && tile.transform.position.y >= cutOffY)
            {
                Draw.Circle(tile.transform.position, Vector3.up,  Color.pink, LevelController.TileSize);
                // Debug.Log($"Tile damage {Damage}");
                // Debug.Break();
                tile.ApplyDamage(1);
            }

        }
    }*/

    [Button]
    private void EvaluateLootTable()
    {
        CalculateDropRates();
    }

#endif
}

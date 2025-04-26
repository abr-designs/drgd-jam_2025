using System;
using GGJ.Collectables;
using Interfaces;
using NaughtyAttributes;
using UnityEngine;
using VisualFX;

public class DestructibleTile : MonoBehaviour, IHaveHealth
{
    public static event Action<int> OnYLevelChanged;
    
    public int Health { get; private set; }
    public int StartingHealth => spawnHealth;
    
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
        RandomizeTileRotation();

        if (onDestroyVFX != VFX.NONE)
            onDestroyVFX.PlayAtLocation(previousPosition);

        // broadcast
        OnYLevelChanged?.Invoke((int)transform.position.y);
    }

    private void SpawnCollectable()
    {
        Debug.Log("Spawn Collectable");
        CollectableController.CreateCollectable(transform.position, 1);
    }

    private void RandomizeTileRotation()
    {
        int randomRotationFactor = UnityEngine.Random.Range(0, 4);
        float rotation = randomRotationFactor * 90f;
        //transform.Rotate(Vector3.up * rotation);
        transform.rotation = Quaternion.Euler(0f, rotation, 0f);
    }

    //Editor Functions
    //============================================================================================================//

#if UNITY_EDITOR

    [Button]
    private void DamageTest()
    {
        ApplyDamage(1);
    }
    
#endif
}

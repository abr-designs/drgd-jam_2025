using System;
using Interfaces;
using NaughtyAttributes;
using Unity.Profiling;
using UnityEngine;
using Utilities.Debugging;
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
            DestroyTile();
        }
    }

    static readonly ProfilerMarker s_PreparePerfMarker = new ProfilerMarker("DestructibleTile.CheckShouldAdjustNeighors");
    private void DestroyTile()
    {
        var previousPosition = transform.position;
        // move position down by tile scale
        transform.position = previousPosition + (Vector3.down * LevelController.TileSize);

        // rotate block to shake up monotony
        RandomizeTileRotation();

        if (onDestroyVFX != VFX.NONE)
            onDestroyVFX.PlayAtLocation(previousPosition);

        // broadcast
        OnYLevelChanged?.Invoke((int)transform.position.y);
        
        s_PreparePerfMarker.Begin();
        TilesController.CheckShouldAdjustNeighors(transform);
        s_PreparePerfMarker.End();
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

    private void OnMouseDown()
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
    }

#endif
}

using System;
using UnityEngine;

public class DestructableTile : MonoBehaviour
{
    public float tileScale = 2.0f;
    [SerializeField] private float spawnHealth = 1.0f;
    public float Health { get; private set; }
    public static event Action<int> OnYLevelChanged;

    private void Awake()
    {
        Health = spawnHealth;
    }

    public void ApplyDamage(float damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            DestroyTile();
        }
    }

    public void DestroyTile()
    {
        // move position down by tile scale
        transform.position = transform.position + Vector3.down * tileScale;

        // rotate block to shake up monotony
        RandomizeTileRotation();

        // broadcast
        OnYLevelChanged?.Invoke((int)transform.position.y);
    }

    private void RandomizeTileRotation()
    {
        int randomRotationFactor = UnityEngine.Random.Range(0, 4);
        float rotation = (float)randomRotationFactor * 90f;
        //transform.Rotate(Vector3.up * rotation);
        transform.rotation = Quaternion.Euler(0f, rotation, 0f);
    }
}

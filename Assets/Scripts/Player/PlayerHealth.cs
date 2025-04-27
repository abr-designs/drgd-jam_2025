using System;
using Audio;
using Audio.SoundFX;
using Interfaces;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHaveHealth, IHaveUpgrade
{
    public static event Action OnPlayerDied;
    public static event Action<int, int> OnPlayerHealthChange;

    public int Health { get; private set; }
    public int StartingHealth => spawnHealth + (int)multiplier;

    public float multiplier { get; private set; }

    [SerializeField]
    private int spawnHealth = 3;

    //Unity Functions
    //============================================================================================================//

    private void Awake()
    {
        Health = spawnHealth;
        OnPlayerHealthChange?.Invoke(Health, StartingHealth);
    }

    //PlayerHealth Functions
    //============================================================================================================//

    public void ApplyDamage(int damage)
    {
        Health -= damage;
        OnPlayerHealthChange?.Invoke(Health, StartingHealth);
        if (Health <= 0)
        {
            Health = StartingHealth;
            Debug.Log("Player DIED!");

            SFX.PLAYER_DIED.PlaySound();

            OnPlayerDied?.Invoke();
        }
        else
        {
            SFX.PLAYER_HIT.PlaySound();
        }
    }

    public void ApplyUpgrade(float newMultiplier)
    {
        multiplier = newMultiplier;
    }
}

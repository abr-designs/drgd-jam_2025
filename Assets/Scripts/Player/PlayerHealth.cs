using System;
using Interfaces;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHaveHealth
{
    public static event Action OnPlayerDied;
    public static event Action<int,int> OnPlayerHealthChange;

    public int Health { get; private set; }
    public int StartingHealth => spawnHealth;

    [SerializeField]
    private int spawnHealth = 3;

    //Unity Functions
    //============================================================================================================//

    private void Awake()
    {
        Health = spawnHealth;
    }

    //PlayerHealth Functions
    //============================================================================================================//

    public void ApplyDamage(int damage)
    {
        Health -= damage;
        OnPlayerHealthChange?.Invoke(Health,StartingHealth);
        if (Health <= 0)
        {
            Health = StartingHealth;
            Debug.Log("Player DIED!");
            OnPlayerDied?.Invoke();
        }
    }



}

using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Thrower : MonoBehaviour
{
    private static readonly int THROW_HASH = Animator.StringToHash("Throw");

    [SerializeField]
    private Animator animator;

    private void OnEnable()
    {
        DynamiteManager.OnStopThrowing += OnStopThrowing;
    }

    private void OnDisable()
    {
        DynamiteManager.OnStopThrowing -= OnStopThrowing;
    }

    //============================================================================================================//
    public void Throw()
    {
        animator.Play("Throw");
        animator.speed = Random.Range(1f, 1.5f);
    }
    
    private void OnStopThrowing()
    {
        gameObject.SetActive(false);
    }


}

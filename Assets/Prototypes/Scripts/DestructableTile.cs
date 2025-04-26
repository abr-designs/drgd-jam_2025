using System;
using UnityEngine;

public class DestructableTile : MonoBehaviour
{
    public static event Action<int> OnYLevelChanged;
    private int yValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) 
            return;
        yValue -= 2;
        OnYLevelChanged?.Invoke(yValue);
    }
}

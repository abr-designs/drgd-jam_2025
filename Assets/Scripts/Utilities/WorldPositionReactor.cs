using System;
using UnityEngine;
using UnityEngine.Serialization;

public class WorldPositionReactor : MonoBehaviour
{
    [SerializeField]
    private float offsetFromYPosition;

    //Unity Functions
    //============================================================================================================//
    private void OnEnable()
    {
        LevelController.OnWorldYChanged += OnWorldYChanged;
    }

    private void OnDisable()
    {
        LevelController.OnWorldYChanged -= OnWorldYChanged;
    }

    //============================================================================================================//

    private void OnWorldYChanged(int newYLevel)
    {
        var temp = transform.position;
        temp.y = newYLevel + offsetFromYPosition;
        transform.position = temp;
    }
}

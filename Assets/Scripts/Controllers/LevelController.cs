using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class LevelController : HiddenSingleton<LevelController>
{
    public static event Action<int> OnWorldYChanged; 
    
    public static int TileSize => Instance.tileSize;
    [SerializeField, Min(1)]
    private int tileSize;

    public static int CurrentYLevel => Instance.m_currentYLevel;
    [SerializeField]
    private int startingYLevel;
    private int m_currentYLevel;
    
    [SerializeField, Header("Level Wall Spawner")]
    private Transform levelStackPrefab;
    private List<Transform> m_trackedLayerStacks;

    //Unity Functions
    //============================================================================================================//
    private void OnEnable()
    {
        DestructibleTile.OnYLevelChanged += OnYLevelChanged;
    }

    private void Start()
    {
        m_trackedLayerStacks = new List<Transform>();
        m_currentYLevel = startingYLevel;
        OnWorldYChanged?.Invoke(m_currentYLevel);
    }

    //For Debugging only
    /*private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            OnYLevelChanged(m_currentYLevel - 2);
    }*/

    private void OnDisable()
    {
        DestructibleTile.OnYLevelChanged -= OnYLevelChanged;
    }
    
    
    //============================================================================================================//
    
    private void OnYLevelChanged(int newYLevel)
    {
        if (newYLevel >= m_currentYLevel)
            return;

        //We won't react to non 
        if (Mathf.Abs(newYLevel - m_currentYLevel) != tileSize)
            return;

        m_currentYLevel = newYLevel;
        CreateNewLayerStack(m_currentYLevel);
        OnWorldYChanged?.Invoke(m_currentYLevel);
    }

    private void CreateNewLayerStack(int targetYPosition)
    {
        var newLayerStack = Instantiate(
            levelStackPrefab, 
            new Vector3(0, targetYPosition, 0), 
            Quaternion.identity,
            transform);
        newLayerStack.gameObject.name = $"{levelStackPrefab}-instance_[{m_trackedLayerStacks.Count}]";
        m_trackedLayerStacks.Add(newLayerStack);

        StackedWallLayer stackedWallLayer = newLayerStack.GetComponent<StackedWallLayer>();
        stackedWallLayer.EvaluateTileDepth();
    }
    
}

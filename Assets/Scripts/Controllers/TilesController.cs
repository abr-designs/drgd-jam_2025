using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using Utilities;
using Utilities.Debugging;
using Debug = UnityEngine.Debug;

/// <summary>
/// Used to prevent vertical gaps in tiles
/// </summary>
public class TilesController : HiddenSingleton<TilesController>
{
    private static int s_GridX = -1;
    private static int s_GridY = -1;
    private static int s_GridSize = -1;
    
    private static readonly Vector2Int s_N = new Vector2Int(1, 0);
    private static readonly Vector2Int s_E = new Vector2Int(0, 1);
    private static readonly Vector2Int s_S = new Vector2Int(-1, 0);
    private static readonly Vector2Int s_W = new Vector2Int(0, -1);
    
    private static readonly Vector2Int s_NE = new Vector2Int(1, 1);
    private static readonly Vector2Int s_SE = new Vector2Int(-1, 1);
    private static readonly Vector2Int s_SW = new Vector2Int(-1, -1);
    private static readonly Vector2Int s_NW = new Vector2Int(1, -1);
    
    
    [SerializeField]
    private Vector2Int gridSize;
    
    [SerializeField]
    private Transform[] allTiles;

    private Dictionary<Transform, int> m_transformIndicies;

    [SerializeField]
    private bool debugLogs;

    //Unity Functions
    //============================================================================================================//
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Assert.IsTrue(gridSize.x * gridSize.y == allTiles.Length, "Grid size does not match number of tiles!!");

        s_GridX = gridSize.x;
        s_GridY = gridSize.y;
        s_GridSize = s_GridY * s_GridX;

        m_transformIndicies = new Dictionary<Transform, int>(s_GridSize);
        for (int i = 0; i < s_GridSize; i++)
        {
            m_transformIndicies.Add(allTiles[i], i);
        }
    }
    
    //============================================================================================================//

    public static void CheckShouldAdjustNeighors(int tileIndex)
    {
        CheckShouldAdjustNeighors(Instance.allTiles[tileIndex]);
    }
    public static void CheckShouldAdjustNeighors(Transform tile)
    {
        if (!Instance.CheckNeighbors(tile, out var toAdjust))
            return;

        var newYPosition = (int)tile.transform.position.y + LevelController.TileSize;
        Instance.AdjustHeight(toAdjust, newYPosition);

        for (int i = 0; i < toAdjust.Length; i++)
        {
            CheckShouldAdjustNeighors(toAdjust[i]);
        }
    }
    
    //This should be called after a shift
    private bool CheckNeighbors(Transform tile, out int[] indexesToAdjust)
    {
        indexesToAdjust = null;
        
        var tileYPosition = (int)tile.transform.position.y;
        var tileIndex = m_transformIndicies[tile];
        var tileCoordinate = GetCoordinateFromIndex(tileIndex);
        
        //TODO Turn this into a span
        var neighbors = new List<int>(8)
        {
            GetTileIndex(tileCoordinate + s_N),
            GetTileIndex(tileCoordinate + s_E),
            GetTileIndex(tileCoordinate + s_S),
            GetTileIndex(tileCoordinate + s_W),
            
            GetTileIndex(tileCoordinate + s_NE),
            GetTileIndex(tileCoordinate + s_SE),
            GetTileIndex(tileCoordinate + s_SW),
            GetTileIndex(tileCoordinate + s_NW),
        };

        DebugState(tile, neighbors);
        
        //------------------------------------------------//

        for (int i = 7; i >= 0; i--)
        {
            var tempIndex = neighbors[i];
            
            //If its not within the grid space, skip it
            if (tempIndex < 0 || tempIndex >= s_GridSize)
            {
                neighbors.RemoveAt(i);
                continue;
            }
            
            Assert.IsTrue(tempIndex >= 0 && tempIndex < s_GridSize, $"[{tempIndex}] Outside of Range!!!");
            
            var tempTile = allTiles[tempIndex];
            var yPos = (int)tempTile.position.y;
            
            //If it doesn't need to be adjusted, skip it
            if (Math.Abs(Math.Abs(yPos) - Math.Abs(tileYPosition)) > LevelController.TileSize) 
                continue;
            
            neighbors.RemoveAt(i);
        }

        if (neighbors.Count <= 0) 
            return false;
        
        indexesToAdjust = neighbors.ToArray();
        return true;
    }

    private void AdjustHeight(int[] indexesToAdjust, int newYPosition)
    {
        for (int i = 0; i < indexesToAdjust.Length; i++)
        {
            var tempIndex = indexesToAdjust[i];
            var tempTile = allTiles[tempIndex];
            var position = tempTile.position;

            position.y = newYPosition;

            tempTile.position = position;
        }
    }

    //============================================================================================================//

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetTileIndex(Vector2Int coordinate)
    {
        return GetTileIndex(coordinate.x, coordinate.y);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetTileIndex(int x, int y)
    {
        if (x < 0 || y < 0)
            return -1;
        
        if (x >= s_GridX || y >= s_GridY)
            return -1;
        
        return Math.Clamp((x * s_GridY) + y, 0, s_GridSize);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Transform GetTile(int index)
    {
        return allTiles[index];
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Vector2Int GetCoordinateFromIndex(int index)
    {
        var xPos = index / s_GridY;

        var yPos = index - (xPos * s_GridY);

        return new Vector2Int(xPos, yPos);
    }
    
    //Editor Functions
    //============================================================================================================//

    [Conditional("UNITY_EDITOR")]
    private void DebugState(Transform tile, List<int> neighbors)
    {
        if (!debugLogs)
            return;
        
        var tileYPosition = tile.position.y;
        //Debugging
        //------------------------------------------------//
        var debug = $"Checking around [{tile?.gameObject?.name}]:\n";
        for (int i = 0; i < 8; i++)
        {
            var tempIndex = neighbors[i];
            
            if (tempIndex < 0 || tempIndex >= s_GridSize)
                continue;
            
            try
            {
                var tempTile = allTiles[tempIndex];
                var yPos = (int)tempTile.position.y;

                if (Math.Abs(Math.Abs(yPos) - Math.Abs(tileYPosition)) <= LevelController.TileSize)
                    debug += $"- <color=red>[{tempIndex}]{tempTile?.gameObject?.name} @ {yPos}</color>\n";
                else 
                    debug += $"- [{tempIndex}]{tempTile?.gameObject?.name} @ {yPos}\n";

                var dir = tile.position - tempTile.position;
                Draw.Arrow(tempTile.position, dir.normalized * dir.magnitude, Color.blue, 0.1f);
            }
            catch (Exception e)
            {
                Debug.LogError($"Out of Range for [{tempIndex}]");
                throw;
            }

        }
        
        Debug.Log(debug);
    }
    
#if UNITY_EDITOR
    [Header("Debugging")]
    [SerializeField]
    private Vector2Int coordinateTest;
    [SerializeField, ReadOnly]
    private int foundIndex;
    [Button("Find Index")]
    private void Find()
    {
        Start();
        
        foundIndex = GetTileIndex(coordinateTest);
        Debug.Log($"Found [{foundIndex}]", GetTile(foundIndex));
    }

    
    [SerializeField, Space(10f)]
    private int testIndex;
    [SerializeField, ReadOnly]
    private Vector2Int foundCoordinate;
    [Button("Convert Index")]
    private void Convert()
    {
        Start();
        
        foundCoordinate = GetCoordinateFromIndex(testIndex);
        
        Debug.Log($"Found [{foundCoordinate.x}, {foundCoordinate.y}]", GetTile(testIndex));
    }
#endif
}

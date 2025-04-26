using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities;

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

        //Debugging
        //------------------------------------------------//
        var debug = $"Checking around [{tile?.gameObject?.name}]:\n";
        for (int i = 0; i < 8; i++)
        {
            if (neighbors[i] < 0)
                continue;
            
            var tempIndex = neighbors[i];
            var tempTile = allTiles[tempIndex];
            var yPos = (int)tempTile.position.y;

            debug += $"- [{tempIndex}]{tempTile?.gameObject?.name} @ {yPos}\n";
            Debug.DrawLine(tempTile.position, tile.position, Color.blue, 0.1f);
        }
        
        Debug.Log(debug);
        //------------------------------------------------//

        for (int i = 7; i >= 0; i--)
        {
            //If its not within the grid space, skip it
            if (neighbors[i] < 0)
            {
                neighbors.RemoveAt(i);
                continue;
            }

            var tempIndex = neighbors[i];
            
            Assert.IsTrue(tempIndex >= 0 && tempIndex < s_GridSize, $"[{tempIndex}] Outside of Range!!!");
            
            var tempTile = allTiles[tempIndex];
            var yPos = (int)tempTile.position.y;
            
            //If it doesn't need to be adjusted, skip it
            if (Math.Abs(Math.Abs(yPos) - Math.Abs(tileYPosition)) <= LevelController.TileSize)
            {
                neighbors.RemoveAt(i);
                continue;
            }
            
            Debug.Log($"<color=red>[{tempTile.gameObject.name}] is {yPos} && [{tile.gameObject.name}] is {tileYPosition}</color>");
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
        
        if (x > s_GridX || y > s_GridY)
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

using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class TileSpawnerTest : MonoBehaviour
{
    public int rowResolution = 4;
    public int depth = 8;
    public float tileScale = 2.0f;
    public List<GameObject> tileTypes;
    
#if UNITY_EDITOR
    public void EmptyTiles()
    {
        for (int i = transform.childCount - 1; i >= 0; i -= 1)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    [Button("Spawn Tile")]
    public void SpawnInitialTiles()
    {
        EmptyTiles();

        int randomTileIndex = 0;

        for (int y = 0; y < depth; y += 1)
        {
            randomTileIndex = Random.Range(0, tileTypes.Count);

            for (int x = 0; x < rowResolution; x += 1)
            {
                for (int z = 0; z < rowResolution; z += 1)
                {
                    var tile = UnityEditor.PrefabUtility.InstantiatePrefab(tileTypes[randomTileIndex], transform) as GameObject;
                    tile.name = $"{tileTypes[randomTileIndex].name}_[{x}, {z}]";
                    tile.transform.localPosition = new Vector3(x, -y, z) * tileScale;
                }
            }
        }
    }
#endif

}


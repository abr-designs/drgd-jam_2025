using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileSpawnerTest))]
public class TileSpawnerTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileSpawnerTest tileSpawnerTest = (TileSpawnerTest)target;

        if (GUILayout.Button("Spawn Tile"))
        {
            tileSpawnerTest.SpawnInitialTiles();
        }
    }
}
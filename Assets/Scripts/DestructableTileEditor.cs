using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DestructableTile))]
public class DestructableTileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DestructableTile destructableTile = (DestructableTile)target;

        if (GUILayout.Button("Damage Tile 1 Health"))
        {
            destructableTile.ApplyDamage(1);
        }
    }
}
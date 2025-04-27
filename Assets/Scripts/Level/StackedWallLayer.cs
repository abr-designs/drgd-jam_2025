using System.Collections.Generic;
using UnityEngine;

public class StackedWallLayer : MonoBehaviour
{
    public List<BackgroundTile> backgroundTiles;

    public void EvaluateTileDepth()
    {
        foreach (BackgroundTile backgroundTile in backgroundTiles)
        {
            backgroundTile.EvaluateTileDepth();
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileProbability
{
    public TileTypeSO tileTypeSo;
    public int weight = 1;
}

[Serializable]
public class DepthLevel
{
    public int depthStart;
    public List<TileProbability> tileProbabilityList;
}

public class TileDepthTypeController : MonoBehaviour
{
    public static TileDepthTypeController Instance;

    public TileTypeSO defaultTileTypeSo;
    public List<DepthLevel> depthLevelList;

    private void Awake()
    {
        Instance = this;
    }

    public TileTypeSO GetTileTypeForDepth(int i)
    {
        DepthLevel selectedDepthLevel = null;
        foreach (DepthLevel level in depthLevelList)
        {
            if(i < level.depthStart)
            {
                selectedDepthLevel = level;
            }
        }

        if (selectedDepthLevel != null)
        {
            return GetRandomTileProbability(selectedDepthLevel.tileProbabilityList);
        }

        return defaultTileTypeSo;
    }

    public TileTypeSO GetRandomTileProbability(List<TileProbability> tileProbabilityList)
    {
        // identify all weight in the list
        int totalWeight = 0;
        foreach (TileProbability tileProbability in tileProbabilityList)
        {
            totalWeight += tileProbability.weight;
        }

        // select random weight value
        int randomSelectedWeight = UnityEngine.Random.Range(0, totalWeight);

        // loop through list again stopping when deducting iteration weight from selected random weight reaches zero
        foreach (TileProbability tileProbability in tileProbabilityList)
        {
            randomSelectedWeight -= tileProbability.weight;
            if (randomSelectedWeight < 0)
            {
                if (tileProbability == null)
                    return null;

                return tileProbability.tileTypeSo;
            }
        }

        return null;
    }
}

using System;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public static event Action<int> OnYLevelChanged;

    private MeshFilter meshFilter;

    //Unity Functions
    //============================================================================================================//

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    //Tile Functions
    //============================================================================================================//
    public void EvaluateTileDepth()
    {
        int tileDepthValue = (int)transform.position.y;
        TileTypeSO newTileDepthType = TileDepthTypeController.Instance.GetTileTypeForDepth(tileDepthValue);
        meshFilter.mesh = newTileDepthType.mesh;
    }

    private void RandomizeTileRotation()
    {
        int randomRotationFactor = UnityEngine.Random.Range(0, 4);
        float rotation = randomRotationFactor * 90f;
        transform.rotation = Quaternion.Euler(0f, rotation, 0f);
    }

}
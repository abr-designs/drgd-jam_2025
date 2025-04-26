using Audio;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Debugging;

public class DynamiteManager : MonoBehaviour
{
    //Spawning
    //------------------------------------------------//
    [SerializeField, Header("Spawning")]
    private Rect SpawnRect;
    [SerializeField]
    private float SpawnRate = 0.5f;
    private float _spawnTimer = 0f;

    
    [SerializeField]
    private float GravityMultiplier = 1f;


    [SerializeField]
    private LayerMask LevelLayerMask;
    private RaycastHit[] _raycastHits = new RaycastHit[10];

    [SerializeField, Header("Prefabs")]
    private Dynamite dynamitePrefab;
    [SerializeField]
    private GameObject indicatorPrefab;

    private List<Dynamite> _activeDynamite;

    //Unity Functions
    //============================================================================================================//
    private void Start()
    {
        _activeDynamite = new List<Dynamite>();
        _spawnTimer = SpawnRate;
    }

    private void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if(_spawnTimer <= 0){
            SpawnNew();
            _spawnTimer = SpawnRate;
        }


    }

    //DynamiteManager Functions
    //============================================================================================================//
    private void SpawnNew()
    {
        
        // Get random coordinate
        Vector2 randomPoint = new Vector2(
            Random.Range(SpawnRect.xMin, SpawnRect.xMax),
            Random.Range(SpawnRect.yMin, SpawnRect.yMax)
        );

        // TODO -- start Raycast from a certain distance above average level height
        Vector3 worldPos = new Vector3(randomPoint.x, 10f, randomPoint.y);

        // Raycast to get ground target                        
        Ray ray = new Ray(worldPos, Vector3.down);
        int hitCount = Physics.RaycastNonAlloc(ray, _raycastHits, 100f);
        Debug.DrawRay(worldPos, Vector3.down * 100f, Color.yellow, 1f);

        // Not hitting is a problem
        Debug.Assert(hitCount > 0, "Dynamite raycast did not hit ground");

        Vector3 targetPoint = _raycastHits[0].point;
        Debug.DrawRay(worldPos, Vector3.down * _raycastHits[0].distance, Color.green, 1f);
        
        var dyn = Instantiate(dynamitePrefab, targetPoint + Vector3.up * 10f, Quaternion.identity, transform);
        var ind = Instantiate(indicatorPrefab, transform, true);
        dyn.Spawn(targetPoint, GravityMultiplier, ind, LevelLayerMask);
    }

    //Unity Editor Functions
    //============================================================================================================//

#if UNITY_EDITOR
    
    private void OnDrawGizmos()
    {
        Vector3 center = new Vector3(SpawnRect.x + SpawnRect.width/2f,0,SpawnRect.y + SpawnRect.height/2f);
        Vector3 size = new Vector3(SpawnRect.width,5f, SpawnRect.height);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center,size);
    }
    
#endif

}

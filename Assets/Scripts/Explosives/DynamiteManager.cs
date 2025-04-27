using Audio;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Utilities.Debugging;
using NaughtyAttributes;

public class DynamiteManager : HiddenSingleton<DynamiteManager>
{

    //Spawning
    //------------------------------------------------//
    [SerializeField, Header("Spawning")]
    private Rect SpawnRect;
    [SerializeField]
    private float SpawnRate = 0.5f;
    private float _spawnTimer = 0f;

    private bool _isSpawning = false;


    [SerializeField]
    private float GravityMultiplier = 1f;
    [SerializeField]
    private float ThrowTargetTime = 3f;


    [SerializeField]
    private LayerMask LevelLayerMask;
    private RaycastHit[] _raycastHits = new RaycastHit[10];

    [SerializeField, Header("Prefabs")]
    private Dynamite dynamitePrefab;
    [SerializeField]
    private GameObject indicatorPrefab;

    [SerializeField]
    private Transform[] dynamiteThrowers;

    //Unity Functions
    //============================================================================================================//
    private void Start()
    {
        _spawnTimer = SpawnRate;
    }

    private void Update()
    {
        if (!_isSpawning) return;

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0)
        {
            SpawnNew();
            _spawnTimer = SpawnRate;
        }


    }

    //DynamiteManager Functions
    //============================================================================================================//

    public static void StartSpawning()
    {
        DynamiteManager.Instance._isSpawning = true;
    }

    public static void StopSpawning()
    {
        // TODO -- maybe cleanup any active dynamite?
        DynamiteManager.Instance._isSpawning = false;
    }

    private void SpawnNew()
    {

        // Get random coordinate
        Vector2 randomPoint = new Vector2(
            Random.Range(SpawnRect.xMin, SpawnRect.xMax),
            Random.Range(SpawnRect.yMin, SpawnRect.yMax)
        );

        // TODO -- start Raycast from a certain distance above average level height
        Vector3 worldPos = new Vector3(randomPoint.x, transform.position.y + 10f, randomPoint.y);

        // Raycast to get ground target                        
        Ray ray = new Ray(worldPos, Vector3.down);
        int hitCount = Physics.RaycastNonAlloc(ray, _raycastHits, 50f);
        Debug.DrawRay(worldPos, Vector3.down * 50f, Color.yellow, 1f);

        // Not hitting is a problem
        Debug.Assert(hitCount > 0, "Dynamite raycast did not hit ground");

        Vector3 targetPoint = _raycastHits[0].point;
        Debug.DrawRay(worldPos, Vector3.down * _raycastHits[0].distance, Color.green, 1f);

        var dyn = Instantiate(dynamitePrefab);
        var ind = Instantiate(indicatorPrefab, transform, true);

        bool isThrowing = transform.position.y > LevelController.TileSize * -5f;

        if (isThrowing)
        {
            // Pick random thrower
            Debug.Assert(dynamiteThrowers.Length > 0, "Dynamite thrower positions missing!");
            var thrower = dynamiteThrowers.PickRandomElement();

            // Cap the thrower positions based on the current mine location
            Vector3 pos = thrower.position;
            float maxY = transform.position.y + LevelController.TileSize * 10f;
            pos.y = Mathf.Min(pos.y, maxY);
            dyn.transform.position = pos;

            dyn.Spawn(targetPoint, GravityMultiplier, ind, LevelLayerMask, ThrowTargetTime, Dynamite.DYNAMITE_BEHAVIOUR.Thrown);
        }
        else
        {
            float grav = Mathf.Abs(Physics.gravity.y) * GravityMultiplier;
            dyn.transform.position = targetPoint + Vector3.up * 0.5f * grav * Mathf.Pow(ThrowTargetTime*1.5f,2);

            dyn.Spawn(targetPoint, GravityMultiplier, ind, LevelLayerMask, ThrowTargetTime, Dynamite.DYNAMITE_BEHAVIOUR.Dropped);
        }

    }

    //Unity Editor Functions
    //============================================================================================================//

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Vector3 center = new Vector3(SpawnRect.x + SpawnRect.width / 2f, transform.position.y, SpawnRect.y + SpawnRect.height / 2f);
        Vector3 size = new Vector3(SpawnRect.width, 8f, SpawnRect.height);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, size);
    }

    [Button]
    private void Run()
    {
        DynamiteManager.StartSpawning();
    }


#endif

}

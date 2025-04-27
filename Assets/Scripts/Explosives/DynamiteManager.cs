using System;
using UnityEngine;
using Utilities;
using NaughtyAttributes;
using Random = UnityEngine.Random;
using Utilities.Recycling;
using Controllers;

public class DynamiteManager : HiddenSingleton<DynamiteManager>
{
    public static event Action OnStopThrowing;
    //Spawning
    //------------------------------------------------//
    [SerializeField, Header("Spawning")]
    private Rect SpawnRect;
    [SerializeField]
    private float SpawnRate = 0.5f;
    private float _spawnTimer = 0f;

    [SerializeField]
    private int clusterAmount = 3;

    [SerializeField]
    private float BatchCooldown = 0.1f;
    private int _batchCounter = 0;
    private float _batchTimer = 0f;


    private bool _isSpawning = false;


    [SerializeField]
    private float GravityMultiplier = 1f;
    [SerializeField]
    private float ThrowTargetTime = 3f;


    [SerializeField]
    private LayerMask ExplodeLayerMask;
    [SerializeField]
    private LayerMask LevelLayerMask;
    private RaycastHit[] _raycastHits = new RaycastHit[10];

    [SerializeField, Header("Prefabs")]
    private Dynamite dynamitePrefab;


    [SerializeField]
    private Thrower[] dynamiteThrowers;

    private bool isThrowing = true;

    // Determines how hard to make dynamite
    private int _difficulty = 1;

    //Unity Functions
    //============================================================================================================//

    private void OnEnable()
    {
        DayController.OnDayChanged += OnNewDay;
    }

    private void OnDisable()
    {
        DayController.OnDayChanged -= OnNewDay;
    }


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
            _batchTimer -= Time.deltaTime;

            if (_batchCounter < clusterAmount)
            {
                if (_batchTimer < 0f)
                {
                    SpawnNew();
                    _batchCounter++;
                    _batchTimer = BatchCooldown;
                }
            }
            else
            {
                _batchCounter = 0;
                _spawnTimer = SpawnRate;
            }

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

        if (Recycler.TryGrab<Dynamite>(transform, Vector3.zero, Quaternion.identity, out var dyn) == false)
            dyn = Instantiate(dynamitePrefab, transform, true);

        var newThrowingState = transform.position.y > LevelController.TileSize * -5f;

        if (newThrowingState != isThrowing)
        {
            OnStopThrowing?.Invoke();
            isThrowing = newThrowingState;
        }


        if (isThrowing)
        {
            // Pick random thrower
            Debug.Assert(dynamiteThrowers.Length > 0, "Dynamite thrower positions missing!");
            var thrower = dynamiteThrowers.PickRandomElement();

            // Cap the thrower positions based on the current mine location
            Vector3 pos = thrower.transform.position;
            float maxY = transform.position.y + LevelController.TileSize * 10f;
            pos.y = Mathf.Min(pos.y, maxY);
            dyn.transform.position = pos;

            dyn.Init(targetPoint, GravityMultiplier, ExplodeLayerMask, LevelLayerMask, ThrowTargetTime, Dynamite.DYNAMITE_BEHAVIOUR.Thrown);
            thrower.Throw();
        }
        else
        {
            float grav = Mathf.Abs(Physics.gravity.y) * GravityMultiplier;
            dyn.transform.position = targetPoint + Vector3.up * 0.5f * grav * Mathf.Pow(ThrowTargetTime * 1.5f, 2);

            dyn.Init(targetPoint, GravityMultiplier, ExplodeLayerMask, LevelLayerMask, ThrowTargetTime, Dynamite.DYNAMITE_BEHAVIOUR.Dropped);
        }

        // Setup dynamite difficulty options
        int bonus = Math.Min(_difficulty/3,3); // bonus is 0,1,2 or 3
        dyn.Damage = Random.Range(1,2+bonus);
        dyn.ExplodeRadius = dyn.Damage * 1.5f;

    }

    private void OnNewDay(int dayNumber) {
        _difficulty = dayNumber;

        // Adjust difficulty settings
        clusterAmount = 3 + Mathf.Min(dayNumber,8);

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

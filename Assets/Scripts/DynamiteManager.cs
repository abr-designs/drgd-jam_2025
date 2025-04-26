using Audio;
using System.Collections.Generic;
using UnityEngine;

public class DynamiteManager : MonoBehaviour
{
    public Rect SpawnRect;
    public float SpawnRate = 0.5f;
    public float GravityMultiplier = 1f;

    private float _spawnTimer = 0f;

    [SerializeField]
    private LayerMask LevelLayerMask;
    private RaycastHit[] _raycastHits = new RaycastHit[10];

    [SerializeField]
    private Dynamite dynamitePrefab;
    [SerializeField]
    private GameObject indicatorPrefab;

    private List<Dynamite> _dynamiteList = new List<Dynamite>();

    private void Start()
    {
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

        // Not hitting is a problem
        Debug.Assert(hitCount > 0, "Dynamite raycast did not hit ground");

        Vector3 targetPoint = _raycastHits[0].point;
        
        var dyn = Instantiate<Dynamite>(dynamitePrefab);
        dyn.transform.parent = transform;
        dyn.transform.position = targetPoint + Vector3.up * 10f;
        var ind = Instantiate(indicatorPrefab);
        ind.transform.parent = transform;
        dyn.Spawn(targetPoint, GravityMultiplier, ind);

        SFXManager.Instance.PlaySound(SFX.DROPING, 0.25f); // hardcoded quieter volume

    }

    void OnDrawGizmos()
    {
        Vector3 center = new Vector3(SpawnRect.x + SpawnRect.width/2f,0,SpawnRect.y + SpawnRect.height/2f);
        Vector3 size = new Vector3(SpawnRect.width,5f, SpawnRect.height);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center,size);
    }

}

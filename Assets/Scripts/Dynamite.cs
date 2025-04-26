using UnityEngine;
using Utilities.Debugging;

public class Dynamite : MonoBehaviour
{
    private GameObject _indicator;
    private Vector3 _target;
    private float _gravityMult;

    private float _totalTime = 1f;
    private float _timeLeft = 0f;

    private Rigidbody _rb;
    private Vector3 _velocity = Vector3.zero;

    private LayerMask _explodeMask;
    public float ExplodeRadius = 2f;
    public float Damage = 1f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();   
    }

    private void Update()
    {   
        if(_indicator == null) return;

        _timeLeft -= Time.deltaTime;
        float t = 1f - Mathf.Clamp01(_timeLeft / _totalTime);
        Vector3 endScale = new Vector3(0.1f, 0.2f, .1f);
        Vector3 startScale = new Vector3(5f, 0.2f, 5f);
        _indicator.transform.localScale = Vector3.Lerp(startScale,endScale,t);

        _velocity += Physics.gravity * _gravityMult * Time.deltaTime;
        transform.position = transform.position + _velocity * Time.deltaTime;

        // Check if this has hit the ground
        if(_timeLeft <= 0)
        {
            DoExplode();
            Destroy(gameObject);
            Destroy(_indicator.gameObject);
        }

    }

    public void Spawn(Vector3 targetPoint, float gravityMult, GameObject indicator, LayerMask explodeMask ) {

        _indicator = indicator;
        _target = targetPoint;
        _gravityMult = gravityMult;
        _explodeMask = explodeMask;

        _indicator.transform.position = _target;

        // Calculate time
        float dist = transform.position.y - targetPoint.y;

        _totalTime = Mathf.Sqrt( (2f * dist) / Mathf.Abs(Physics.gravity.y*_gravityMult));
        _timeLeft = _totalTime;

    }

    private Collider[] _explodeHits = new Collider[10];
    private void DoExplode() {

        // Get affected tiles
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, ExplodeRadius, _explodeHits, _explodeMask);
        
        var cutOffY = _target.y - (LevelController.TileSize/2f + 0.05f);
        Draw.Circle(transform.position, Color.green, ExplodeRadius);
        
        for(int i=0;i<hitCount;i++) {

            var tile = _explodeHits[i].GetComponent<DestructableTile>();
            if(tile != null && tile.transform.position.y >= cutOffY) {
                Draw.Circle(tile.transform.position, Color.pink, LevelController.TileSize);
                // Debug.Log($"Tile damage {Damage}");
                // Debug.Break();
                tile.ApplyDamage(Damage);
            }

        }


    }

}
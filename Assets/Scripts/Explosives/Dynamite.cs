using System;
using Audio;
using Interfaces;
using UnityEditor.PackageManager;
using UnityEngine;
using Utilities.Debugging;
using Utilities.Recycling;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Dynamite : MonoBehaviour, IRecyclable
{
    public enum DYNAMITE_BEHAVIOUR
    {
        Thrown = 0,
        Dropped
    }

    public enum INDICATORS
    {
        TARGET = 0
    }

    public static int ActiveCount { get; private set; }

    public float ExplodeRadius = 2f;
    public int Damage = 1;

    [SerializeField]
    private GameObject indicatorPrefab;
    private GameObject _indicator;
    private Material _indicatorMat;
    private Vector3 _target;
    private float _gravityMult;

    private Rigidbody _rigidbody;

    private LayerMask _explodeMask;
    private LayerMask _levelMask;

    private Collider[] explosionHitColliders = new Collider[30];

    // Used to mark which frame we have already done an indicator check on
    private int _lastFrameCheck = 0;

    // Recycling
    public bool IsRecycled { get; set; }

    //Unity Functions
    //============================================================================================================//

    private void OnEnable()
    {
        ActiveCount++;
        PlayerHealth.OnPlayerDied += OnPlayerDied;
    }



    private void Update()
    {
        if (_indicator == null) return;

        // Check if the indicator needs to move
        CheckIndicator();

        // Check if this has hit the ground
        if (_target.y >= transform.position.y)
        {
            Explode();
            if (_indicator)
                Recycler.RecycleEnum(INDICATORS.TARGET, _indicator);
            if (!IsRecycled)
                Recycler.Recycle(this);
        }

    }

    private void FixedUpdate()
    {
        if (!_rigidbody) return;

        Vector3 grav = Physics.gravity * _gravityMult;
        _rigidbody.AddForce(grav, ForceMode.Acceleration);
    }

    private void OnDisable()
    {
        ActiveCount--;
        PlayerHealth.OnPlayerDied -= OnPlayerDied;
    }

    //Callbacks
    //============================================================================================================//

    private void OnPlayerDied()
    {
        if (_indicator)
            Recycler.RecycleEnum(INDICATORS.TARGET, _indicator);
        if (!IsRecycled)
            Recycler.Recycle(this);
    }

    //============================================================================================================//

    // Used for thrown behaviour
    private void Launch(float timeToTarget)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = _target;

        // distance to target
        Vector3 direction = targetPos - startPos;

        float hDist = new Vector2(direction.x, direction.z).magnitude;
        float vDist = direction.y;

        float gravity = Mathf.Abs(Physics.gravity.y) * _gravityMult;

        float velocityXZ = hDist / timeToTarget;
        float velocityY = (vDist / timeToTarget) + (0.5f * gravity * timeToTarget);

        Vector3 result = new Vector3(direction.x, 0, direction.z).normalized * velocityXZ;
        result.y = velocityY;

        _rigidbody.linearVelocity = result;
    }

    public void Init(Vector3 targetPoint, float gravityMult, LayerMask explodeMask, LayerMask levelMask, float timeToTarget = 3f,
        DYNAMITE_BEHAVIOUR behaviour = DYNAMITE_BEHAVIOUR.Dropped)
    {
        SFXManager.Instance.PlaySound(SFX.DROPING, 0.25f); // hardcoded quieter volume

        _target = targetPoint;
        if (Recycler.TryGrabEnum(INDICATORS.TARGET, null, Vector3.zero, Quaternion.identity, out _indicator) == false)
            _indicator = Instantiate(indicatorPrefab, transform.parent, true);

        _indicator.transform.position = _target;
        _indicator.transform.localScale = new Vector3(ExplodeRadius, ExplodeRadius*.1f, ExplodeRadius) * 2f;
        _indicatorMat = _indicator.GetComponent<MeshRenderer>().material;

        _gravityMult = gravityMult;
        _explodeMask = explodeMask;
        _levelMask = levelMask;

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;

        // Add a random torque
        Vector3 randomTorque = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized * 5f;

        _rigidbody.AddTorque(randomTorque, ForceMode.Impulse);

        // If it was thrown we need to calculate an initial launch force
        if (behaviour == DYNAMITE_BEHAVIOUR.Thrown) Launch(timeToTarget);

    }

    private void Explode()
    {
        SFXManager.Instance.PlaySound(SFX.EXPLOSION);

        // Get affected tiles
        int hitCount = Physics.OverlapSphereNonAlloc(_target, ExplodeRadius, explosionHitColliders, _explodeMask);

        float halfTile = LevelController.TileSize / 2f;
        var cutOffY = _target.y - (halfTile + 0.01f);
        Draw.Circle(_target, Vector3.up, Color.green, ExplodeRadius);

        for (int i = 0; i < hitCount; i++)
        {
            var healthObj = explosionHitColliders[i].GetComponent<IHaveHealth>();
            if (healthObj != null)
            {
                healthObj.ApplyDamage(Damage);
            }
        }
    }


    private RaycastHit[] _raycastHits = new RaycastHit[5];
    private void CheckIndicator()
    {

        if (!_indicator) return;

        // Color the material based on the distance left
        float dist = Mathf.Abs(transform.position.y - _indicator.transform.position.y);
        _indicatorMat.SetColor("_BaseColor", Color.Lerp(Color.red, Color.white, dist / 10.0f));

        if (Time.frameCount - _lastFrameCheck <= 10) return;

        // Do a new raycast and check  
        Vector3 startPos = _indicator.transform.position + Vector3.up * LevelController.TileSize * 5f;
        Ray ray = new Ray(startPos, Vector3.down);
        int hitCount = Physics.RaycastNonAlloc(ray, _raycastHits, LevelController.TileSize * 10f, _levelMask);

        Debug.DrawRay(startPos, Vector3.down * LevelController.TileSize * 10f, Color.purple, 1f);
        Debug.Assert(hitCount > 0, "Indicator was unable to find a tile beneath!");

        Vector3 newPos = _raycastHits[0].point;
        _indicator.transform.position = newPos;

        _lastFrameCheck = Time.frameCount;

    }


    public void OnRecycled()
    {
        try {
        _rigidbody.linearVelocity = Vector3.zero;
        _indicator = null;
        } catch (Exception e) {
            Debug.Log(gameObject, gameObject);
        }
    }

    //============================================================================================================//

}
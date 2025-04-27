using Audio;
using Interfaces;
using UnityEngine;
using Utilities.Debugging;

[RequireComponent(typeof(Rigidbody))]
public class Dynamite : MonoBehaviour
{
    public enum DYNAMITE_BEHAVIOUR
    {
        Thrown = 0,
        Dropped
    }

    [SerializeField] private float ExplodeRadius = 2f;
    [SerializeField] private int Damage = 1;

    private GameObject _indicator;
    private Vector3 _target;
    private float _gravityMult;

    private Rigidbody _rigidbody;

    private LayerMask _explodeMask;

    private Collider[] explosionHitColliders = new Collider[30];

    // Used to mark which frame we have already done an indicator check on
    private int _lastFrameCheck = 0;

    //Unity Functions
    //============================================================================================================//

    private void OnEnable()
    {
        // Listen for block change events (to change indicator)
        DestructibleTile.OnYLevelChanged += OnBlockChange;
    }
    private void OnDisable()
    {
        // Listen for block change events (to change indicator)
        DestructibleTile.OnYLevelChanged -= OnBlockChange;
    }

    private void FixedUpdate()
    {
        if (!_rigidbody) return;

        Vector3 grav = Physics.gravity * _gravityMult;
        _rigidbody.AddForce(grav, ForceMode.Acceleration);
    }

    private void Update()
    {
        if (_indicator == null) return;

        // Check if this has hit the ground
        if (_target.y >= transform.position.y)
        {
            Explode();
            Destroy(gameObject);
            Destroy(_indicator.gameObject);
        }

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
        float velocityY = (vDist/timeToTarget) + (0.5f * gravity * timeToTarget);

        Vector3 result = new Vector3(direction.x,0,direction.z).normalized * velocityXZ;
        result.y = velocityY;

        _rigidbody.linearVelocity = result;
    }

    public void Spawn(Vector3 targetPoint, float gravityMult, GameObject indicator, LayerMask explodeMask, float timeToTarget = 3f,
        DYNAMITE_BEHAVIOUR behaviour = DYNAMITE_BEHAVIOUR.Dropped)
    {
        SFXManager.Instance.PlaySound(SFX.DROPING, 0.25f); // hardcoded quieter volume

        _indicator = indicator;
        _target = targetPoint;
        _gravityMult = gravityMult;
        _explodeMask = explodeMask;

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;

        // Add a random torque
        Vector3 randomTorque = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized * 5f;

        _rigidbody.AddTorque(randomTorque, ForceMode.Impulse);

        _indicator.transform.position = _target;
        _indicator.transform.localScale = new Vector3(ExplodeRadius, 0.1f, ExplodeRadius);

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

    private void OnBlockChange(int newY) {

        if(Time.frameCount == _lastFrameCheck) return;

        _lastFrameCheck = Time.frameCount;

    }
    //============================================================================================================//

}
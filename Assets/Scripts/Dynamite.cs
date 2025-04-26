using Audio;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    private GameObject _indicator;
    private Vector3 _target;
    private float _gravityMult;

    private float _totalTime = 1f;
    private float _timeLeft = 0f;

    private Rigidbody _rb;
    private Vector3 _velocity = Vector3.zero;

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
            SFXManager.Instance.PlaySound(SFX.EXPLOSION);
            DestroyImmediate(gameObject);
            DestroyImmediate(_indicator.gameObject);
        }

    }

    public void Spawn(Vector3 targetPoint, float gravityMult, GameObject indicator ) {

        _indicator = indicator;
        _target = targetPoint;
        _gravityMult = gravityMult;

        _indicator.transform.position = _target;

        // Calculate time
        float dist = transform.position.y - targetPoint.y;

        _totalTime = Mathf.Sqrt( (2f * dist) / Mathf.Abs(Physics.gravity.y*_gravityMult));
        _timeLeft = _totalTime;

    }

}
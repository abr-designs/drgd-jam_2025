using Audio;
using Samples.CharacterController3D.Scripts;
using System;
using Audio.SoundFX;
using UnityEngine;
using GGJ.Collectables;

[Serializable]
public struct CollectableBehaviourData
{
    public float pickupDistance;
    public float drag;
    public float accel;
}

public class CollectableBase : MonoBehaviour
{
    public enum COLLECTABLE_TYPE
    {
        NONE = 0,
        DIRT = 1,
        GOLD = 2,
        COPPER_WIRE = 3,
        GEAR = 4,
        SPRING = 5,
    }

    private enum STATE
    {
        NONE,
        LAUNCHING,
        WAITING_FOR_PLAYER,
        MOVE_TO_PLAYER
    }
    //------------------------------------------------//

    //private static PlayerHealth _playerHealth;
    private static CharacterController3D _characterController;
    private static InventorySystem _inventorySystem;
    private static Transform _playerTransform;

    [SerializeField]
    private COLLECTABLE_TYPE _type;
    public COLLECTABLE_TYPE CollectableType { get; private set; }

    [SerializeField]
    private ItemStack itemStack;
    public void SetItemStack(ItemStack itemStack)
    {
        this.itemStack = itemStack;
    }
    public ItemStack GetItemStack() { return itemStack; }

    private CollectableBehaviourData _behaviourData;

    private STATE _currentState;
        
    private Vector3 _velocity;
    private float _pickupCountdown;
    private float _moveSpeed;

    public float maxDistanceFromGround = 1f;
    public float raycastDistance = 100f;
    public float moveSpeed = 5f;
    private float rotationSpeed = 50f;
    public LayerMask groundLayer; // Assign the ground layer here

    //Unity Functions
    //============================================================================================================//

    // Start is called before the first frame update
    private void Start()
    {
        if (_playerTransform != null)
            return;

        _characterController = FindAnyObjectByType<CharacterController3D>();
        _playerTransform = _characterController.transform;
    }

    //TODO Set this up as a state machine
    // Update is called once per frame
    private void Update()
    {
        //------------------------------------------------//
        
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        void Slow()
        {
            _velocity -= _velocity * (Time.deltaTime * _behaviourData.drag);
            transform.position += _velocity * Time.deltaTime;
        }

        //------------------------------------------------//
            
        var dirToPlayer = _playerTransform.position - transform.position;

        switch (_currentState)
        {
            //------------------------------------------------//
            case STATE.NONE:
                return;
            //------------------------------------------------//
            case STATE.LAUNCHING:
                if (_pickupCountdown > 0f)
                {
                    _pickupCountdown -= Time.deltaTime;
                }
                else
                    _currentState = STATE.WAITING_FOR_PLAYER;

                //Drag
                Slow();
                break;
            //------------------------------------------------//
            case STATE.WAITING_FOR_PLAYER:
                //Distance check
                if (dirToPlayer.magnitude < _behaviourData.pickupDistance)
                {
                    // need to check if the play has room forthe item in their inventory
                    if(InventorySystem.Instance.TryCheckHaveInventorySpace(itemStack))
                        _currentState = STATE.MOVE_TO_PLAYER;

                }

                // check position from ground
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
                {
                    float distance = hit.distance;

                    if (distance > maxDistanceFromGround)
                    {
                        // Move toward the ground smoothly
                        Vector3 targetPosition = hit.point + Vector3.up * maxDistanceFromGround;
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                        //transform.position = targetPosition;
                    }
                }

                Slow();
                break;
            //------------------------------------------------//
            case STATE.MOVE_TO_PLAYER:
                if (dirToPlayer.magnitude < 0.5f)
                {
                    PickUpByPlayer();
                    return;
                }
                    
                _moveSpeed += Time.deltaTime * _behaviourData.accel;
                var currentPosition = transform.position;

                currentPosition = Vector3.MoveTowards(
                    currentPosition, 
                    _playerTransform.position,
                    _moveSpeed * Time.deltaTime);
                
                transform.position = currentPosition;
                break;
            //------------------------------------------------//
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    //============================================================================================================//

    public void Launch(CollectableBehaviourData behaviourData, Vector3 direction, float speed, float pickupDelay = 1f)
    {
        _behaviourData = behaviourData;
        _velocity = direction.normalized * speed;

        _pickupCountdown = pickupDelay;
            
        _currentState = STATE.LAUNCHING;
    }

    private void PickUpByPlayer()
    {
        InventorySystem.Instance.InsertItemStackToInventory(itemStack);
        SFX.OBJECT_PICKUP.PlaySound();

        // we need to broadcast that the item has been collected
        //OnItemCollected?.Invoke(this, EventArgs.Empty);

        // remove from CollectableController list
        //CollectableController.Instance.collectables.Remove(this);
        //CollectableController.Instance.RemoveCollectable(this);

        //Destroy(gameObject);

        DestroyCollectable();
    }

    public void DestroyCollectable()
    {
        CollectableController.Instance.RemoveCollectable(this);
        Destroy(gameObject);
    }
        
    //============================================================================================================//
}
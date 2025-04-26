using NaughtyAttributes;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGJ.Collectables
{
    public class CollectableController : MonoBehaviour
    {
        private static CollectableController _instance;

        [SerializeField]
        private CollectableBase collectablePrefab;
        [SerializeField]
        private List<CollectableBase> collectablePrefabs;

        [SerializeField, Min(0f)]
        private float launchSpeed;
        [SerializeField, Min(0f)]
        private float pickupDelay = 1f;

        [SerializeField, Space(10f)] 
        private CollectableBehaviourData collectableBehaviourData;

        //Unity Functions
        //============================================================================================================//

        private void Awake()
        {
            _instance = this;
        }

        //============================================================================================================//
        
        public static void CreateCollectable(Vector3 position, int count)
        {
            Debug.Log("Create Collectable");
            _instance.CreateCollectables(position, count, _instance.pickupDelay);
        }
        public static void CreateCollectable(Vector3 position, int count, float delay)
        {
            _instance.CreateCollectables(position, count, delay);
        }

        private void CreateCollectables(Vector3 position, int count, float delay)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject selectedPrefab = SelectRandomPrefab();
                var newCollectable = Instantiate(selectedPrefab, position, quaternion.identity, transform);
                CollectableBase collectable = newCollectable.GetComponent<CollectableBase>();

                var dir = Random.insideUnitCircle.normalized;

                //float vertical = Random.Range(2, 5);
                dir = Vector2.zero;
                collectable.Launch(collectableBehaviourData, new Vector3(dir.x, 0, dir.y), launchSpeed, delay);
                //newCollectable.Launch(collectableBehaviourData, new Vector3(dir.x, vertical, dir.y), launchSpeed, delay);
            }
        }

        private GameObject SelectRandomPrefab()
        {
            return collectablePrefabs[Random.Range(0, collectablePrefabs.Count)].gameObject;
        }
        
        //Unity Editor
        //============================================================================================================//
        
#if UNITY_EDITOR
        
        [SerializeField, Min(1), Header("DEBUGGING")]
        private int spawnCount;
        [SerializeField]
        private Vector3 spawnPosition;
        
        [ContextMenu("Spawn Test Collectables")]
        private void SpawnCollectables()
        {
            //if (Application.isPlaying == false)
            //    return;
            
            for (int i = 0; i < spawnCount; i++)
            {
                var newCollectable = Instantiate(collectablePrefab, spawnPosition, quaternion.identity, transform);

                var dir = Random.insideUnitCircle.normalized;
                
                newCollectable.Launch(collectableBehaviourData, new Vector3(dir.x, 0, dir.y), launchSpeed, pickupDelay);
            }
        }

        [Button]
        private void SpawnCollectable()
        {
            CreateCollectables(spawnPosition, spawnCount, 0);
        }

#endif
        //============================================================================================================//
    }
}
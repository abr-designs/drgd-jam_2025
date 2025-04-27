using NaughtyAttributes;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGJ.Collectables
{
    public class CollectableController : MonoBehaviour
    {
        private static CollectableController _instance;
        public static CollectableController Instance { get { return _instance; } }

        [SerializeField] private Transform collectablesContainer;
        [SerializeField] private List<CollectableBase> collectablePrefabs;

        [SerializeField] private static float dropProbability = 0.2f;

        [SerializeField, Min(0f)]
        private float launchSpeed;
        [SerializeField, Min(0f)]
        private float pickupDelay = 1f;

        [SerializeField, Space(10f)] 
        private CollectableBehaviourData collectableBehaviourData;

        public List<CollectableBase> collectables;
        public int maxCollectibles = 20;

        //Unity Functions
        //============================================================================================================//

        private void Awake()
        {
            _instance = this;
            collectables = new List<CollectableBase>();
        }

        //============================================================================================================//
        
        public static void TryCreateCollectable(Vector3 position, int count)
        {
            if (Random.value > dropProbability)
                return;

            CreateCollectable(position, count);
        }

        public static void CreateCollectable(Vector3 position, int count)
        {
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
                if (collectables.Count >= maxCollectibles)
                {
                    //Debug.Log("Too many");
                    RemoveCollectable(collectables[0]);
                    return;
                }

                GameObject selectedPrefab = SelectRandomPrefab();
                var newCollectable = Instantiate(selectedPrefab, position, quaternion.identity, collectablesContainer);
                CollectableBase collectable = newCollectable.GetComponent<CollectableBase>();
                collectables.Add(collectable);
                //Debug.Log($"collectables contains {collectables.Count}");

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

        public void RemoveCollectable(CollectableBase collectable)
        {
            collectables.Remove(collectable);
            //Debug.Log($"collectables contains {collectables.Count}");
        }
        
        //Unity Editor
        //============================================================================================================//
        
#if UNITY_EDITOR
        
        [SerializeField, Min(1), Header("DEBUGGING")]
        private int spawnCount;
        [SerializeField]
        private Vector3 spawnPosition;
        [SerializeField] private bool requireApplicationPlaying = true;

        [Button]
        private void SpawnCollectable()
        {
            if (requireApplicationPlaying && Application.isPlaying == false)
                return;
            
            CreateCollectables(spawnPosition, spawnCount, 0);
        }

#endif
        //============================================================================================================//
    }
}
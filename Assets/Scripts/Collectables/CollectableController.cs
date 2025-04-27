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

        public static void TryCreateCollectable(LootTable lootTable, Vector3 position)
        {
            GameObject lootPrefab = null;

            ItemStack newItemStack = lootTable.GetRandomLootItem();
            if (newItemStack == null)
                return;

            InventoryItemSO spawnItemSO = newItemStack.itemSo;
            if (spawnItemSO == null)
                return;

            lootPrefab = spawnItemSO.collectablePrefab.gameObject;
            if (lootPrefab == null)
                return;

            CreateCollectable(lootPrefab, position);
        }

        public static void CreateCollectable(GameObject lootPrefab, Vector3 position)
        {
            _instance.CreateCollectables(lootPrefab, position, _instance.pickupDelay);
        }
        public static void CreateCollectable(GameObject lootPrefab, Vector3 position, float delay)
        {
            _instance.CreateCollectables(lootPrefab, position, delay);
        }

        private void CreateCollectables(Vector3 position, float delay)
        {
            GameObject selectedPrefab = SelectRandomPrefab();
            _instance.CreateCollectables(selectedPrefab, position, delay);
        }

        private void CreateCollectables(GameObject lootPrefab, Vector3 position, float delay)
        {
            if (collectables.Count >= maxCollectibles)
            {
                //Debug.Log("Too many");
                RemoveCollectable(collectables[0]);
                return;
            }

            //GameObject selectedPrefab = SelectRandomPrefab();
            var newCollectable = Instantiate(lootPrefab, position, quaternion.identity, collectablesContainer);
            CollectableBase collectable = newCollectable.GetComponent<CollectableBase>();
            collectables.Add(collectable);
            //Debug.Log($"collectables contains {collectables.Count}");

            var dir = Random.insideUnitCircle.normalized;

            //float vertical = Random.Range(2, 5);
            dir = Vector2.zero;
            collectable.Launch(collectableBehaviourData, new Vector3(dir.x, 0, dir.y), launchSpeed, delay);
            //newCollectable.Launch(collectableBehaviourData, new Vector3(dir.x, vertical, dir.y), launchSpeed, delay);
        }

        private GameObject SelectRandomPrefab()
        {
            return collectablePrefabs[Random.Range(0, collectablePrefabs.Count)].gameObject;
        }

        public void RemoveCollectable(CollectableBase collectable)
        {
            collectables.Remove(collectable);
            Destroy(collectable.gameObject);
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

            CreateCollectables(spawnPosition, spawnCount);
        }

#endif
        //============================================================================================================//
    }
}
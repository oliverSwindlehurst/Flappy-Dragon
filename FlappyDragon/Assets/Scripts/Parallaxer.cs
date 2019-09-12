using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{
    class PoolObject
    {
        public Transform transform;
        public bool inUse;

        public PoolObject(Transform transformation)
        {
            transform = transformation;
        }

        public void Use()
        {
            inUse = true;
        }

        public void Dispose()
        {
            inUse = false;
        }
    }

        [System.Serializable]
        public struct YspawnRange
    {
        public float minY;
        public float maxY;
    }

    public GameObject Prefab;
    public int poolSize;
    public float shiftSpeed;
    public float spawnRate;

    public YspawnRange ySpawnRange;
    public Vector3 defaultSpawnPossition;
    public bool spawnImmediate; //Partical Prewarm
    public Vector3 immediateSpawnPossition;
    public Vector2 targetAspectRatio;

    float spawnTimer;
    float targetAspect;

    PoolObject[] poolObjects;
    GameManager game;

    void Awake()
    {
        Configure();   
    }

    void Start()
    {
        game = GameManager.Instance;
    }

    private void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameOverConfirmed()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }

        Configure();
    }

    void Update()
    {
        if (game.GameOver) return;

        Shift();
        spawnTimer += Time.deltaTime;

        if (spawnTimer > spawnRate)
        {
            Spawn();
            spawnTimer = 0;
        }
    }

    void Configure()
    {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];

        for (int i = 0; i < poolObjects.Length; i++)
        {
            GameObject gameObject = Instantiate(Prefab) as GameObject;  
            Transform transform = gameObject.transform;
            transform.SetParent(transform);
            transform.position = Vector3.one * 1000;
            poolObjects[i] = new PoolObject(transform);
        }

        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

    void Spawn()
    {
        Transform spawnTransform = GetPoolObject();

        if (spawnTransform == null) return; //if true, this indicates that poolSize is too small
        Vector3 spawnPossition = Vector3.zero;
        spawnPossition.x = (defaultSpawnPossition.x * Camera.main.aspect) / targetAspect;
        spawnPossition.y = Random.Range(ySpawnRange.minY, ySpawnRange.maxY);

        spawnTransform.position = spawnPossition;

    }

    void SpawnImmediate()
    {
        Transform spawnTransform = GetPoolObject();

        if (spawnTransform == null) return; //if true, this indicates that poolSize is too small
        Vector3 spawnPossition = Vector3.zero;
        spawnPossition.x = (immediateSpawnPossition.x * Camera.main.aspect) / targetAspect;
        spawnPossition.y = Random.Range(ySpawnRange.minY, ySpawnRange.maxY);

        spawnTransform.position = spawnPossition;

        Spawn();
    }

    void Shift()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].transform.position += -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjects[i]);
        }
    }

    void CheckDisposeObject(PoolObject poolObject)
    {
        if (poolObject.transform.position.x < (-defaultSpawnPossition.x * Camera.main.aspect) / targetAspect)
        {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 1000;
        }
    }

    Transform GetPoolObject()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            if (!poolObjects[i].inUse)
            {
                poolObjects[i].Use();
                return poolObjects[i].transform;
            }
        }
        return null;
    }
}
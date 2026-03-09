using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public static ZombieSpawner Instance { get; private set; }

    [Header("Spawning")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private int zombieCount = 10;
    [SerializeField] private float arenaSize = 18f;

    private List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < zombieCount; i++)
        {
            GameObject z = Instantiate(zombiePrefab, GetRandomSpawnPoint(), Quaternion.identity);
            pool.Add(z);
        }
    }

    public Vector3 GetRandomSpawnPoint()
    {
        // Spawn near edges so they don't appear on top of car
        float x = Random.Range(-arenaSize, arenaSize);
        float z = Random.Range(-arenaSize, arenaSize);
        return new Vector3(x, 0f, z);
    }
}
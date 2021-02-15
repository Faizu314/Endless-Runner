using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    private const int POOL_OFFSET_Y = -400;

    [SerializeField] private List<Powerup> powerups;
    [SerializeField] private Transform player;
    [SerializeField] private float tickPeriod;

    private List<Queue<GameObject>> idlePowerups;
    private List<GameObject>[] activePowerups;
    private EnemySpawner enemySpawner;
    private List<int> candidates = new List<int>();
    float x = 0f;

    void Start()
    {
        enemySpawner = GameObject.Find("Level Designer").GetComponent<EnemySpawner>();
        idlePowerups = new List<Queue<GameObject>>();
        activePowerups = new List<GameObject>[powerups.Count];
        for (int i = 0; i < powerups.Count; i++)
        {
            idlePowerups.Add(new Queue<GameObject>());
            activePowerups[i] = new List<GameObject>();
        }

        for (int i = 0; i < powerups.Count; i++)
        {
            GameObject pool = new GameObject(powerups[i].name + " Pool");
            pool.transform.parent = transform;

            for (int j = 0; j < powerups[i].poolSize; j++)
            {
                GameObject currentPowerup = Instantiate(powerups[i].prefab, new Vector3(0, POOL_OFFSET_Y, 0), Quaternion.identity);
                currentPowerup.transform.parent = pool.transform;
                currentPowerup.SetActive(false);
                idlePowerups[i].Enqueue(currentPowerup);
            }
        }
    }

    private void Spawn()
    {
        if (candidates.Count == 0)
            return;
        int index = Random.Range(0, candidates.Count);
        if (idlePowerups[index].Count == 0)
        {
            Debug.Log("Pool of Power Up " + powerups[candidates[index]].name + " depleted");
            return;
        }
        GameObject spawn = idlePowerups[candidates[index]].Dequeue();
        activePowerups[candidates[index]].Add(spawn);
        spawn.SetActive(true);
        spawn.transform.position = new Vector3(Random.Range(-4, 4), -0.5f, player.position.z + 40f);
        candidates.Clear();
    }

    public void Despawn(GameObject powerup)
    {
        for (int i = 0; i < activePowerups.Length; i++)
        {
            if (activePowerups[i].Contains(powerup))
            {
                powerup.SetActive(false);
                powerup.transform.position = new Vector3(0, POOL_OFFSET_Y, 0);
                activePowerups[i].Remove(powerup);
                idlePowerups[i].Enqueue(powerup);
            }
        }
    }

    void Update()
    {
        if (enemySpawner.bossMode)
            return;
        x += Time.deltaTime;
        if (x < tickPeriod)
            return;
        x = Random.value;
        for (int i = 0; i < powerups.Count; i++)
            if (powerups[i].spawnRate > x)
                candidates.Add(i);
        Spawn();
        x = 0f;
    }

    [System.Serializable] public struct Powerup
    {
        public string name;
        public GameObject prefab;
        [Range(0, 1)] public float spawnRate;
        public int poolSize;
    }
}

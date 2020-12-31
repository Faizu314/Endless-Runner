using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    private const int POOL_OFFSET_Y = -300;

    [SerializeField] private PlayerMovement player;
    [SerializeField] private int levelLength;
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private List<EnemySpawnDetails> enemyDetails;

    private List<Queue<GameObject>> idlePools;
    private List<GameObject>[] activeEnemies;
    private List<int> candidates = new List<int>();
    private float x = 0f;
    private bool bossMode = false;

    private void Start()
    {
        idlePools = new List<Queue<GameObject>>();
        activeEnemies = new List<GameObject>[enemyPrefabs.Count];
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            idlePools.Add(new Queue<GameObject>());
            activeEnemies[i] = new List<GameObject>();
        }
        if (enemyPrefabs.Count != enemyDetails.Count)
        {
            Debug.LogError("Enemy details and prefabs not matching up. Expect loads of errors");
        }
        
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            GameObject child = new GameObject(enemyDetails[i].enemyName + " Pool");
            child.transform.parent = transform;

            if (enemyDetails[i].isBoss)
            {
                GameObject obj = Instantiate(enemyPrefabs[i], new Vector3(0, POOL_OFFSET_Y, 0), Quaternion.identity);
                obj.transform.parent = child.transform;
                idlePools[i].Enqueue(obj);
            }
            else
            {
                for (int j = 0; j < enemyDetails[i].preferredPoolSize; j++)
                {
                    GameObject obj = Instantiate(enemyPrefabs[i], new Vector3(0, POOL_OFFSET_Y, 0), Quaternion.identity);
                    obj.transform.parent = child.transform;
                    idlePools[i].Enqueue(obj);
                }
            }
        }
    }

    private void SpawnRandomCandidate()
    {
        if (candidates.Count == 0)
            return;
        int index = Random.Range(0, candidates.Count);
        if (enemyDetails[candidates[index]].isBoss)
            Spawn(candidates[index], new Vector3(0, 1.69f, player.transform.position.z + 40f));
        else
            Spawn(candidates[index], new Vector3(Random.Range(-7f, 7f), 1.69f, player.transform.position.z + 40f));
        candidates.Remove(candidates[index]);
    }

    private void Update()
    {
        if (bossMode)
            return;
        if (Mathf.Abs(player.transform.position.z - levelLength) < 5)
        {
            Debug.Log("Mubarakan muk gyeh level");
            return;
        }
        SpawnRandomCandidate();

        x += Time.deltaTime;
        if (x > 1)
            x = Random.value;
        else
            return;

        for (int i = 0; i < enemyDetails.Count; i++)
            if (x < enemyDetails[i].spawnRates.Evaluate(player.transform.position.z / levelLength))
                candidates.Add(i);

        x = 0;
    }

    public void Despawn(GameObject obj)
    {
        for (int i = 0; i < activeEnemies.Length; i++)
        {
            if (activeEnemies[i].Contains(obj))
            {
                activeEnemies[i].Remove(obj);
                obj.transform.position = new Vector3(0, POOL_OFFSET_Y, 0);
                obj.transform.rotation = Quaternion.identity;
                obj.SetActive(false);
                idlePools[i].Enqueue(obj);
                break;
            }
        }
    }

    public void Spawn(int index, Vector3 position)
    {
        if (idlePools[index].Count == 0)
        {
            Debug.LogError("Enemy pool of " + enemyDetails[index].enemyName + " depleted. Perhaps consider a bigger pool size");
            return;
        }
        if (enemyDetails[index].isBoss)
        {
            player.Stop();
            bossMode = true;
        }
        GameObject obj = idlePools[index].Dequeue();
        activeEnemies[index].Add(obj);
        obj.transform.position = position;
        obj.SetActive(true);
    }

    public void DisableBossMode()
    {
        bossMode = false;
    }

    public List<GameObject> GetEnemyPrefabs()
    {
        return enemyPrefabs;
    }

    public int GetLevelLength()
    {
        return levelLength;
    }

    [System.Serializable] private struct EnemySpawnDetails
    {
        public string enemyName;
        public AnimationCurve spawnRates;
        public int preferredPoolSize;
        public bool isBoss;
    }
}

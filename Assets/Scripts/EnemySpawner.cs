using UnityEngine;
using System;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    private const int POOL_OFFSET_Y = -300;
    public const int LEFT_BOUNDARY = -4;
    public const int RIGHT_BOUNDARY = 4;

    [SerializeField] private PlayerMovement player;
    [SerializeField] private List<BiomeSectorManager> biomes;
    public List<EnemyDetails> enemyDetails;

    [HideInInspector] public bool bossMode = false;
    private List<Queue<GameObject>> idlePools;
    private List<GameObject>[] activeEnemies;
    private Action bossCallBack;
    private int currentBossIndex;
    private bool bossPresent = false;

    private void Start()
    {
        CheckPoint.Load();
        Debug.Log("Resuming from: " + CheckPoint.progress);
        idlePools = new List<Queue<GameObject>>();
        activeEnemies = new List<GameObject>[enemyDetails.Count];
        for (int i = 0; i < enemyDetails.Count; i++)
        {
            idlePools.Add(new Queue<GameObject>());
            activeEnemies[i] = new List<GameObject>();
        }
        
        for (int i = 0; i < enemyDetails.Count; i++)
        {
            GameObject child = new GameObject(enemyDetails[i].enemyName + " Pool");
            child.transform.parent = transform;

            if (enemyDetails[i].enemyPrefab != null)
            {
                if (enemyDetails[i].isBoss)
                {
                    GameObject obj = Instantiate(enemyDetails[i].enemyPrefab, new Vector3(0, POOL_OFFSET_Y, 0), Quaternion.identity);
                    obj.transform.parent = child.transform;
                    idlePools[i].Enqueue(obj);
                }
                else
                {
                    for (int j = 0; j < enemyDetails[i].preferredPoolSize; j++)
                    {
                        GameObject obj = Instantiate(enemyDetails[i].enemyPrefab, new Vector3(0, POOL_OFFSET_Y, 0), Quaternion.identity);
                        obj.transform.parent = child.transform;
                        idlePools[i].Enqueue(obj);
                    }
                }
            }
            else
                Debug.Log(enemyDetails[i].enemyName + " Has no prefab provided");
        }
        Resume();
    }

    private void Update()
    {
        if (bossMode)
        {
            int currentlyActive = 0;
            for (int i = 0; i < activeEnemies.Length; i++)
            {
                currentlyActive += activeEnemies[i].Count;
            }
            if (bossPresent)
            {
                if (currentlyActive == 1 && bossCallBack != null)
                {
                    bossCallBack();
                }
                else if (currentlyActive == 0)
                {
                    DisableBossMode();
                }
            }
            else if (currentlyActive == 0)
            {
                player.Stop();
                GameObject obj = idlePools[currentBossIndex].Dequeue();
                activeEnemies[currentBossIndex].Add(obj);
                obj.transform.position = player.transform.position + Vector3.forward * 40f;
                obj.SetActive(true);
                bossPresent = true;
            }
        }
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
            //player.Stop();
            bossMode = true;
            currentBossIndex = index;
            return;
        }
        GameObject obj = idlePools[index].Dequeue();
        activeEnemies[index].Add(obj);
        obj.transform.position = position;
        obj.SetActive(true);
    }

    public void DisableBossMode()
    {
        bossMode = false;
        bossPresent = false;
        bossCallBack = null;
        currentBossIndex = -1;
        player.Move();
    }

    public void SetBossCallBack(Action bossCallBack)
    {
        this.bossCallBack = bossCallBack;
    }

    public void InitiateRandomBiome()
    {
        int index = UnityEngine.Random.Range(0, biomes.Count);
        biomes[index].Initialize();
        biomes[index].enabled = true;
        CheckPoint.Save(player.transform.position.z + CheckPoint.progress, index);
    }

    public void Resume()
    {
        int lastBiome = CheckPoint.currentBiome;
        biomes[lastBiome].Initialize();
        biomes[lastBiome].enabled = true;
    }

    [System.Serializable] public struct EnemyDetails
    {
        public string enemyName;
        public GameObject enemyPrefab;
        public int preferredPoolSize;
        public bool isBoss;
    }
}
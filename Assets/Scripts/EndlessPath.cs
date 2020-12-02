using UnityEngine;
using System.Collections.Generic;

public class EndlessPath : MonoBehaviour
{
    private const float POOL_OFFSET_Y = -250f;

    [SerializeField] private GameObject[] pathPrefabs;
    [SerializeField] private float pathWidthInUnits;
    [SerializeField] private int concurrentPaths;
    [SerializeField] private Transform player;

    private GameObject[] pathsPool;
    private List<int> idlePathsIndex = new List<int>();
    private List<int> activePathsIndex = new List<int>();
    private int totalTiles;

    private void Start()
    {
        totalTiles = pathPrefabs.Length;
        pathsPool = new GameObject[totalTiles];

        if (totalTiles < concurrentPaths)
        {
            concurrentPaths = totalTiles;
            Debug.LogError("Concurrent paths are greater than the number of tiles given");
        }

        for (int i = 0; i < totalTiles; i++)
        {
            pathsPool[i] = Instantiate(pathPrefabs[i], Vector3.up * POOL_OFFSET_Y, Quaternion.identity);
            pathsPool[i].transform.parent = transform;
            idlePathsIndex.Add(i);
        }

        for (int i = 0; i < concurrentPaths; i++)
            SpawnPath(Vector3.forward * pathWidthInUnits * i);
    }

    private void Update()
    {
        for (int i = 0; i < activePathsIndex.Count; i++)
        {
            float currentPathZ = pathsPool[activePathsIndex[i]].transform.position.z;
            if (player.transform.position.z - currentPathZ > pathWidthInUnits / 2f + 5f)
            {
                DespawnPath(i);
                SpawnPath(Vector3.forward * (currentPathZ + concurrentPaths * pathWidthInUnits));
                break;
            }
        }
    }

    private void SpawnPath(Vector3 position)
    {
        if (idlePathsIndex.Count == 0)
            return;
        int randomIndex = Random.Range(0, idlePathsIndex.Count - 1);
        pathsPool[idlePathsIndex[randomIndex]].transform.SetPositionAndRotation(position, Quaternion.identity);
        activePathsIndex.Add(idlePathsIndex[randomIndex]);
        idlePathsIndex.Remove(idlePathsIndex[randomIndex]);
    }

    private void DespawnPath(int pathIndex)
    {
        pathsPool[activePathsIndex[pathIndex]].transform.SetPositionAndRotation(Vector3.up * POOL_OFFSET_Y, Quaternion.identity);
        idlePathsIndex.Add(activePathsIndex[pathIndex]);
        activePathsIndex.Remove(activePathsIndex[pathIndex]);
    }
}

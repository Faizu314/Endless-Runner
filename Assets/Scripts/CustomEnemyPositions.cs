using UnityEngine;
using System.Collections.Generic;

//Enemy names in EnemySpawner must corelate with enemyIndicies in CustomEnemySpawner
public class CustomEnemyPositions : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [HideInInspector] public List<Spawnee> spawnees;
    public List<EnemyData> data;

    private EnemySpawner enemySpawner;
    private int currentEnemyIndex;

    public List<float> Markers;
    public bool draw;
    [HideInInspector] public int selectedSpawnee = 0;

    public void Start()
    {
        enemySpawner = GetComponent<EnemySpawner>();
        spawnees.Sort((p1,p2) => p1.position.y.CompareTo(p2.position.y));
        currentEnemyIndex = 0;
    }
    
    void Update()
    {
        if (spawnees[currentEnemyIndex].position.z - player.transform.position.z <= 40f)
        {
            RequestSpawn();
            currentEnemyIndex++;
            if (currentEnemyIndex == spawnees.Count)
            {
                enabled = false;
            }
        }
    }

    private void RequestSpawn()
    {
        enemySpawner.Spawn(spawnees[currentEnemyIndex].enemyIndex, spawnees[currentEnemyIndex].position);
    }

    public void AddEnemy(int index)
    {
        if (index >= data.Count)
        {
            Debug.Log("No data for this index");
            return;
        }

        Spawnee enemy = new Spawnee();
        for (int i = 0; i < data.Count; i++)
        {
            if (index == data[i].enemyIndex)
            {
                enemy.enemyName = data[i].enemyName;
                enemy.enemyIndex = index;
            }
        }
        spawnees.Add(enemy);
    }

    public void RemoveEnemy(int index)
    {
        if (index >= data.Count)
        {
            Debug.Log("No data for this index");
            return;
        }

        for (int i = spawnees.Count - 1; i >= 0; i--)
        {
            if (index == spawnees[i].enemyIndex)
            {
                spawnees.RemoveAt(i);
                break;
            }
        }
    }

    [System.Serializable] public class EnemyData
    {
        public string enemyName;
        public int enemyIndex;
        public Color color;
        public bool Draw;
    }

    [System.Serializable] public class Spawnee
    {
        public string enemyName;
        public int enemyIndex;
        public Vector3 position;

        public void MoveRight()
        {
            position.x += 0.1f;
        }
        public void MoveLeft()
        {
            position.x -= 0.1f;
        }
        public void MoveUp()
        {
            position.z += 0.1f;
        }
        public void MoveDown()
        {
            position.z -= 0.1f;
        }


        public void MoveRight_()
        {
            position.x += 1f;
        }
        public void MoveLeft_()
        {
            position.x -= 1f;
        }
        public void MoveUp_()
        {
            position.z += 1f;
        }
        public void MoveDown_()
        {
            position.z -= 1f;
        }
    }

    public void OnDrawGizmos()
    {
        if (draw)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(new Vector3(-10, 0, 0), new Vector3(-10, 0, 500));
            Gizmos.DrawLine(new Vector3(10, 0, 0), new Vector3(10, 0, 500));

            Color temp = new Color(0, 0, 0);
            foreach (Spawnee enemy in spawnees)
            {
                if (data[enemy.enemyIndex].Draw)
                {
                    temp.r = data[enemy.enemyIndex].color.r;
                    temp.g = data[enemy.enemyIndex].color.g;
                    temp.b = data[enemy.enemyIndex].color.b;
                    Gizmos.color = temp;
                    Gizmos.DrawSphere(enemy.position, 1f);
                }
            }

            Gizmos.color = Color.white;
            foreach (float marker in Markers)
            {
                Vector3 left = new Vector3(-10f, 0, marker);
                Vector3 right = new Vector3(10f, 0, marker);
                Gizmos.DrawLine(left, right);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawLine(spawnees[selectedSpawnee].position, spawnees[selectedSpawnee].position + Vector3.up * 3);
        }
    }
}

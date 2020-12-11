﻿using UnityEngine;
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
    [HideInInspector] public bool editModeEnabled = true;
    [HideInInspector] public int selectedSpawnee = 0;

    private void Awake()
    {
        if (editModeEnabled)
        {
            Debug.Log("You forgot to save the enemy positions before playing");
        }
    }
    private void Start()
    {
        enemySpawner = GetComponent<EnemySpawner>();
        spawnees.Sort((p1,p2) => p1.position.z.CompareTo(p2.position.z));
        currentEnemyIndex = 0;
    }
    
    private void Update()
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

    // Editor Code
    public void ReviewPositions()
    {
        editModeEnabled = true;
        for (int i = 0; i < spawnees.Count; i++)
        {
            spawnees[i].EnableEditMode(data[spawnees[i].enemyIndex].mat);
        }
    }
    public void SavePositions()
    {
        editModeEnabled = false;
        for (int i = 0; i < spawnees.Count; i++)
        {
            spawnees[i].SavePos();
            if (spawnees[i].enemyName == null)
            {
                spawnees.RemoveAt(i);
                i--;
                selectedSpawnee = 0;
            }
        }

        int N = transform.childCount;
        for (int i = 0; i < N; i++)
        {
            GameObject addition = transform.GetChild(i).gameObject;
            string name = null;
            int index = 0;
            for (int x = 0; x < addition.name.Length; x++)
            {
                if (addition.name[x] == ' ')
                {
                    name = addition.name.Substring(0, x);
                    x = addition.name.Length;
                }
            }
            for (int j = 0; j < data.Count; j++)
            {
                if (name == data[j].enemyName)
                {
                    index = j;
                }
            }
            Spawnee temp = new Spawnee(name, index, data[index].mat, addition);
            spawnees.Add(temp);
        }
        
        for (int i = spawnees.Count - N; i < spawnees.Count; i++)
        {
            spawnees[i].SavePos();
        }
    }
    public void UpdateSpawneePositions()
    {
        if (!editModeEnabled)
        {
            return;
        }
        foreach (Spawnee spawnee in spawnees)
        {
            spawnee.UpdatePos();
        }
    }

    public void AddEnemy(int dataIndex)
    {
        if (!editModeEnabled)
        {
            Debug.Log("Can only add enemy in edit mode. Press 'ReviewPositions'");
            return;
        }
        if (dataIndex >= data.Count)
        {
            Debug.Log("No data for this index. Data List must match enemy prefabs list in EnemySpawner");
            return;
        }

        Spawnee enemy = new Spawnee(data[dataIndex].enemyName, dataIndex, data[dataIndex].mat);
        spawnees.Add(enemy);
    }

    public void RemoveEnemy(int spawneesIndex)
    {
        if (!editModeEnabled)
        {
            Debug.Log("Can only remove enemy in edit mode. Press 'Review Positions'");
            return;
        }
        if (spawneesIndex >= spawnees.Count)
        {
            Debug.Log("Select an Enemy from the list of white boxes (not black) before deleting it?");
            return;
        }

        if (spawnees[spawneesIndex].dummy == null)
        {
            Debug.Log("This enemy's dummy has been manually deleted. Save positions " +
                "and then update spawnee data to reflect additions or removals of dummy gameobjects");
        }
        if (spawnees[spawneesIndex].dummy != null)
        {
            DestroyImmediate(spawnees[spawneesIndex].dummy);
        }
        spawnees.RemoveAt(spawneesIndex);

        selectedSpawnee = 0;
    }

    [System.Serializable] public class EnemyData
    {
        public string enemyName;
        public Material mat;
        public bool Draw;
    }

    [System.Serializable] public class Spawnee
    {
        public string enemyName;
        public int enemyIndex;
        public Vector3 position;
        public GameObject dummy;
        private Material mat;

        public Spawnee(string EN, int EI, Material mat)
        {
            this.mat = mat;
            dummy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dummy.name = EN + " Dummy";
            dummy.GetComponent<MeshRenderer>().sharedMaterial = mat;
            position = new Vector3(0, 1.69f, 0);
            dummy.transform.position = position;
            dummy.transform.parent = GameObject.Find("Level Designer").transform;
            enemyName = EN;
            enemyIndex = EI;
        }
        public Spawnee(string EN, int EI, Material mat, GameObject dumdum)
        {
            this.mat = mat;
            dummy = dumdum;
            dummy.name = EN + " Dummy";
            dummy.GetComponent<MeshRenderer>().sharedMaterial = mat;
            position = dummy.transform.position;
            enemyName = EN;
            enemyIndex = EI;
        }
        public void UpdatePos()
        {
            if (dummy == null)
            {
                enemyName = null;
                return;
            }
            position = dummy.transform.position;
        }
        public void SavePos()
        {
            UpdatePos();
            DestroyImmediate(dummy);
        }
        public void EnableEditMode(Material mat)
        {
            dummy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dummy.name = enemyName + " Dummy";
            dummy.GetComponent<MeshRenderer>().sharedMaterial = mat;
            dummy.transform.position = position;
            dummy.transform.parent = GameObject.Find("Level Designer").transform;
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
            if (spawnees.Count != 0 && !editModeEnabled)
            {
                foreach (Spawnee enemy in spawnees)
                {
                    if (data[enemy.enemyIndex].Draw)
                    {
                        temp.r = data[enemy.enemyIndex].mat.color.r;
                        temp.g = data[enemy.enemyIndex].mat.color.g;
                        temp.b = data[enemy.enemyIndex].mat.color.b;
                        Gizmos.color = temp;
                        Gizmos.DrawSphere(enemy.position, 1f);
                    }
                }
            }

            Gizmos.color = Color.red;
            if (selectedSpawnee < spawnees.Count && editModeEnabled)
            {
                Gizmos.DrawLine(spawnees[selectedSpawnee].position, spawnees[selectedSpawnee].position + Vector3.up * 5);
            }

            Gizmos.color = Color.white;
            if (Markers.Count != 0)
            {
                foreach (float marker in Markers)
                {
                    Vector3 left = new Vector3(-10f, 0, marker);
                    Vector3 right = new Vector3(10f, 0, marker);
                    Gizmos.DrawLine(left, right);
                }
            }
        }
    }
}

using UnityEngine;

public class BiomeSectorManager : MonoBehaviour
{
    [SerializeField] private string sectorName;
    [SerializeField] private CustomFormations myFormations;
    [SerializeField] private int myBossIndex;
    [SerializeField] private BiomeSectorManager nextSector;
    [SerializeField] private float tickPeriod;
    [SerializeField] private int sectorLength;

    public enum Position { Transitioning, Last, Transitioning_Boss, Last_Boss };
    [Space(10)] public Position position;

    private float basePosition;
    private EnemySpawner enemySpawner;
    private Transform player;

    void Awake()
    {
        enemySpawner = GameObject.Find("Level Designer").GetComponent<EnemySpawner>();
        player = GameObject.Find("Player").transform;
    }

    void Start()
    {
        if (sectorName == null)
            sectorName = new string(' ', 0);
        if (myFormations == null)
        {
            Debug.Log("Formations not provided for Biome: " + gameObject.name + ", Sector: " + sectorName);
            enabled = false;
        }
        if ((position == Position.Transitioning || position == Position.Transitioning_Boss) && nextSector == null)
        {
            Debug.Log("Next Sector not provided for Biome: " + gameObject.name + ", Sector: " + sectorName);
            enabled = false;
        }
        if (myBossIndex < 0 || sectorLength < 0)
        {
            Debug.Log("Invalid boss index or sectorLength");
            enabled = false;
        }
        if (tickPeriod < 0)
        {
            Debug.Log("Invalid tick period");
            enabled = false;
        }
    }

    public void Initialize()
    {
        myFormations.tickPeriod = tickPeriod;
        basePosition = player.position.z;
        myFormations.enabled = true;
    }

    void Update()
    {
        if (player.position.z - basePosition >= sectorLength)
        {
            myFormations.enabled = false;
            if (position == Position.Transitioning)
            {
                nextSector.Initialize();
                nextSector.enabled = true;
                enabled = false;
            }
            else if (position == Position.Transitioning_Boss)
            {
                enemySpawner.Spawn(myBossIndex, player.position + Vector3.forward * 40f);
                nextSector.Initialize();
                nextSector.enabled = true;
                enabled = false;
            }
            else if (position == Position.Last)
            {
                enemySpawner.BiomeEnded();
                enabled = false;
            }
            else
            {
                enemySpawner.Spawn(myBossIndex, player.position + Vector3.forward * 40f);
                enemySpawner.BiomeEnded();
                enabled = false;
            }
        }
    }
}

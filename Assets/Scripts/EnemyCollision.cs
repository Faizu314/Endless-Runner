using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float healthBar;

    private EnemySpawner enemySpawner;
    private EnemyMovement enemyMovement;
    private Material myCopyOfMaterial;
    private float currentHealthBar;

    private void Start()
    {
        currentHealthBar = healthBar;
        enemyMovement = GetComponent<EnemyMovement>();
        enemySpawner = GameObject.Find("Level Designer").GetComponent<EnemySpawner>();
        myCopyOfMaterial = new Material(material);
        myCopyOfMaterial.EnableKeyword("_Emission");
        myCopyOfMaterial.SetColor("_EmissionColor", Color.clear);
        meshRenderer.sharedMaterial = myCopyOfMaterial;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Light"))
        {
            currentHealthBar -= Time.deltaTime;
            myCopyOfMaterial.SetColor("_EmissionColor", Color.white);
            if (currentHealthBar <= 0)
                InflictDeath();
        }
    }

    private void FixedUpdate()
    {
        myCopyOfMaterial.SetColor("_EmissionColor", Color.clear);
    }

    private void InflictDeath()
    {
        myCopyOfMaterial.SetColor("_EmissionColor", Color.clear);
        currentHealthBar = healthBar;
        enemyMovement.ReportDeath();
        enemySpawner.Despawn(gameObject);
    }
}

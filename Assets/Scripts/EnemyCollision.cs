using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float health;

    private EnemySpawner enemySpawner;
    private EnemyMovement enemyMovement;
    private Material myCopyOfMaterial;
    private float currentHealth;

    private void Start()
    {
        currentHealth = health;
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
            currentHealth -= Time.fixedDeltaTime * SwipeControl.playerDamage;
            myCopyOfMaterial.SetColor("_EmissionColor", Color.white);
            enemyMovement.currentMovementSpeed = enemyMovement.movementSpeed * 0.75f;
            if (currentHealth <= 0)
                InflictDeath();
        }
    }

    private void FixedUpdate()
    {
        myCopyOfMaterial.SetColor("_EmissionColor", Color.clear);
        enemyMovement.currentMovementSpeed = enemyMovement.movementSpeed;
    }

    private void InflictDeath()
    {
        myCopyOfMaterial.SetColor("_EmissionColor", Color.clear);
        currentHealth = health;
        enemyMovement.ReportDeath();
        enemySpawner.Despawn(gameObject);
    }
}

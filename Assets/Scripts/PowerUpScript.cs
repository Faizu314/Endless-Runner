using UnityEngine;

public class PowerUpScript : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private float health;

    private PowerupSpawner powerupSpawner;
    private Transform player;
    private SwipeControl swipeControl;
    private Material myCopyOfMaterial;
    private float currentHealth;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        powerupSpawner = GameObject.Find("Powerups Manager").GetComponent<PowerupSpawner>();
        swipeControl = player.gameObject.GetComponent<SwipeControl>();
        myCopyOfMaterial = new Material(material);
        myCopyOfMaterial.EnableKeyword("_Emission");
        myCopyOfMaterial.SetColor("_EmissionColor", Color.clear);
        meshRenderer.sharedMaterial = myCopyOfMaterial;
        currentHealth = health;
    }

    private void Update()
    {
        transform.parent.transform.Translate(Vector3.back * Time.deltaTime * 0.5f);
        if (player.position.z - transform.position.z > 10f)
            InflictDeath(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Light"))
        {
            currentHealth -= Time.fixedDeltaTime;
            myCopyOfMaterial.SetColor("_EmissionColor", Color.white);
            if (currentHealth <= 0)
                InflictDeath(true);
        }
    }

    private void FixedUpdate()
    {
        myCopyOfMaterial.SetColor("_EmissionColor", Color.clear);
    }

    private void InflictDeath(bool eaten)
    {
        myCopyOfMaterial.SetColor("_EmissionColor", Color.clear);
        currentHealth = health;
        powerupSpawner.Despawn(transform.parent.gameObject);

        if (eaten)
        {
            if (gameObject.name == "Infinite Battery")
                swipeControl.InfiniteBattery();
            else if (gameObject.name == "More Damage")
                swipeControl.MoreDamage();
            else if (gameObject.name == "Longer Range")
                swipeControl.LongerRange();
        }
    }
}

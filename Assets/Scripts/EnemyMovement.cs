using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 0.5f)] private float frequency;
    [SerializeField] private AnimationCurve bobCurve;
    [Range(1, 5)] public float movementSpeed;

    [HideInInspector] public float currentMovementSpeed;
    private Transform target;
    private bool hasSpawnRoutineEnded = false;
    private bool hasSpawned = false;

    private void Awake()
    {
        target = GameObject.Find("Buddi").transform;
        currentMovementSpeed = movementSpeed;
    }

    private IEnumerator Spawn(float targetY)
    {
        float currentY = targetY + Time.deltaTime * 720;
        while (currentY < targetY + 360f)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, currentY, 0));
            currentY += Time.deltaTime * 720;

            if (currentY < targetY + 180f)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 2f, transform.position.z), Time.deltaTime * 2f);
                transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(1.2f, 1.2f, 1.2f), Time.deltaTime * 4f);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 1.69f, transform.position.z), Time.deltaTime * 2f);
                transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.one, Time.deltaTime * 4f);
            }

            yield return null;
        }
        hasSpawnRoutineEnded = true;
    }

    private void Update()
    {
        if (!hasSpawned)
        {
            StartCoroutine(Spawn(180));
            hasSpawned = true;
        }
        if (!hasSpawnRoutineEnded)
            return;
        
        float h = Time.time * frequency % 1f;
        float x = Time.time * frequency * 2f % 1f;
        float z = Time.time * frequency % 1f;

        float currentHeight = bobCurve.Evaluate(h);
        float xRotation = bobCurve.Evaluate(x);
        float zRotation = bobCurve.Evaluate(z);

        float rotationY = 180f;
        if (Vector3.SqrMagnitude(transform.position - target.position) < 400)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            rotationY = Mathf.Atan2(-dir.z, dir.x) * Mathf.Rad2Deg + 90f;
        }

        Vector3 movementBob = Vector3.zero;
        float imageSpeed = currentMovementSpeed * (1 + Mathf.Abs(currentHeight));
        movementBob.y = 1 + Mathf.Abs(currentHeight) * 0.5f;
        movementBob.x = transform.position.x + currentHeight * 0.002f;
        movementBob.z = transform.position.z;

        transform.position = Vector3.MoveTowards(transform.position, movementBob + transform.forward, imageSpeed * Time.deltaTime);

        Vector3 rotationBob = Vector3.zero;
        rotationBob.x = -xRotation * 8f;
        rotationBob.y = rotationY;
        rotationBob.z = zRotation * 8f;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotationBob), 16f * Time.deltaTime);
    }

    public void ReportDeath()
    {
        hasSpawnRoutineEnded = false;
        hasSpawned = false;
    }
}

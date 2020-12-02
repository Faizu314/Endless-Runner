using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float MovementSpeed;

    private bool moving = true;

    private void Update()
    {
        if (moving)
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.forward, Time.deltaTime * MovementSpeed);
    }

    public void Stop()
    {
        moving = false;
    }

    public void Move()
    {
        moving = true;
    }
}

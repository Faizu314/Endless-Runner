using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            //Debug.Break();
        }
    }
}

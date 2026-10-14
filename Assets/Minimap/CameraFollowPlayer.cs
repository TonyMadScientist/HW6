using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform player;

     void Update()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        newPosition.x = transform.position.x;
        newPosition.z = transform.position.z;
        transform.position = newPosition;
    }

}

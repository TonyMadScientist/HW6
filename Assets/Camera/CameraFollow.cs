using UnityEngine;

public class CameraFollow:MonoBehaviour
{
    public GameObject player;

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x,transform.position.y,player.transform.position.z-3f);
    }
}

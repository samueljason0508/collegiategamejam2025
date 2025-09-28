using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public PlayerControllerMain player; 
    public bool followY = true;

    void FixedUpdate()
    {
        if (player == null) return;
    
        Vector3 newPosition = transform.position;
    
        // Move the camera right at the same speed as the player
        float speed = player.autoScrollSpeed;
        newPosition.x += speed * Time.fixedDeltaTime;
    
        if (followY)
            newPosition.y = player.transform.position.y;
    
        transform.position = newPosition;
    }

}

using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public Transform[] backgroundSprites;
    public Camera cam;
    
    private float backgroundWidth;
    private Vector3 lastCameraPos;
    
    void Start()
    {
        backgroundWidth = backgroundSprites[0].GetComponent<SpriteRenderer>().bounds.size.x;
        lastCameraPos = cam.transform.position;
        
        // Make sure backgrounds start properly positioned
        backgroundSprites[0].position = new Vector3(0, backgroundSprites[0].position.y, backgroundSprites[0].position.z);
        backgroundSprites[1].position = new Vector3(backgroundWidth, backgroundSprites[1].position.y, backgroundSprites[1].position.z);
    }
    
    void Update()
    {
        Vector3 cameraMovement = cam.transform.position - lastCameraPos;
        
        // Move both backgrounds
        foreach (Transform bg in backgroundSprites)
        {
            bg.position -= new Vector3(cameraMovement.x, 0, 0);
        }
        
        // Check each background individually
        if (backgroundSprites[0].position.x < cam.transform.position.x - backgroundWidth)
        {
            backgroundSprites[0].position = new Vector3(backgroundSprites[1].position.x + backgroundWidth, backgroundSprites[0].position.y, backgroundSprites[0].position.z);
        }
        
        if (backgroundSprites[1].position.x < cam.transform.position.x - backgroundWidth)
        {
            backgroundSprites[1].position = new Vector3(backgroundSprites[0].position.x + backgroundWidth, backgroundSprites[1].position.y, backgroundSprites[1].position.z);
        }
        
        lastCameraPos = cam.transform.position;
    }
}
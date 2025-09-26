using UnityEngine;

public class CombinedBackgroundScroller : MonoBehaviour
{
    public Transform[] backgroundSprites; // All 4 backgrounds go here
    public Camera cam;
    
    private float backgroundWidth;
    private float backgroundHeight;
    private Vector3 lastCameraPos;
    
    void Start()
    {
        backgroundWidth = backgroundSprites[0].GetComponent<SpriteRenderer>().bounds.size.x;
        backgroundHeight = backgroundSprites[0].GetComponent<SpriteRenderer>().bounds.size.y;
        lastCameraPos = cam.transform.position;
        
        // Position all 4 backgrounds in a 2x2 grid
        backgroundSprites[0].position = new Vector3(0, 0, 0);                           // Bottom-left
        backgroundSprites[1].position = new Vector3(backgroundWidth, 0, 0);            // Bottom-right
        backgroundSprites[2].position = new Vector3(0, backgroundHeight, 0);           // Top-left
        backgroundSprites[3].position = new Vector3(backgroundWidth, backgroundHeight, 0); // Top-right
    }
    
    void Update()
    {
        Vector3 cameraMovement = cam.transform.position - lastCameraPos;
        
        // Move all backgrounds
        foreach (Transform bg in backgroundSprites)
        {
            bg.position -= new Vector3(cameraMovement.x, cameraMovement.y, 0);
        }
        
        // Loop each background when it goes off screen
        for (int i = 0; i < backgroundSprites.Length; i++)
        {
            // Horizontal looping
            if (backgroundSprites[i].position.x < cam.transform.position.x - backgroundWidth)
            {
                backgroundSprites[i].position = new Vector3(backgroundSprites[i].position.x + (backgroundWidth * 2), backgroundSprites[i].position.y, backgroundSprites[i].position.z);
            }
            if (backgroundSprites[i].position.x > cam.transform.position.x + backgroundWidth)
            {
                backgroundSprites[i].position = new Vector3(backgroundSprites[i].position.x - (backgroundWidth * 2), backgroundSprites[i].position.y, backgroundSprites[i].position.z);
            }
            
            // Vertical looping (both directions)
            if (backgroundSprites[i].position.y < cam.transform.position.y - backgroundHeight)
            {
                backgroundSprites[i].position = new Vector3(backgroundSprites[i].position.x, backgroundSprites[i].position.y + (backgroundHeight * 2), backgroundSprites[i].position.z);
            }
            if (backgroundSprites[i].position.y > cam.transform.position.y + backgroundHeight)
            {
                backgroundSprites[i].position = new Vector3(backgroundSprites[i].position.x, backgroundSprites[i].position.y - (backgroundHeight * 2), backgroundSprites[i].position.z);
            }
        }
        
        lastCameraPos = cam.transform.position;
    }
}
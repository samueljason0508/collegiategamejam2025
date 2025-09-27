using UnityEngine;
using System.Collections.Generic;

public class EntitySpawner : MonoBehaviour
{
    public GameObject entityPrefab;
    public Camera cam;
    public Transform[] backgroundSprites;
    public Transform player;
    
    [Header("Spawn Settings")]
    public int maxEntities;              
    [Range(0f, 1f)]
    public float spawnChancePerTick;  
    public float minSpawnDistance;     
    public float startDelay;
    
    private List<GameObject> currentEntities = new List<GameObject>();
    private float backgroundWidth;
    private float backgroundHeight;
    private float gameStartTime;
    
    void Start()
    {
        backgroundWidth = backgroundSprites[0].GetComponent<SpriteRenderer>().bounds.size.x;
        backgroundHeight = backgroundSprites[0].GetComponent<SpriteRenderer>().bounds.size.y;
        gameStartTime = Time.time;
    }
    
    void Update()
    {
        // Don't do anything until start delay seconds have passed
        if (Time.time - gameStartTime < startDelay) return;
        
        // Clean up destroyed entities from list
        currentEntities.RemoveAll(entity => entity == null);
        
        // for each entity check if entities are outside the 4 background frames
        for (int i = currentEntities.Count - 1; i >= 0; i--)
        {
            if (currentEntities[i] != null)
            {
                Vector3 entityPos = currentEntities[i].transform.position;
                Vector3 cameraPos = cam.transform.position;
                
                float leftBound = cameraPos.x - backgroundWidth;
                float rightBound = cameraPos.x + backgroundWidth;
                float bottomBound = cameraPos.y - backgroundHeight;
                float topBound = cameraPos.y + backgroundHeight;
                
                if (entityPos.x < leftBound || entityPos.x > rightBound || 
                    entityPos.y < bottomBound || entityPos.y > topBound)
                {
                    Destroy(currentEntities[i]);
                    currentEntities.RemoveAt(i);
                }
            }
        }
        
        // Try to spawn new entity
        if (currentEntities.Count < maxEntities && Random.value < spawnChancePerTick)
        {
            SpawnEntity();
        }
    }
    
    void SpawnEntity()
    {   
        Vector3 playerPos = player.transform.position;
        Vector3 cameraPos = cam.transform.position;
    
        // Calculate background bounds
        float backgroundLeftBound = cameraPos.x - backgroundWidth;
        float backgroundRightBound = cameraPos.x + backgroundWidth;
        float backgroundBottomBound = cameraPos.y - backgroundHeight;
        float backgroundTopBound = cameraPos.y + backgroundHeight;
    
        // Must be at least couple units to the right of player (ahead of player)
        float minSpawnX = playerPos.x + minSpawnDistance;  
    
        // Ensure we're spawning ahead of the player AND within background bounds
        float actualMinX = Mathf.Max(backgroundLeftBound, minSpawnX);
        float actualMaxX = backgroundRightBound;
    
        if (actualMinX <= playerPos.x || actualMinX >= actualMaxX) return;
    
        float randomX = Random.Range(actualMinX, actualMaxX);
        float randomY = Random.Range(backgroundBottomBound, backgroundTopBound);
    
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0);
    
        GameObject newEntity = Instantiate(entityPrefab, spawnPosition, Quaternion.identity);
        currentEntities.Add(newEntity);
    }
}
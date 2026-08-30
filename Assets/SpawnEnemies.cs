using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject normalEnemy;      
    public GameObject pintuEnemy;       
    public GameObject spikeEnemy;       
    public GameObject directionEnemy;
    public GameObject abhinavEnemy;  

    [Header("Spawn Settings")]
    public float spawnPadding = 2f;
    public float despawnPadding = 5f;
    public float minY = 10f;
    public float minDistBetweenEnemies = 3f;

    [Header("Booster Density")]
    public float boosterSpawnInterval = 0.1f;
    public int initialBoosterCount = 30;

    [Header("Special Enemy Spawn Intervals")]
    public float pintuSpawnInterval = 4f;    
    public float spikeSpawnInterval = 3f;    

    [Header("Spike + Direction Pair")]
    public float pairMinOffset = 3f;         
    public float pairMaxOffset = 5f;         

    private float boosterTimer;
    private float pintuTimer;
    private float abhinavTimer;
    public float abhinavSpawnInterval = 4f;
    private float spikeTimer;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        for (int i = 0; i < initialBoosterCount; i++)
            SpawnOffScreen(normalEnemy);

        
        SpawnOffScreen(pintuEnemy);
        SpawnOffScreen(pintuEnemy);
        SpawnOffScreen(abhinavEnemy);    
        SpawnSpecialOffScreen(); 
    }

    private void Update()
    {
        
        boosterTimer -= Time.deltaTime;
        if (boosterTimer <= 0f)
        {
            boosterTimer = boosterSpawnInterval;
            SpawnOffScreen(normalEnemy);
        }

        // Pintu 
        pintuTimer -= Time.deltaTime;
        if (pintuTimer <= 0f)
        {
            pintuTimer = pintuSpawnInterval + Random.Range(-1f, 1f); // randomness
            SpawnOffScreen(pintuEnemy);
        }

        
        abhinavTimer -= Time.deltaTime;
        if (abhinavTimer <= 0f)
        {
            abhinavTimer = abhinavSpawnInterval + Random.Range(-1f, 1f);
            SpawnOffScreen(abhinavEnemy);
        }

        // Spike
        spikeTimer -= Time.deltaTime;
        if (spikeTimer <= 0f)
        {
            spikeTimer = spikeSpawnInterval + Random.Range(-0.5f, 0.5f);
            SpawnSpecialOffScreen();
        }

        DespawnFarEnemies();
    }

    // Spawn spike
    private void SpawnSpecialOffScreen()
    {
        Vector2 spikePos = GetOffScreenPosition();
        if (spikePos == Vector2.zero) return;

        Instantiate(spikeEnemy, spikePos, Quaternion.identity);

        // 50/50
        if (Random.value > 0.5f)
        {
            // random angl
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float offset = Random.Range(pairMinOffset, pairMaxOffset);
            Vector2 dirPos = spikePos + new Vector2(Mathf.Cos(angle) * offset,
                                                     Mathf.Sin(angle) * offset);

            // the direction enemy isn't below ground
            if (dirPos.y >= minY)
                Instantiate(directionEnemy, dirPos, Quaternion.identity);
        }
    }

    private void SpawnOffScreen(GameObject prefab)
    {
        Vector2 pos = GetOffScreenPosition();
        if (pos == Vector2.zero) return;
        Instantiate(prefab, pos, Quaternion.identity);
    }

    //  random position 
    private Vector2 GetOffScreenPosition()
    {
        Vector2 half = GetCameraHalfExtents();
        Vector2 cam = mainCamera.transform.position;

        float x, y;
        int edge = Random.Range(0, 4);

        switch (edge)
        {
            case 0: // Left
                x = cam.x - half.x - spawnPadding;
                y = RandomY(cam, half);
                break;
            case 1: // Right
                x = cam.x + half.x + spawnPadding;
                y = RandomY(cam, half);
                break;
            case 2: // Top
                x = Random.Range(cam.x - half.x, cam.x + half.x);
                y = cam.y + half.y + spawnPadding;
                break;
            default: // Bottom
                x = Random.Range(cam.x - half.x, cam.x + half.x);
                y = cam.y - half.y - spawnPadding;
                break;
        }

        if (y < minY) return Vector2.zero; // Reject below ground

        Vector2 candidate = new Vector2(x, y);
        if (TooCloseToOtherEnemy(candidate)) return Vector2.zero;

        return candidate;
    }

    private float RandomY(Vector2 cam, Vector2 half)
    {
        float low = Mathf.Max(minY, cam.y - half.y - spawnPadding);
        float high = cam.y + half.y + spawnPadding;
        return Mathf.Lerp(low, high, (Random.value + Random.value) / 2f);
    }

    private void DespawnFarEnemies()
    {
        Vector2 half = GetCameraHalfExtents();
        Vector2 cam = mainCamera.transform.position;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;
            Vector2 pos = enemy.transform.position;
            if (pos.x < cam.x - half.x - despawnPadding ||
                pos.x > cam.x + half.x + despawnPadding ||
                pos.y < cam.y - half.y - despawnPadding ||
                pos.y > cam.y + half.y + despawnPadding)
            {
                Destroy(enemy);
            }
        }
    }

    private bool TooCloseToOtherEnemy(Vector2 pos)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in enemies)
        {
            if (e != null && Vector2.Distance(e.transform.position, pos) < minDistBetweenEnemies)
                return true;
        }
        return false;
    }

    private Vector2 GetCameraHalfExtents()
    {
        float orthoSize = mainCamera.orthographicSize;
        return new Vector2(orthoSize * mainCamera.aspect, orthoSize);
    }
}
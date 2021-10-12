using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemies = GameObject.FindGameObjectsWithTag("Hittable").ToList();
        
        if (instance != null)
            return;
        instance = this;

        Lean.Pool.LeanPool.Preload(enemyPrefabs[0], 100);
        Lean.Pool.LeanPool.Preload(enemyPrefabs[1], 100);
        Lean.Pool.LeanPool.Preload(bulletPrefab, 50);
        //ObjectPooling.Preload(enemyPrefabs[0], 100);
        //ObjectPooling.Preload(enemyPrefabs[1], 100);
        //ObjectPooling.Preload(bulletPrefab, 50);

    }
    #endregion
    
    [Header("Flowers")]
    public GameObject flowerPrefab;
    [Tooltip("If set to -1, random value between 1 and 10")]
    public float flowerSpawnRateInSeconds;
    [Tooltip("Set value between 1 and +inf")]
    public float flowerSpawnWidth;
    [Tooltip("Set value between 1 and +inf")]
    public float flowerSpawnHeight;
    public float flowerDistance;
    public int flowerMaxAmount;
    public List<GameObject> flowers = new List<GameObject>();

    [Header("Enemies")]
    public List<GameObject> enemyPrefabs;
    public List<GameObject> enemies = new List<GameObject>();
    [Header("Enemy spawn")]
    public float enemySpawnRateInSeconds;
    [Tooltip("Set value between 1 and +inf")]
    public float enemySpawnWidth;
    [Tooltip("Set value between 1 and +inf")]
    public float enemySpawnHeight;
    public float enemyDistance;
    public int enemyMaxAmount;
    public float enemySpawnXOffset;
    public float enemySpawnYOffset;
    [Header("Enemy shooter spawn")]
    public int initialShooterSpawnChance;
    private int shooterSpawnChance;

   
    [Tooltip("Bonus chance added to default every picked flower")]
    public int bonusSpawnChance;
    
    
    [Header("Enemies clear")] 
    public float enemyClearInterval;
    public float enemyClearDistance;
    [Header("Player")] 
    public GameObject newPlayerPrefab;
    public GameObject player;
    public GameObject swordTether;
    public int flowersPickedUp;
    public int killedEnemies;
    public bool playerInvicible;
    public Vector2 lastPlayerPosition;
    [Header("Sword sensitivity")] 
    public float slowSense;
    public float mediumSense;
    public float fastSense;
    [Header("Tutorial")] 
    public GameObject preTutorialPanel;
    public float preTutorialDuration;
    public GameObject tutorialPanel;
    public float tutorialDuration;
    [Header("UI")] 
    public TMP_Text flowersPickedUpText;
    public TMP_Text flowersHighscoreText;
    public TMP_Text enemiesKilledText;
    public TMP_Text enemiesHighscoreText;
    public GameObject gamePausedPanel;
    public GameObject gamePlayingPanel;
    public GameObject gameStartPanel;

    public GameObject swordFace;
    public Sprite angryFace;
    [Header("Game state")] public GameState gameState;
    private bool firstRun;
    [Header("Pooling")] public GameObject bulletPrefab;
    
    public enum GameState
    {
        NEWGAME,
        PLAYING,
        PAUSED,
        END
    }
    void Start()
    {
        firstRun = true;
        gameState = GameState.NEWGAME;
        gameStartPanel.SetActive(true);
        gamePlayingPanel.SetActive(false);
        swordTether.SetActive(false);

        FMODUnity.RuntimeManager.PlayOneShot("event:/BGMusic");
    }

    void SetInvokes()
    {
        if (flowerSpawnRateInSeconds == -1)
            InvokeRepeating("SpawnFlower", 0f, Random.Range(1, 10));
        else
            InvokeRepeating("SpawnFlower", 0f, flowerSpawnRateInSeconds);
        InvokeRepeating("SpawnEnemy", 0f, enemySpawnRateInSeconds);
        InvokeRepeating("RemoveFarEnemies", 0f, enemyClearInterval);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && gameState == GameState.NEWGAME)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/UIClick");
            gameStartPanel.SetActive(false);
            StartCoroutine(ShowPreTutorial());
            RemovePlayer();
        }
    }

    private IEnumerator ShowPreTutorial()
    {
        preTutorialPanel.SetActive(true);
        yield return new WaitForSeconds(preTutorialDuration);
        preTutorialPanel.SetActive(false);
    }
    private IEnumerator ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        yield return new WaitForSeconds(tutorialDuration);
        tutorialPanel.SetActive(false);
    }
    void RemoveFarEnemies()
    {
        if (gameState != GameState.PLAYING)
            return;
        List<GameObject> enemiesToRemove = new List<GameObject>();

        foreach (GameObject enemy in enemies)
        {
            if (Vector2.Distance(enemy.transform.position, player.transform.position) > enemyClearDistance)
            {
                enemiesToRemove.Add(enemy);
            }
        }

        //Debug.Log("Clearing " + enemiesToRemove.Count + " enemies.");
        foreach (GameObject enemy in enemiesToRemove)
        {
            RemoveEnemy(enemy);
        }

    }
    
    Vector2 GenerateSpawnPos(float width, float height, float offsetX, float offsetY)
    {
        bool onLeft = Random.Range(0, 100) > 50;
        bool onUp = Random.Range(0, 100) > 50;
        List<Vector2> spawnPositions = new List<Vector2>();
        if (onLeft)
        {
            spawnPositions.Add(new Vector2(Random.Range(-width + 1f, 0f), Random.Range(-height+1f, height)));
        }
        else
        {
            spawnPositions.Add(new Vector2(Random.Range(1f, width), Random.Range(-height+1f, height)));
        }
        if(onUp)
        {
            spawnPositions.Add(new Vector2(Random.Range(-width+1f, width), Random.Range(1f, height)));
        }
        else
        {
            spawnPositions.Add(new Vector2(Random.Range(-width+1f, width), Random.Range(-height + 1f, 0f)));
        }

        return spawnPositions[Random.Range(0, spawnPositions.Count)];
    }
    
    void SpawnFlower()
    {
        if (gameState != GameState.PLAYING)
            return;
        if (flowers.Count >= flowerMaxAmount)
            return;
        bool goodDistance = true;
        
        Vector2 flowerPos = Camera.main.ViewportToWorldPoint(GenerateSpawnPos(flowerSpawnWidth, flowerSpawnHeight, 0f, 0f));
        
        foreach (GameObject flowerList in flowers)
        {
            if (Vector2.Distance(flowerList.transform.position, flowerPos) < flowerDistance)
                goodDistance = false;
        }

        if (goodDistance)
        {
            GameObject flower = Instantiate(flowerPrefab, flowerPos, Quaternion.identity);
            flowers.Add(flower);
        }
    }


    void SpawnNewPlayer()
    {
        Vector2 enemyPos = Camera.main.ViewportToWorldPoint(GenerateSpawnPos(1.2f, 1.2f, 0f, 0f));
        
        player = Instantiate(newPlayerPrefab, enemyPos, Quaternion.identity);
    }
    void SpawnEnemy()
    {
        if (gameState != GameState.PLAYING)
            return;
        if (enemies.Count >= enemyMaxAmount)
            return;
        bool goodDistance = true;

        Vector2 enemyPos = Camera.main.ViewportToWorldPoint(GenerateSpawnPos(enemySpawnWidth, enemySpawnHeight, enemySpawnXOffset, enemySpawnYOffset));
        
        GameObject enemy;
        if (Random.Range(0, 100) > 100 - shooterSpawnChance)
            enemy = Lean.Pool.LeanPool.Spawn(enemyPrefabs[1]);
        else
            enemy = Lean.Pool.LeanPool.Spawn(enemyPrefabs[0]);
        enemy.transform.position = enemyPos;
        enemies.Add(enemy);
        
    }

    public void RemoveFlower(GameObject flower)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/FlowerPickup");
        SFXSpawner.instance.SpawnHeartExplosion(flower.transform.position);
        flowersPickedUp += 1;
        flowersPickedUpText.text = flowersPickedUp.ToString();
        shooterSpawnChance += bonusSpawnChance;
        flowers.Remove(flower);
        Destroy(flower);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
        Lean.Pool.LeanPool.Despawn(enemy);
        //Destroy(enemy);
    }

    public void KillEnemy(GameObject enemy)
    {
        killedEnemies += 1;
        enemiesKilledText.text = killedEnemies.ToString();
        enemies.Remove(enemy);
        Lean.Pool.LeanPool.Despawn(enemy);
        //Destroy(enemy);
    }

    public void RemovePlayer()
    {
        flowersHighscoreText.text = Mathf.Max(flowersPickedUp, int.Parse(flowersHighscoreText.text)).ToString();
        enemiesHighscoreText.text = Mathf.Max(killedEnemies, int.Parse(enemiesHighscoreText.text)).ToString();
        gamePlayingPanel.SetActive(true);
        CancelInvoke();
        swordTether.SetActive(false);
        if(player!=null)
            lastPlayerPosition = player.transform.position;
        gameState = GameState.END;

        flowersPickedUp = 0;
        killedEnemies = 0;
        
        flowersPickedUpText.text = flowersPickedUp.ToString();
        enemiesKilledText.text = killedEnemies.ToString();
        
        shooterSpawnChance = initialShooterSpawnChance;
        SpawnNewPlayer();
        //flowersPickedUpEndText.text = flowersPickedUp.ToString();
        
        //gamePausedPanel.SetActive(false);
        //gameEndPanel.SetActive(true);
    }

    public void NewPlayerFoundSword()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SwordPickup");
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("BGMType", 1);
        if (firstRun)
        {
            StartCoroutine(ShowTutorial());
            firstRun = false;
        }
        swordFace.GetComponent<SpriteRenderer>().sprite = angryFace;
        swordTether.SetActive(true);
        gameState = GameState.PLAYING;
        SetInvokes();
    }
    public void PauseGame()
    {
        if (gameState == GameState.PAUSED && Time.timeScale == 0)
        {
            Time.timeScale = 1;
            gamePausedPanel.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            gameState = GameState.PLAYING;
        }
        else if (gameState == GameState.PLAYING && Time.timeScale == 1)
        {
            Time.timeScale = 0;
            gamePausedPanel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            gameState = GameState.PAUSED;
        }
    }

    public void OnRetryClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetSlowSense()
    {
        FindObjectOfType<CursedWeaponController>().speedScale = slowSense;
    }

    public void SetMediumSense()
    {
        FindObjectOfType<CursedWeaponController>().speedScale = mediumSense;
    }

    public void SetFastSense()
    {
        FindObjectOfType<CursedWeaponController>().speedScale = fastSense;
    }
}

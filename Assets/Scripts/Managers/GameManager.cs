using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static bool m_isGameOver = false;
    private static bool m_gameOverCheck = false; // Used to make sure OnGameOver is only called once.

    public static bool m_hasGameStarted = false; // I know it's weird to have both isGameOver and hasGameStarted but whatever lol.

    [Header("Player Points")]
    public int m_points;

    [Header("Fish Types")]
    public GameObject m_smallFish;
    public GameObject m_mediumFish;
    public GameObject m_largeFish;

    [Header("Spawn Zones")]
    public BoxCollider m_fishSpawnZone;

    [Header("Fish Spawning Numbers")]
    public float m_spawnRate = 5;
    private float m_spawnCounter = 0;
    private bool m_spawnCooldown = false;

    [Header("Extra Canvas Things")]
    public Canvas m_gameOverCanvas;
    public Canvas m_startGameCanvas;

    public Button m_startGameButton; 
    public static Button m_startGameButtonS; // Static version to be accessed within the PlayerController.cs script.

    public Button m_retryButton;
    public static Button m_retryButtonS;

    [Header("Other Manager References")]
    public PlayerManager m_playerManager;
    public CombatManager m_combatManager;

    // Highscore stuff.
    [HideInInspector]
    public static int m_currentScore = 0; // Current high score for the session.
    [HideInInspector]
    public static int m_highScore = 0; // loaded in from previous session.


	private void Awake()
	{
        //m_startGameButtonS = m_startGameButton;
        //m_retryButtonS = m_retryButton;
    }

	private void Start()
	{

        Debug.Log("Default save location: " + Application.persistentDataPath);

       

        FileManager.Load();

        m_startGameButtonS = m_startGameButton;
        m_retryButtonS = m_retryButton;

        GameEvents.gameEvents.OnScoreEvent += AddPoints;
	}


	// Update is called once per frame
	void Update()
    {
        if (m_hasGameStarted)
        { 
            if (m_spawnCooldown)
            {
                m_spawnCounter += Time.deltaTime;
                if (m_spawnCounter >= m_spawnRate)
                {
                    // Enough time has elapsed to reset the spawn cooldown.
                    m_spawnCooldown = false;
                    m_spawnCounter = 0;
                }
            }

            if (!m_spawnCooldown && !m_isGameOver)
            {
                // We can spawn a fish!
                GameObject fish = GenerateRandomFish();
                SpawnFish(fish, m_fishSpawnZone);
            }


            // Handling game over stuff
            if (m_isGameOver && !m_gameOverCheck)
            {
                m_gameOverCheck = true; // Setting this to true will prevent this from getting called again. So it will only be called once.
                OnGameOver();
            }
        }


    }


    /// <summary>
    /// GenerateRandomFish() will pick a number from 0 - 2. 0 - small fish, 1 - medium fish, 2 - large fish.
    /// </summary>
    /// <returns></returns>
    GameObject GenerateRandomFish()
    {
        int num = Random.Range(0, 3); // The 3 is exlusive, so this is a range between 0-2.
        switch (num)
        {
            case 0:
                return m_smallFish;
              
            case 1:
                return m_mediumFish;

            case 2:
                return m_largeFish;

            default:
                return null;
                Debug.Assert(true); // This default case should never happen.
        }

        

    }

    void SpawnFish(GameObject fish, BoxCollider spawnZone)
    {
        float xExtent = spawnZone.bounds.extents.x;
        float yExtent = spawnZone.bounds.extents.y;
        float zExtent = spawnZone.bounds.extents.z;

        Vector3 spawnLocation = new Vector3(
            Random.Range(spawnZone.transform.position.x - xExtent, spawnZone.transform.position.x + xExtent),
            Random.Range(spawnZone.transform.position.y - yExtent, spawnZone.transform.position.y + yExtent),
            Random.Range(spawnZone.transform.position.z - zExtent, spawnZone.transform.position.z + zExtent)
            );

        GameObject newFish = Instantiate(fish);
        newFish.transform.position = spawnLocation;

        newFish.transform.rotation = Quaternion.Euler(newFish.transform.rotation.x, 180, newFish.transform.rotation.z); // Eh the test fish asset is rotated the wrong way, so I'll just dodgily fix that here.

        m_spawnCooldown = true;
    }

    /// <summary>
    /// OnGameOver() call things that you would like to occur when the game is over here.
    /// </summary>
    void OnGameOver()
    {
        // If they shot a torpedo, lets reset the cooldown since we want them to start completely fresh.
        m_combatManager.m_torpedoGunCounter = 0.0f;
        m_combatManager.m_isTorpedoCooldown = false;

        m_gameOverCanvas.gameObject.SetActive(true);
        FileManager.Save();
        if (m_currentScore > m_highScore)
            m_highScore = m_currentScore;
    }

    public static void EndGame()
    {
        m_isGameOver = true;
  
    }

    public static void AddPoints(int points)
    {
        m_currentScore += points;
    }


    public void StartGame()
    {
        m_hasGameStarted = true;
        m_startGameCanvas.gameObject.SetActive(false);

        Debug.Log("Game Started");
    }

    /// <summary>
    /// RestartGame() has no references because it is called as a callback function on a UI button.
    /// </summary>
    public void RestartGame()
    {
        m_hasGameStarted = true;
        m_gameOverCanvas.gameObject.SetActive(false);

        Debug.Log("Game restarted.");

        m_isGameOver = false;
        m_gameOverCheck = false;

        m_currentScore = 0;

        m_playerManager.m_playerHealth = m_playerManager.m_maxPlayerHealth;


        

        GameObject[] m_allEnemies = GameObject.FindGameObjectsWithTag("Enemy"); // Clearing all current enemies on the screen.
        for (int i = 0; i < m_allEnemies.Length; i++)
        {
            Destroy(m_allEnemies[i]);
        }
    }

    
}

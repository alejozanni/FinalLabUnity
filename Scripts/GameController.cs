using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState { Idle, Playing, Ended, Ready };

public class GameController : MonoBehaviour
{
    [Range(0f, 0.2f)]
    public float parallaxSpeed = 0.024f;
    public RawImage background;
    public RawImage platform;
    public GameObject uiIdle;
    public GameObject scoreIdle;

    public Text pointsText;
    public Text recordText;

    public GameState gameState = GameState.Idle;

    public GameObject player;
    public GameObject enemyGenerator;

    public float scaleTime = 6f;
    public float scaleInc = .25f;

    private AudioSource musicPlayer;
    private int points;

    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
        recordText.text = "Best score: " + GetMaxScore().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        bool userAction = Input.GetKeyDown("up") || Input.GetMouseButtonDown(0);

        // Empieza el juego
        if (gameState == GameState.Idle && userAction)
        {
            // Juego detenido
            gameState = GameState.Playing;
            uiIdle.SetActive(false);
            scoreIdle.SetActive(true);
            player.SendMessage("UpdateState", "PlayerRun");
            enemyGenerator.SendMessage("StartGenerator");
            musicPlayer.Play();

            InvokeRepeating("GameTimeScale", scaleTime, scaleTime);

        } else if (gameState == GameState.Playing) {
            
            // Juego en marcha
            Parallax();

        } else if (gameState == GameState.Ready) {
            
            // Muerte del personaje
            if (userAction)
            {
                RestartGame();
            }
        
        }
    }

    void Parallax ()
    {
        float finalSpeed = parallaxSpeed * Time.deltaTime;
        background.uvRect = new Rect(background.uvRect.x + finalSpeed, 0f, 1f, 1f);
        platform.uvRect = new Rect(platform.uvRect.x + finalSpeed * 4, 0f, 1f, 1f);
    }

    public void RestartGame()
    {
        ResetTimeScale();
        SceneManager.LoadScene("Principal");
    }

    void GameTimeScale()
    {
        Time.timeScale += scaleInc;
        Debug.Log("Ritmo incrementado" + Time.timeScale.ToString());
    }

    public void ResetTimeScale(float newTimeScale = 1f)
    {
        CancelInvoke("GameTimeScale");
        Time.timeScale = newTimeScale;
        Debug.Log("Reset ritmo" + Time.timeScale.ToString());
    }

    public void IncreasePoints()
    {
        points++;
        pointsText.text = points.ToString();

        if (points >= GetMaxScore())
        {
            recordText.text = "Best score: " + points.ToString();
            SaveScore(points);
        }
    }

    public int GetMaxScore()
    {
        return PlayerPrefs.GetInt("Max Points", 0);
    }

    public void SaveScore(int currentPoints)
    {
        PlayerPrefs.SetInt("Max Points", currentPoints);
    }
}
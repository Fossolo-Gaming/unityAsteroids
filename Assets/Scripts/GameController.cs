using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    // game stats
    private int level = 1;
    private int lives = 3;
    private int bombs = 3;
    private int score = 0;

    public GameObject background;
    public GameObject splashScreen;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText;

    public TextMeshProUGUI gameStatsText;

    public AudioSource startSound;
    public AudioSource gameOverSound;
    public AudioSource victorySound;

    public GameObject player;
    public PlayerController playerController;
    public AsteroidsController asteroids;
    public PowerUpsController powerUps;

    private int maxLevel;

    private Rect screenRect;

    static public GameController GetGameController()
    {
        return (GameController)FindObjectOfType(typeof(GameController));
    }

    private void Awake()
    {
        playerController = player.GetComponent<PlayerController>();
        maxLevel = asteroids.numAsteroids.Count;

    }

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer renderer = background.GetComponent<SpriteRenderer>();
        float w = renderer.size.x;
        float h = renderer.size.y;
        float x = background.transform.position.x - 0.5f * w;
        float y = background.transform.position.y - 0.5f * h;
        screenRect = new Rect(x, y, w, h);

        RestartLevel();
    }

    public int Bombs
    {
        get { return bombs; }
        set { bombs = value; UpdateGameStats(); }
    }

    public int Score
    { 
        get { return score;  }
        set { score = value; UpdateGameStats(); }
    }

    public int Lives
    {
        get { return lives;  }
        set { lives = value; UpdateGameStats(); }
    }

    public int Level
    {
        get { return level;  }
    }

    public void RestartLevel()
    {
        ClearLevel();

        SplashScreen("Level " + level.ToString(), startSound);
        StartCoroutine(StartNewLevel(2.0f));
    }

    public void ClearLevel()
    {
        player.SetActive(false);
        asteroids.DestroyAsteroids();
        powerUps.Deactivate();
    }

    public Vector3 GetRandomPosition()
    {
        float x = Random.Range(screenRect.xMin, screenRect.xMax);
        float y = Random.Range(screenRect.yMin, screenRect.yMax);
        return  new UnityEngine.Vector3((float)x, (float)y, 0.0f);
    }

    public void LevelCompleted()
    {
        level += 1;
        if (level < maxLevel)
        {
            RestartLevel();
        }
        else
        {
            Victory();
        }
    }

    public void PlayerDeath()
    {
        --lives;
        if(lives>0)
        {
            RestartLevel();
        }
        else
        {
            GameOver();
        }        
    }

    void GameOver()
    {
        ClearLevel();
        SplashScreen("Game Over!", gameOverSound);
    }    

    void Victory()
    {
        ClearLevel();
        SplashScreen("Victory!", victorySound);
    }

    void SplashScreen(string text,AudioSource sound)
    {
        player.SetActive(false);
        splashScreen.SetActive(true);
        gameStatsText.gameObject.SetActive(false);
        levelText.text = text;
        scoreText.text = "Score: " + score.ToString();
        sound.Play();
    }

    void UpdateGameStats()
    {
        gameStatsText.text = "Level " + level.ToString() + " Lives " + lives.ToString() + " Bombs " + bombs.ToString() + " Score " + score.ToString();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator StartNewLevel(float timer)
    {
        yield return new WaitForSeconds(timer);
        splashScreen.SetActive(false);
        gameStatsText.gameObject.SetActive(true);
        UpdateGameStats();
        player.SetActive(true);
        playerController.Restart();
        asteroids.CreateAsteroids();
        powerUps.Activate();
    }

}

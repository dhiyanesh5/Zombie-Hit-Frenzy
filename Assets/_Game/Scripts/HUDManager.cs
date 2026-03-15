using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI fpsText;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    // FPS tracking
    private float fpsTimer;
    private int frameCount;
    private float currentFPS;

    private void Start()
    {
        gameOverPanel.SetActive(false);

        // Subscribe to GameManager events
        GameManager.Instance.onScoreChanged.AddListener(UpdateScore);
        GameManager.Instance.onTimerChanged.AddListener(UpdateTimer);
        GameManager.Instance.onGameOver.AddListener(ShowGameOver);
    }

    private void Update()
    {
        UpdateFPS();
    }

    private void UpdateScore(int score)
    {
        scoreText.text = $"<size=70%>SCORE</size>\n<size=150%>{score}</size>";
    }

    private void UpdateTimer(float timeLeft)
    {
        int seconds = Mathf.CeilToInt(timeLeft);
        timerText.text = $"<size=70%>TIMER</size>\n<size=150%>{seconds}</size>";

        // Turn red when under 10 seconds
        timerText.color = seconds <= 10 ? Color.red : Color.white;
    }

    private void UpdateFPS()
    {
        frameCount++;
        fpsTimer += Time.deltaTime;

        if (fpsTimer >= 0.5f)
        {
            currentFPS = frameCount / fpsTimer;
            fpsText.text = $"<size=70%>FPS</size>\n<size=150%>{Mathf.RoundToInt(currentFPS)}</size>";
            frameCount = 0;
            fpsTimer = 0f;
        }
    }

    private void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = "" + GameManager.Instance.Score;
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
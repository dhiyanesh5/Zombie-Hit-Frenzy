using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 60f;

    [Header("Events")]
    public UnityEvent<int> onScoreChanged;
    public UnityEvent<float> onTimerChanged;
    public UnityEvent onGameOver;

    public int Score { get; private set; }
    public float TimeLeft { get; private set; }
    public bool IsPlaying { get; private set; }

    private void Awake()
    {
        Application.targetFrameRate = 75;
        Instance = this;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (!IsPlaying) return;

        TimeLeft -= Time.deltaTime;
        onTimerChanged?.Invoke(TimeLeft);

        if (TimeLeft <= 0f)
        {
            TimeLeft = 0f;
            EndGame();
        }
    }

    public void StartGame()
    {
        Score = 0;
        TimeLeft = gameDuration;
        IsPlaying = true;
        onScoreChanged?.Invoke(Score);
        onTimerChanged?.Invoke(TimeLeft);
    }

    public void AddScore(int points = 1)
    {
        if (!IsPlaying) return;
        Score += points;
        onScoreChanged?.Invoke(Score);
    }

    private void EndGame()
    {
        IsPlaying = false;
        onGameOver?.Invoke();
        Debug.Log("GAME OVER! Final score: " + Score);
    }
}
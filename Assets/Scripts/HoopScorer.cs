using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class HoopScorer : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] private string hoopEntryTag = "HoopEntry";
    [SerializeField] private string hoopExitTag = "HoopExit";

    [Header("UI References")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;

    [Header("Timing")]
    [SerializeField] private float missDelay = 1.5f;
    [SerializeField] private float gameDuration = 120f;

    private bool enteredHoop = false;
    private bool hasScored = false;
    private Coroutine missCoroutine;
    private Coroutine timerCoroutine;
    private int currentScore = 0;
    private int highScore = 0;
    private bool gameRunning = false;

    private string[] missMessages = new string[]
    {
        "Just missed!",
        "So close!",
        "Almost had it!",
        "Nice try!",
        "You'll get it next time!"
    };

    private void Awake()
    {
        feedbackText.text = "";
        scoreText.text = "Score: 0";
        timerText.text = "Time: 120";

        startButton.onClick.AddListener(StartGame);
        stopButton.onClick.AddListener(StopGame);

        startPanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gameRunning || hasScored) return;

        if (other.CompareTag(hoopEntryTag))
        {
            enteredHoop = true;
            missCoroutine = StartCoroutine(HandlePossibleMiss());
        }
        else if (enteredHoop && other.CompareTag(hoopExitTag))
        {
            if (missCoroutine != null)
                StopCoroutine(missCoroutine);

            hasScored = true;
            ScorePoint();
            ResetFlags();
        }
    }

    private IEnumerator HandlePossibleMiss()
    {
        yield return new WaitForSeconds(missDelay);

        if (!hasScored)
        {
            ShowMissMessage();
            ResetFlags();
        }
    }

    private void ShowMissMessage()
    {
        string message = missMessages[Random.Range(0, missMessages.Length)];
        feedbackText.text = message;
        Invoke(nameof(ClearFeedback), 2f);
    }

    private void ClearFeedback()
    {
        feedbackText.text = "";
    }

    private void ScorePoint()
    {
        Debug.Log("✅ Score counted!");
        currentScore++;
        scoreText.text = "Score: " + currentScore;
        feedbackText.text = "SCORE!";
        Invoke(nameof(ClearFeedback), 2f);
    }

    private void ResetFlags()
    {
        enteredHoop = false;
        hasScored = false;
    }

    private void StartGame()
    {
        Debug.Log("🏁 Game started.");

        currentScore = 0;
        scoreText.text = "Score: 0";
        timerText.text = "Time: 120";
        gameRunning = true;

        startPanel.SetActive(false);
        gamePanel.SetActive(true);

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        timerCoroutine = StartCoroutine(GameTimer());
    }

    private void StopGame()
    {
        Debug.Log("🛑 Game stopped.");
        gameRunning = false;

        if (currentScore > highScore)
        {
            highScore = currentScore;
            Debug.Log("🥇 New high score: " + highScore);
        }

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        startPanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    private IEnumerator GameTimer()
    {
        float remainingTime = gameDuration;

        while (remainingTime > 0)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(remainingTime).ToString();
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        timerText.text = "Time: 0";
        StopGame();
    }
}

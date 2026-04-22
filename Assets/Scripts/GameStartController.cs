using UnityEngine;
using TMPro;

public class GameStartController : MonoBehaviour
{
    [Header("UI")]
    public GameObject introPanel;
    public TMP_Text timerText;

    [Header("Audio")]
    public AudioSource introAudioSource;
    public AudioSource gameplayAudioSource;

    [Header("Timer")]
    public float elapsedTime = 0f;
    private bool timerRunning = false;
    private bool gameStarted = false;

    private void Start()
    {
        Time.timeScale = 0f;

        if (introPanel != null)
            introPanel.SetActive(true);

        if (introAudioSource != null)
            introAudioSource.Play();

        if (gameplayAudioSource != null)
            gameplayAudioSource.Stop();

        UpdateTimerUI();
    }

    private void Update()
    {
        if (!timerRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateTimerUI();
    }

    
    public void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;

        if (introPanel != null)
            introPanel.SetActive(false);

        if (introAudioSource != null && introAudioSource.isPlaying)
            introAudioSource.Stop();

        if (gameplayAudioSource != null)
            gameplayAudioSource.Play();

        Time.timeScale = 1f;
        timerRunning = true;
    }

    
    public void FinishGame()
    {
        timerRunning = false;
        Time.timeScale = 0f;

        if (gameplayAudioSource != null && gameplayAudioSource.isPlaying)
            gameplayAudioSource.Stop();
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
using UnityEngine;
using TMPro;

public class GameStartController : MonoBehaviour
{
    [Header("UI")]
    public GameObject introPanel;
    public TMP_Text timerText;

    [Header("Levels")]
    public Transform levelsRoot;

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

        Transform activeLevel = GetActiveLevelPanel();
        if (activeLevel != null)
        {
            SetTimerFromLevel(activeLevel, true);
        }
        else
        {
            UpdateTimerUI();
        }
    }

    private void Update()
    {
        if (!timerRunning)
            return;

        elapsedTime += Time.deltaTime;
        UpdateTimerUI();
    }

    public void StartGame()
    {
        if (gameStarted)
            return;

        gameStarted = true;

        if (introPanel != null)
            introPanel.SetActive(false);

        if (introAudioSource != null && introAudioSource.isPlaying)
            introAudioSource.Stop();

        if (gameplayAudioSource != null && !gameplayAudioSource.isPlaying)
            gameplayAudioSource.Play();

        Time.timeScale = 1f;
        timerRunning = true;
        UpdateTimerUI();
    }

    public void FinishLevel()
    {
        timerRunning = false;
        Time.timeScale = 0f;
    }

    public void ResumeForNextLevel()
    {
        Time.timeScale = 1f;
        timerRunning = true;

        if (gameplayAudioSource != null && !gameplayAudioSource.isPlaying)
            gameplayAudioSource.Play();

        UpdateTimerUI();
    }

    public void SetTimerText(TMP_Text newTimerText, bool resetElapsedTime = true)
    {
        timerText = newTimerText;

        if (resetElapsedTime)
        {
            elapsedTime = 0f;
        }

        UpdateTimerUI();
    }

    public void SetTimerFromLevel(Transform levelRoot, bool resetElapsedTime = true)
    {
        if (levelRoot == null)
            return;

        LevelUIReference levelUI = levelRoot.GetComponent<LevelUIReference>();
        if (levelUI == null || levelUI.timerText == null)
        {
            Debug.LogWarning($"Level {levelRoot.name} is missing LevelUIReference or timerText.");
            timerText = null;
            return;
        }

        SetTimerText(levelUI.timerText, resetElapsedTime);
    }

    private Transform GetActiveLevelPanel()
    {
        if (levelsRoot == null)
            return null;

        for (int i = 0; i < levelsRoot.childCount; i++)
        {
            Transform child = levelsRoot.GetChild(i);

            if (child.gameObject.activeSelf)
                return child;
        }

        return null;
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
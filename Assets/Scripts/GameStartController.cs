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

    [Header("Mascot")]
    public SparkyDialogueController sparkyDialogue;

    [Header("Timer")]
    public float elapsedTime = 0f;

    private bool timerRunning = false;
    private bool gameStarted = false;
    private LevelUIReference currentLevelUI;

    private void Start()
    {
        Time.timeScale = 0f;

        if (introPanel != null)
            introPanel.SetActive(true);

        if (introAudioSource != null)
            introAudioSource.Play();

        if (gameplayAudioSource != null)
            gameplayAudioSource.Stop();

        if (sparkyDialogue != null)
            sparkyDialogue.Hide();

        Transform activeLevel = GetActiveLevelPanel();
        if (activeLevel != null)
            PrepareLevel(activeLevel, showSparky: false);
        else
            UpdateTimerUI();
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
        if (!gameStarted)
        {
            gameStarted = true;

            if (introPanel != null)
                introPanel.SetActive(false);

            if (introAudioSource != null && introAudioSource.isPlaying)
                introAudioSource.Stop();

            if (gameplayAudioSource != null && !gameplayAudioSource.isPlaying)
                gameplayAudioSource.Play();
        }

        if (currentLevelUI != null && currentLevelUI.startBlockerPanel != null)
            currentLevelUI.startBlockerPanel.SetActive(false);

        if (sparkyDialogue != null)
            sparkyDialogue.Hide();

        Time.timeScale = 1f;
        timerRunning = true;
        UpdateTimerUI();
    }

    public void PrepareLevel(Transform levelRoot, bool showSparky = true)
    {
        Time.timeScale = 0f;
        timerRunning = false;

        SetTimerFromLevel(levelRoot, true);

        if (currentLevelUI != null && currentLevelUI.startBlockerPanel != null)
            currentLevelUI.startBlockerPanel.SetActive(true);

        if (showSparky && sparkyDialogue != null && currentLevelUI != null)
            sparkyDialogue.ShowLevelIntro(levelRoot.GetSiblingIndex());
    }

    public void ShowCurrentLevelIntro()
    {
        if (introPanel != null)
            introPanel.SetActive(false);

        if (currentLevelUI != null && currentLevelUI.startBlockerPanel != null)
            currentLevelUI.startBlockerPanel.SetActive(true);

        Transform activeLevel = GetActiveLevelPanel();

        if (activeLevel != null)
            sparkyDialogue.ShowLevelIntro(activeLevel.GetSiblingIndex());
    }

    public void FinishLevel()
    {
        timerRunning = false;
        Time.timeScale = 0f;
    }

    public void SetTimerFromLevel(Transform levelRoot, bool resetElapsedTime = true)
    {
        if (levelRoot == null)
            return;

        currentLevelUI = levelRoot.GetComponent<LevelUIReference>();

        if (currentLevelUI == null || currentLevelUI.timerText == null)
        {
            Debug.LogWarning($"Level {levelRoot.name} is missing LevelUIReference or timerText.");
            timerText = null;
            return;
        }

        timerText = currentLevelUI.timerText;

        if (resetElapsedTime)
            elapsedTime = 0f;

        UpdateTimerUI();
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
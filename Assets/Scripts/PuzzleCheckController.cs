using UnityEngine;
using TMPro;
using System.Collections;

public class PuzzleCheckController : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] private Transform _levelsRoot;

    [Header("UI")]
    [SerializeField] private TMP_Text _resultText;
    [SerializeField] private GameObject _nextLevelButton;

    [Header("Mascot")]
    [SerializeField] private GameObject _mascot;
    [SerializeField] private float _mascotDisplayTime = 2f;

    private Coroutine _mascotRoutine;

    [Header("Game Flow")]
    [SerializeField] private GameStartController _gameStartController;

    private void Start()
    {
        if (_nextLevelButton != null)
            _nextLevelButton.SetActive(false);

        if (_mascot != null)
            _mascot.SetActive(false);
    }

    public void CheckPuzzle()
    {
        Transform activeLevel = GetActiveLevelPanel();

        if (activeLevel == null)
        {
            if (_resultText != null)
                _resultText.text = "No active level found.";

            if (_nextLevelButton != null)
                _nextLevelButton.SetActive(false);

            return;
        }

        Drop[] dropZones = activeLevel.GetComponentsInChildren<Drop>(true);

        if (dropZones == null || dropZones.Length == 0)
        {
            if (_resultText != null)
                _resultText.text = "No drop zones found in active level.";

            if (_nextLevelButton != null)
                _nextLevelButton.SetActive(false);

            return;
        }

        int correctCount = 0;
        int totalCount = 0;

        for (int i = 0; i < dropZones.Length; i++)
        {
            Drop drop = dropZones[i];

            if (drop == null || !drop.gameObject.activeInHierarchy)
                continue;

            totalCount++;

            if (drop.IsCorrect())
                correctCount++;
        }

        if (totalCount == 0)
        {
            if (_resultText != null)
                _resultText.text = "No active drop zones found.";

            if (_nextLevelButton != null)
                _nextLevelButton.SetActive(false);

            return;
        }

        if (correctCount == totalCount)
        {
            if (_resultText != null)
                _resultText.text = $"Correct! {correctCount}/{totalCount}";

            bool hasNextLevel = activeLevel.GetSiblingIndex() < _levelsRoot.childCount - 1;

            if (_nextLevelButton != null)
                _nextLevelButton.SetActive(hasNextLevel);

            ShowMascot(permanent: true);

            if (_gameStartController != null)
                _gameStartController.FinishLevel();
        }
        else
        {
            if (_resultText != null)
                _resultText.text = $"You have {correctCount}/{totalCount} correct.";

            if (_nextLevelButton != null)
                _nextLevelButton.SetActive(false);

            ShowMascot(permanent: false);
        }
    }

    private void ShowMascot(bool permanent)
    {
        if (_mascot == null)
            return;

        _mascot.SetActive(true);

        if (_mascotRoutine != null)
        {
            StopCoroutine(_mascotRoutine);
            _mascotRoutine = null;
        }

        if (!permanent)
        {
            _mascotRoutine = StartCoroutine(HideMascotAfterDelay());
        }
    }

    private IEnumerator HideMascotAfterDelay()
    {
        yield return new WaitForSeconds(_mascotDisplayTime);

        if (_mascot != null)
            _mascot.SetActive(false);

        _mascotRoutine = null;
    }

    private void HideMascotImmediately()
    {
        if (_mascotRoutine != null)
        {
            StopCoroutine(_mascotRoutine);
            _mascotRoutine = null;
        }

        if (_mascot != null)
            _mascot.SetActive(false);
    }

    public void GoToNextLevel()
    {
        if (_levelsRoot == null)
            return;

        Transform currentLevel = GetActiveLevelPanel();

        if (currentLevel == null)
            return;

        int currentIndex = currentLevel.GetSiblingIndex();
        int nextIndex = currentIndex + 1;

        if (nextIndex >= _levelsRoot.childCount)
        {
            if (_resultText != null)
                _resultText.text = "All levels complete!";

            if (_nextLevelButton != null)
                _nextLevelButton.SetActive(false);

            HideMascotImmediately();
            return;
        }

        currentLevel.gameObject.SetActive(false);

        Transform nextLevel = _levelsRoot.GetChild(nextIndex);
        nextLevel.gameObject.SetActive(true);

        if (_nextLevelButton != null)
            _nextLevelButton.SetActive(false);

        if (_resultText != null)
            _resultText.text = "";

        HideMascotImmediately();

        if (_gameStartController != null)
        {
            _gameStartController.SetTimerFromLevel(nextLevel, true);
            _gameStartController.ResumeForNextLevel();
        }
    }

    private Transform GetActiveLevelPanel()
    {
        if (_levelsRoot == null)
            return null;

        for (int i = 0; i < _levelsRoot.childCount; i++)
        {
            Transform child = _levelsRoot.GetChild(i);

            if (child.gameObject.activeSelf)
                return child;
        }

        return null;
    }
}
using UnityEngine;
using TMPro;
using System.Collections;

public class SparkyDialogueController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _sparkyRoot;
    [SerializeField] private TMP_Text _speechText;

    [Header("Level Intro Messages")]
    [TextArea()]
    [SerializeField] private string[] _levelIntroMessages;

    [Header("Check Messages")]
    [TextArea()]
    [SerializeField] private string _incorrectMessage = "You have {0}/{1} correct.";

    [TextArea()]
    [SerializeField] private string _correctMessage = "Correct! {0}/{1}";

    [Header("Defaults")]
    [SerializeField] private float _autoHideTime = 2f;

    private Coroutine _hideRoutine;

    public void ShowLevelIntro(int levelIndex)
    {
        string message = "";

        if (_levelIntroMessages != null &&
            levelIndex >= 0 &&
            levelIndex < _levelIntroMessages.Length)
        {
            message = _levelIntroMessages[levelIndex];
        }

        ShowMessage(message, autoHide: false);
    }

    public void ShowCorrectMessage(int correctCount, int totalCount)
    {
        ShowMessage(string.Format(_correctMessage, correctCount, totalCount), autoHide: false);
    }

    public void ShowIncorrectMessage(int correctCount, int totalCount)
    {
        ShowMessage(string.Format(_incorrectMessage, correctCount, totalCount), autoHide: true);
    }

    public void ShowMessage(string message, bool autoHide)
    {
        if (_sparkyRoot == null)
            return;

        _sparkyRoot.SetActive(true);

        if (_speechText != null)
            _speechText.text = message;

        StopHideRoutine();

        if (autoHide)
            _hideRoutine = StartCoroutine(HideAfterDelay());
    }

    public void Hide()
    {
        StopHideRoutine();

        if (_sparkyRoot != null)
            _sparkyRoot.SetActive(false);
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(_autoHideTime);
        Hide();
    }

    private void StopHideRoutine()
    {
        if (_hideRoutine != null)
        {
            StopCoroutine(_hideRoutine);
            _hideRoutine = null;
        }
    }
}
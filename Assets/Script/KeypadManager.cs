using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Attach this to the Keypad Canvas/Panel GameObject.
/// Manages digit input, validates the code, and triggers the door.
/// </summary>
public class KeypadManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The 'Input Here' display text at the top of the keypad")]
    public TextMeshProUGUI displayText;

    [Header("Door Settings")]
    [Tooltip("The door GameObject to destroy when the correct code is entered")]
    public GameObject doorObject;

    [Tooltip("The secret code the player must enter (e.g. 1234)")]
    public string correctCode = "1234";

    [Header("Feedback")]
    [Tooltip("Color shown on the display when the code is correct")]
    public Color correctColor = new Color(0.2f, 0.8f, 0.2f);

    [Tooltip("Color shown on the display when the code is wrong")]
    public Color wrongColor = new Color(0.8f, 0.2f, 0.2f);

    [Tooltip("Default display background color")]
    public Color defaultColor = new Color(0.68f, 0.47f, 0.47f);

    [Header("Audio (optional)")]
    public AudioSource audioSource;
    public AudioClip keyPressSound;
    public AudioClip successSound;
    public AudioClip failSound;

    // --- private state ---
    private string currentInput = "";
    private bool isLocked = false;
    private Image displayBackground;
    private Coroutine submitCoroutine;  // tracked so we can stop it cleanly

    // --- Unity lifecycle ---
    void Awake()
    {
        if (displayText != null)
        {
            displayBackground = displayText.GetComponentInParent<Image>();
        }

        ResetDisplay();
    }

    // Called automatically when the keypad panel is hidden (player walks away)
    void OnDisable()
    {
        // Stop mid-feedback coroutine so it cannot skip the reset
        if (submitCoroutine != null)
        {
            StopCoroutine(submitCoroutine);
            submitCoroutine = null;
        }

        // Restore shake position in case the animation was interrupted mid-frame
        if (displayText != null)
            displayText.rectTransform.localPosition = Vector3.zero;

        isLocked = false;
        ResetDisplay();
    }

    // --- Public API called by each KeypadButton ---

    /// <summary>
    /// Call this from each number button's OnClick event, passing the digit string ("0" to "9").
    /// </summary>
    public void OnDigitPressed(string digit)
    {
        if (isLocked) return;

        PlaySound(keyPressSound);

        if (currentInput.Length >= correctCode.Length) return;

        currentInput += digit;
        UpdateDisplay();

        if (currentInput.Length == correctCode.Length)
        {
            submitCoroutine = StartCoroutine(SubmitCode());
        }
    }

    /// <summary>
    /// Optionally wire a Clear button to this. Clears all input.
    /// </summary>
    public void OnClearPressed()
    {
        if (isLocked) return;
        currentInput = "";
        ResetDisplay();
    }

    /// <summary>
    /// Call this from a Backspace button. Removes the last entered digit.
    /// </summary>
    public void OnBackspacePressed()
    {
        if (isLocked) return;
        if (currentInput.Length == 0) return;

        currentInput = currentInput.Substring(0, currentInput.Length - 1);

        if (currentInput.Length == 0)
        {
            ResetDisplay();
        }
        else
        {
            UpdateDisplay();
        }
    }

    // --- Private helpers ---

    private void UpdateDisplay()
    {
        displayText.text = new string('*', currentInput.Length);
    }

    private void ResetDisplay()
    {
        currentInput = "";
        if (displayText != null)
            displayText.text = "Input Here";
        SetDisplayColor(defaultColor);
    }

    private IEnumerator SubmitCode()
    {
        isLocked = true;

        if (currentInput == correctCode)
        {
            // Correct
            displayText.text = "ACCESS GRANTED";
            SetDisplayColor(correctColor);
            PlaySound(successSound);

            yield return new WaitForSeconds(1.2f);

            OpenDoor();
        }
        else
        {
            // Wrong
            displayText.text = "DENIED";
            SetDisplayColor(wrongColor);
            PlaySound(failSound);

            yield return StartCoroutine(ShakeDisplay());

            yield return new WaitForSeconds(0.4f);

            ResetDisplay();
        }

        isLocked = false;
        submitCoroutine = null;
    }

    private void OpenDoor()
    {
        if (doorObject != null)
        {
            Destroy(doorObject);
        }
        else
        {
            Debug.LogWarning("KeypadManager: No door GameObject assigned!");
        }

        gameObject.SetActive(false);
    }

    private void SetDisplayColor(Color color)
    {
        if (displayBackground != null)
            displayBackground.color = color;
    }

    private IEnumerator ShakeDisplay()
    {
        if (displayText == null) yield break;

        Vector3 origin = displayText.rectTransform.localPosition;
        float duration = 0.4f;
        float magnitude = 6f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = origin.x + Random.Range(-1f, 1f) * magnitude;
            float y = origin.y + Random.Range(-1f, 1f) * magnitude;
            displayText.rectTransform.localPosition = new Vector3(x, y, origin.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        displayText.rectTransform.localPosition = origin;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
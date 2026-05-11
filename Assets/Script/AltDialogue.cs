using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class AltDialogue : MonoBehaviour
{
    [TextArea(2, 5)]
    public string[] lines;
    public TextMeshProUGUI dialogueText;
    public GameObject Yes;
    public bool tutorial = false;
    public bool HasParentImage;
    public GameObject Image;

    private int currentLine = 0;
    private bool isActive = false;
    private bool hasTriggered = false;
    private bool dialogueDone = false;

    void Update()
    {
        if (!isActive) return;
        if (!Keyboard.current.eKey.wasPressedThisFrame) return;
        if (dialogueDone) return;

        currentLine++;
        if (currentLine >= lines.Length)
        {
            EndDialogue();
            dialogueDone = true;
            Time.timeScale = 1;
        }
        else
        {
            ShowLine();
        }
    }

    void ShowLine()
    {
        dialogueText.gameObject.SetActive(true);
        if (HasParentImage)
            Image.gameObject.SetActive(true);
        dialogueText.text = lines[currentLine];
    }

    void EndDialogue()
    {
        isActive = false;
        dialogueText.gameObject.SetActive(false);
        if (HasParentImage)
            Image.gameObject.SetActive(false);
        dialogueText.text = "";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (hasTriggered) return;

        hasTriggered = true;
        isActive = true;
        currentLine = 0;
        ShowLine();
        Time.timeScale = 0f;

        if (Yes != null) Yes.gameObject.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        EndDialogue();
    }
}
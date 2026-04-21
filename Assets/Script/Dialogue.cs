using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [TextArea(2, 5)]
    public string[] lines;

    public TextMeshProUGUI dialogueText;

    private int currentLine = 0;
    private bool playerNear = false;
    private bool isActive = false;

    void Update()
    {
        if (!playerNear) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!isActive)
            {
                isActive = true;
                currentLine = 0;
                ShowLine();
            }
            else
            {
                currentLine++;
                if (currentLine >= lines.Length)
                {
                    EndDialogue();
                }
                else
                {
                    ShowLine();
                }
            }
        }
    }

    void ShowLine()
    {
        dialogueText.gameObject.SetActive(true);
        dialogueText.text = lines[currentLine];
    }

    void EndDialogue()
    {
        isActive = false;
        dialogueText.gameObject.SetActive(false);
        dialogueText.text = "";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerNear = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNear = false;
            EndDialogue();
        }
    }
}
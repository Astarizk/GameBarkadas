using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [TextArea(2, 5)]
    public string[] lines;

    public TextMeshProUGUI dialogueText;
    public GameObject Yes;
    private int currentLine = 0;
    private bool playerNear = false;
    private bool isActive = false;
    public bool tutorial = false;
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
                Time.timeScale = 0;
                Yes.gameObject.SetActive(false);
            }
            else
            {
                currentLine++;
                if (currentLine >= lines.Length)
                {
                    EndDialogue();
                    Time.timeScale = 1;
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
        {
            
            if (tutorial == true)
            {
                Yes.gameObject.SetActive(true);
            }
            playerNear = true;
        }
            
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNear = false;
            EndDialogue();
            Yes.gameObject.SetActive(false);
        }
    }
}
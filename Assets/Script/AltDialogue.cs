using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class AltDialogue : MonoBehaviour
{
    [TextArea(2, 5)]
    public string[] lines;

    public TextMeshProUGUI dialogueText;
    public GameObject Yes;
    private int currentLine = 0;
    private bool playerNear = false;
    private bool isActive = false;
    public bool tutorial = false;
    public bool HasParentImage;
    public int done = 0;
    public GameObject Image;
    void Update()
    {

    }

    void ShowLine()
    {
        dialogueText.gameObject.SetActive(true);
        if (HasParentImage == true)
        {
            Image.gameObject.SetActive(true);
        }

        dialogueText.text = lines[currentLine];
    }

    void EndDialogue()
    {
        isActive = false;
        dialogueText.gameObject.SetActive(false);
        if (HasParentImage == true)
        {
            Image.gameObject.SetActive(false);
        }
        dialogueText.text = "";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && done ==0)
        {
            currentLine = 0;
            ShowLine();

            done++;
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
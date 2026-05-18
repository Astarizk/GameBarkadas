using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    public GameObject puzzleUI;
    public MonoBehaviour playerMovement;

    private bool playerInside;

    void Start()
    {
        puzzleUI.SetActive(false);
    }

    void Update()
    {
        // Open puzzle
        if (playerInside && Keyboard.current.eKey.wasPressedThisFrame)
        {
            OpenPuzzle();
        }

        // Close puzzle if movement keys are pressed
        if (puzzleUI.activeSelf)
        {
            bool moving =
                Keyboard.current.wKey.isPressed ||
                Keyboard.current.aKey.isPressed ||
                Keyboard.current.sKey.isPressed ||
                Keyboard.current.dKey.isPressed;

            if (moving)
            {
                ClosePuzzle();
            }
        }
    }

    void OpenPuzzle()
    {
        puzzleUI.SetActive(true);

        // Disable movement
        playerMovement.enabled = false;
    }

    void ClosePuzzle()
    {
        puzzleUI.SetActive(false);

        // Enable movement
        playerMovement.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            ClosePuzzle();
        }
    }
}
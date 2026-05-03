using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Optional — place this on a trigger collider near the door.
/// When the player enters the trigger, the keypad canvas appears.
/// When they leave, it hides again.
///
/// Requires a Collider2D with "Is Trigger" checked on the same GameObject.
/// Tag your player GameObject as "Player".
/// </summary>
public class KeypadActivator : MonoBehaviour
{
    [Tooltip("The Keypad Canvas or Panel to show/hide")]
    public GameObject keypadCanvas;

    [Tooltip("Tag used to identify the player")]
    public string playerTag = "Player";

    [Header("Optional: Press a key to open instead of auto-show")]
    public bool requireKeyPress = false;

    private bool playerNearby = false;

    void Start()
    {
        if (keypadCanvas != null)
            keypadCanvas.SetActive(false);   // hidden by default
    }

    void Update()
    {
        if (requireKeyPress && playerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleKeypad();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerNearby = true;

        if (!requireKeyPress && keypadCanvas != null)
            keypadCanvas.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerNearby = false;

        if (keypadCanvas != null)
            keypadCanvas.SetActive(false);
    }

    private void ToggleKeypad()
    {
        if (keypadCanvas != null)
            keypadCanvas.SetActive(!keypadCanvas.activeSelf);
    }
}
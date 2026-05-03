using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this script to EACH number button on the keypad.
///
/// Setup per button:
///   1. Set the "Digit" field in the Inspector (e.g. "1", "2", "0")
///   2. Drag the KeypadManager GameObject into the "Keypad Manager" field
///      OR the script will find it automatically at runtime.
///   3. The button's OnClick() in the Inspector is NOT needed;
///      this script wires itself up automatically.
/// </summary>
[RequireComponent(typeof(Button))]
public class KeypadButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Tooltip("The digit this button represents, e.g. \"1\" or \"0\"")]
    public string digit = "0";

    [Tooltip("Reference to the KeypadManager (auto-found if left empty)")]
    public KeypadManager keypadManager;

    [Header("Press Animation")]
    [Tooltip("How far the button sinks on press (in UI units)")]
    public float pressDepth = 4f;

    [Tooltip("Speed of the press / release animation")]
    public float animSpeed = 18f;

    // --- private ---
    private Button btn;
    private RectTransform rt;
    private Vector3 restPosition;
    private Vector3 pressedPosition;
    private bool isPressed = false;

    void Awake()
    {
        btn = GetComponent<Button>();
        rt = GetComponent<RectTransform>();

        restPosition = rt.localPosition;
        pressedPosition = restPosition + new Vector3(0, -pressDepth, 0);

        // Auto-find manager if not assigned
        if (keypadManager == null)
            keypadManager = FindObjectOfType<KeypadManager>();

        // Wire the click
        btn.onClick.AddListener(() => keypadManager?.OnDigitPressed(digit));
    }

    void Update()
    {
        // Smooth press / release animation
        Vector3 target = isPressed ? pressedPosition : restPosition;
        rt.localPosition = Vector3.Lerp(rt.localPosition, target, Time.deltaTime * animSpeed);
    }

    public void OnPointerDown(PointerEventData eventData) => isPressed = true;
    public void OnPointerUp(PointerEventData eventData) => isPressed = false;
}
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzlePiece : MonoBehaviour
{
    public int order;
    public Color activatedColor = Color.yellow;

    private FloorPuzzle group;
    private SpriteRenderer sprite;
    private Color startColor;
    private bool playerInside = false;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        startColor = sprite.color;

        // Find the group this piece belongs to (its parent or ancestor)
        group = GetComponentInParent<FloorPuzzle>();

        if (group == null)
            Debug.LogWarning($"PuzzlePiece '{name}' has no FloorPuzzle in its parent!", this);
    }

    void Update()
    {
        if (playerInside && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log($"E pressed on piece {order}, group = {group}");
            group?.ReportStep(this);
        }
    }
    public void SetActivated(bool on)
    {
        if (sprite == null) sprite = GetComponent<SpriteRenderer>();
        sprite.color = on ? activatedColor : startColor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInside = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInside = false;
    }
}
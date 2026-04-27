using UnityEngine;

/// <summary>
/// Attach this to your Enemy GameObject.
/// Requires a Collider2D on the enemy (does NOT need Is Trigger checked —
/// works with both trigger and solid colliders).
/// </summary>
public class EnemyCatch : MonoBehaviour
{
    public JumpscareManager jumpscareManager;

    private bool caught = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (caught) return;
        if (!collision.CompareTag("Player")) return;

        Catch();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (caught) return;
        if (!collision.collider.CompareTag("Player")) return;

        Catch();
    }

    private void Catch()
    {
        caught = true;

        if (jumpscareManager != null)
            jumpscareManager.TriggerJumpscare();
        else
            Debug.LogWarning("JumpscareManager not assigned on EnemyCatch!");
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public bool gameItem1Picked;
    public bool gameItem1OnSite;
    public GameObject ForItem1;

    public Vector2 boxSize = new Vector2(1f, 1f);
    public LayerMask itemLayer;

    void Update()
    {
        Collider2D hit = Physics2D.OverlapBox(player.position, boxSize, 0f, itemLayer);

        gameItem1OnSite = hit != null;

        if (gameItem1OnSite && Keyboard.current.eKey.wasPressedThisFrame)
        {
            gameItem1Picked = true;
            Destroy(hit.gameObject);
            Destroy(ForItem1);
        }
    }
}
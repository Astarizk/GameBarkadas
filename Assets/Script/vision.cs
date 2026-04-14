using UnityEngine;

public class vision : MonoBehaviour
{
    public Transform player;
    public Camera cam;

    void Update()
    {
        Vector2 screenPos = cam.WorldToScreenPoint(player.position);
        transform.position = screenPos;
    }
}
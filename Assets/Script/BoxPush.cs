using UnityEngine;
using UnityEngine.InputSystem;

public class PushBox : MonoBehaviour
{
    public float moveDistance = 1f;
    public float moveSpeed = 5f;

    public LayerMask wallLayer;
    public LayerMask playerLayer;

    private bool isMoving = false;
    private Vector3 targetPosition;

    private void Start()
    {
        transform.position = SnapToGrid(transform.position);
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }

            return;
        }

        Collider2D player = Physics2D.OverlapBox(
            transform.position,
            new Vector2(1.1f, 1.1f),
            0f,
            playerLayer
        );

        if (player != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Vector2 dir = Get4Direction(player.transform.position, transform.position);

            Vector3 proposedPosition = SnapToGrid(transform.position + (Vector3)(dir * moveDistance));

            // IMPORTANT: check destination only
            Collider2D wallCheck = Physics2D.OverlapBox(
                proposedPosition,
                new Vector2(0.9f, 0.9f),
                0f,
                wallLayer
            );

            if (wallCheck != null)
                return;

            targetPosition = proposedPosition;
            isMoving = true;
        }
    }

    private Vector2 Get4Direction(Vector3 playerPos, Vector3 boxPos)
    {
        Vector3 diff = boxPos - playerPos;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return new Vector2(Mathf.Sign(diff.x), 0);
        else
            return new Vector2(0, Mathf.Sign(diff.y));
    }

    private Vector3 SnapToGrid(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x),
            Mathf.Round(pos.y),
            pos.z
        );
    }
}
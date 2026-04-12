using UnityEngine;
using UnityEngine.InputSystem;

public class PushPullBox : MonoBehaviour
{
    public float moveDistance = 1f;
    public float moveSpeed = 6f;

    public LayerMask wallLayer;
    public LayerMask playerLayer;

    private bool isMoving = false;
    private Transform player;
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

        HandlePlayerDetection();
        HandleInput();
    }

    private void HandlePlayerDetection()
    {
        Collider2D p = Physics2D.OverlapBox(
            transform.position,
            new Vector2(1.1f, 1.1f),
            0f,
            playerLayer
        );

        player = (p != null) ? p.transform : null;
    }

    private void HandleInput()
    {
        if (player == null)
            return;

        Vector2 dir = Vector2.zero;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            dir = GetPushDirection(player.position, transform.position);
        }
        else if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            dir = GetPullDirection(player.position, transform.position);
        }
        else
        {
            return;
        }

        Vector3 nextPos = SnapToGrid(transform.position + (Vector3)(dir * moveDistance));

        if (IsBlocked(nextPos))
            return;

        targetPosition = nextPos;
        isMoving = true;
    }

    // PUSH = away from player
    private Vector2 GetPushDirection(Vector3 playerPos, Vector3 boxPos)
    {
        Vector3 diff = boxPos - playerPos;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return new Vector2(Mathf.Sign(diff.x), 0);
        else
            return new Vector2(0, Mathf.Sign(diff.y));
    }

    // PULL = toward player
    private Vector2 GetPullDirection(Vector3 playerPos, Vector3 boxPos)
    {
        return -GetPushDirection(playerPos, boxPos);
    }

    private bool IsBlocked(Vector3 pos)
    {
        Collider2D hit = Physics2D.OverlapBox(
            pos,
            new Vector2(0.9f, 0.9f),
            0f,
            wallLayer
        );

        return hit != null;
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
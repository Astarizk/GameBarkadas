using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Tilemap wallTilemap;
    public Transform player;

    [Header("Movement & Vision")]
    public float moveSpeed = 3f;
    public float visionRange = 10f;

    [Header("Stealth Settings")]
    public float teleportDelay = 1.5f;

    [Header("Smoothing")]
    public float velocitySmoothTime = 0.08f;

    [Header("Pathing Stability")]
    public float repathInterval = 0.35f;
    public float minMoveBeforeRepath = 0.2f;
    public float arriveDistance = 0.12f;

    [Header("Wander Settings")]
    public float wanderRadius = 8f;
    public float wanderWaitTime = 1.5f;

    private Rigidbody2D rb;
    private List<Vector3Int> path = new List<Vector3Int>();
    private int pathIndex = 0;
    private Vector3 currentTarget;
    private bool hasTarget = false;
    private bool isWaitingToTeleport = false;
    private Vector2 velocitySmoothing;

    private enum EnemyState { Idle, Chase, GoToLastKnown, Wander }
    private EnemyState state = EnemyState.Idle;
    private Vector3 lastKnownPlayerPos;
    private float wanderWaitTimer = 0f;

    private Vector3 lastRepathPos;
    private Vector3Int lastGoalCell;
    private bool hasLastGoal = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        transform.position = TileCenter(transform.position);
        lastRepathPos = transform.position;
        StartCoroutine(RefreshPath());
    }

    void StartWander()
    {
        state = EnemyState.Wander;
        hasTarget = false;
        hasLastGoal = false;
        path.Clear();
        wanderWaitTimer = 0f;

        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector3 wp = GetRandomWanderPoint();
            if (wp != Vector3.zero)
            {
                PathToPosition(wp);
                return;
            }
        }
    }

    public void OnPlayerUsedPortal(Vector3 portalPosition, Transform destination, bool wasHidingSpot)
    {
        float dist = Vector2.Distance(transform.position, portalPosition);
        Vector3Int myCell = wallTilemap.WorldToCell(transform.position);
        Vector3Int portalCell = wallTilemap.WorldToCell(portalPosition);

        if (dist <= visionRange && CheckLineOfSight(myCell, portalCell))
        {
            isWaitingToTeleport = true;
            hasTarget = false;
            path.Clear();
            rb.linearVelocity = Vector2.zero;
            velocitySmoothing = Vector2.zero;
            StartCoroutine(DelayedTeleport(destination));
        }
        else
        {
            if (wasHidingSpot)
            {
                return;
            }

            StartWander();
        }
    }

    private IEnumerator DelayedTeleport(Transform destination)
    {
        yield return new WaitForSeconds(teleportDelay);
        if (destination != null)
            transform.position = TileCenter(destination.position);

        isWaitingToTeleport = false;
        hasLastGoal = false;
        path.Clear();
        hasTarget = false;
        lastRepathPos = transform.position;
        state = EnemyState.Chase;
    }

    IEnumerator RefreshPath()
    {
        while (true)
        {
            if (player != null) RecalcPath();
            yield return new WaitForSeconds(repathInterval);
        }
    }

    void RecalcPath()
    {
        if (isWaitingToTeleport) return;

        Vector3Int myCell = wallTilemap.WorldToCell(transform.position);
        Vector3Int playerCell = wallTilemap.WorldToCell(player.position);
        float distToPlayer = Vector2.Distance(transform.position, player.position);
        bool canSeePlayer = !DoorOrPortal.PlayerIsHidden && distToPlayer <= visionRange && CheckLineOfSight(myCell, playerCell);

        if (canSeePlayer && !DoorOrPortal.PlayerIsHidden)
        {
            state = EnemyState.Chase;
            lastKnownPlayerPos = player.position;
        }

        switch (state)
        {
            case EnemyState.Chase:
                if (DoorOrPortal.PlayerIsHidden && !canSeePlayer)
                {
                    state = EnemyState.GoToLastKnown;
                    hasTarget = false;
                    path.Clear();
                }
                else
                {
                    lastKnownPlayerPos = player.position;
                    PathToPosition(player.position);
                }
                break;

            case EnemyState.GoToLastKnown:
                float distToLastKnown = Vector2.Distance(transform.position, lastKnownPlayerPos);

                if (!hasTarget)
                {
                    StartWander();
                    break;
                }

                if (distToLastKnown <= arriveDistance * 4f)
                {
                    StartWander();
                }
                else
                {
                    PathToPosition(lastKnownPlayerPos);
                }
                break;

            case EnemyState.Wander:
                if (!DoorOrPortal.PlayerIsHidden && canSeePlayer)
                {
                    state = EnemyState.Chase;
                    lastKnownPlayerPos = player.position;
                    hasTarget = false;
                    path.Clear();
                    break;
                }
                HandleWander();
                break;

            case EnemyState.Idle:
                if (canSeePlayer) state = EnemyState.Chase;
                break;
        }
    }

    void HandleWander()
    {
        if (hasTarget) return;

        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector3 randomTarget = GetRandomWanderPoint();
            if (randomTarget != Vector3.zero)
            {
                hasLastGoal = false;
                path.Clear();
                PathToPosition(randomTarget);
                return;
            }
        }
    }

    Vector3 GetRandomWanderPoint()
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
            Vector3 candidate = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            Vector3Int cell = wallTilemap.WorldToCell(candidate);
            if (!IsWall(cell))
                return wallTilemap.GetCellCenterWorld(cell);
        }
        return Vector3.zero;
    }

    void PathToPosition(Vector3 targetPos)
    {
        Vector3Int startCell = wallTilemap.WorldToCell(transform.position);
        Vector3Int goalCell = wallTilemap.WorldToCell(targetPos);

        if (IsWall(goalCell)) return;

        float movedSinceRepath = Vector2.Distance(transform.position, lastRepathPos);
        if (hasLastGoal && goalCell == lastGoalCell && movedSinceRepath < minMoveBeforeRepath && path.Count > 0)
            return;

        var newPath = AStar(startCell, goalCell);
        if (newPath == null || newPath.Count == 0) return;

        int bestIdx = 0;
        float bestDist = float.MaxValue;
        Vector2 pos = rb.position;
        for (int i = 0; i < newPath.Count; i++)
        {
            float d = ((Vector2)TileCenterFromCell(newPath[i]) - pos).sqrMagnitude;
            if (d < bestDist) { bestDist = d; bestIdx = i; }
        }

        path = newPath;
        pathIndex = Mathf.Min(bestIdx + 1, path.Count - 1);
        currentTarget = TileCenterFromCell(path[pathIndex]);
        hasTarget = true;

        lastGoalCell = goalCell;
        hasLastGoal = true;
        lastRepathPos = transform.position;
    }

    void FixedUpdate()
    {
        if (!hasTarget || isWaitingToTeleport)
        {
            rb.linearVelocity = Vector2.SmoothDamp(
                rb.linearVelocity, Vector2.zero, ref velocitySmoothing, velocitySmoothTime);
            return;
        }

        Vector2 toTarget = (Vector2)currentTarget - rb.position;
        float dist = toTarget.magnitude;

        if (dist <= arriveDistance)
        {
            AdvanceTarget();
            return;
        }

        rb.linearVelocity = Vector2.SmoothDamp(
            rb.linearVelocity, toTarget.normalized * moveSpeed, ref velocitySmoothing, velocitySmoothTime);
    }

    void AdvanceTarget()
    {
        pathIndex++;
        if (path == null || pathIndex >= path.Count)
        {
            rb.linearVelocity = Vector2.zero;
            velocitySmoothing = Vector2.zero;
            hasTarget = false;

            if (state == EnemyState.Wander)
            {
                StartWander();
            }

            return;
        }
        currentTarget = TileCenterFromCell(path[pathIndex]);
        hasTarget = true;
    }

    bool CheckLineOfSight(Vector3Int start, Vector3Int end)
    {
        int x0 = start.x, y0 = start.y, x1 = end.x, y1 = end.y;
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy, e2;
        while (true)
        {
            if (IsWall(new Vector3Int(x0, y0, 0)) && (x0 != x1 || y0 != y1)) return false;
            if (x0 == x1 && y0 == y1) break;
            e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
        return true;
    }

    List<Vector3Int> AStar(Vector3Int start, Vector3Int goal)
    {
        if (IsWall(start)) return null;
        var openSet = new List<ANode>();
        var closedSet = new HashSet<Vector3Int>();
        var nodeMap = new Dictionary<Vector3Int, ANode>();
        var startNode = new ANode(start, null, 0, Manhattan(start, goal));
        openSet.Add(startNode);
        nodeMap[start] = startNode;
        int limit = 4000;
        while (openSet.Count > 0 && limit-- > 0)
        {
            int bestIdx = 0;
            for (int i = 1; i < openSet.Count; i++)
                if (openSet[i].F < openSet[bestIdx].F ||
                   (Mathf.Approximately(openSet[i].F, openSet[bestIdx].F) && openSet[i].h < openSet[bestIdx].h))
                    bestIdx = i;
            ANode cur = openSet[bestIdx];
            openSet.RemoveAt(bestIdx);
            closedSet.Add(cur.pos);
            if (cur.pos == goal) return Reconstruct(cur);
            Vector3Int[] neighbors = {
                cur.pos + Vector3Int.up, cur.pos + Vector3Int.down,
                cur.pos + Vector3Int.left, cur.pos + Vector3Int.right
            };
            foreach (var nb in neighbors)
            {
                if (closedSet.Contains(nb) || IsWall(nb)) continue;
                float g = cur.g + 1f;
                if (nodeMap.TryGetValue(nb, out ANode existing))
                {
                    if (g < existing.g)
                    {
                        existing.g = g; existing.parent = cur;
                        if (!openSet.Contains(existing)) openSet.Add(existing);
                    }
                }
                else
                {
                    var nbNode = new ANode(nb, cur, g, Manhattan(nb, goal));
                    openSet.Add(nbNode); nodeMap[nb] = nbNode;
                }
            }
        }
        return null;
    }

    List<Vector3Int> Reconstruct(ANode end)
    {
        var result = new List<Vector3Int>();
        ANode cur = end;
        while (cur != null) { result.Add(cur.pos); cur = cur.parent; }
        result.Reverse();
        if (result.Count > 0) result.RemoveAt(0);
        return result;
    }

    bool IsWall(Vector3Int cell) => wallTilemap != null && wallTilemap.HasTile(cell);
    float Manhattan(Vector3Int a, Vector3Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    Vector3 TileCenter(Vector3 worldPos) => TileCenterFromCell(wallTilemap.WorldToCell(worldPos));
    Vector3 TileCenterFromCell(Vector3Int cell) => wallTilemap.GetCellCenterWorld(cell);

    class ANode
    {
        public Vector3Int pos; public ANode parent; public float g, h;
        public float F => g + h;
        public ANode(Vector3Int p, ANode par, float g, float h) { pos = p; parent = par; this.g = g; this.h = h; }
    }
}
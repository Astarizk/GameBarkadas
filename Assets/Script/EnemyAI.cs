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
    [Tooltip("How many seconds the enemy waits at the door before following you.")]
    public float teleportDelay = 1.5f; 

    // ── internals ──────────────────────────────────────────
    private Rigidbody2D rb;
    private List<Vector3Int> path = new List<Vector3Int>();
    private int pathIndex = 0;
    private Vector3 currentTarget;
    private bool hasTarget = false;
    private bool isWaitingToTeleport = false; // Prevents the enemy from moving while waiting

    private const float ARRIVE_DIST = 0.08f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        transform.position = TileCenter(transform.position);
        StartCoroutine(RefreshPath());
    }

    // ─────────────────────────────────────────────────────
    //  Called by DoorOrPortal.cs when 'E' is pressed
    // ─────────────────────────────────────────────────────
    public void OnPlayerUsedPortal(Vector3 portalPosition, Transform destination, bool wasHidingSpot)
    {
        float dist = Vector2.Distance(transform.position, portalPosition);
        Vector3Int myCell = wallTilemap.WorldToCell(transform.position);
        Vector3Int portalCell = wallTilemap.WorldToCell(portalPosition);

        // If the enemy sees the portal being used...
        if (dist <= visionRange && CheckLineOfSight(myCell, portalCell))
        {
            if (wasHidingSpot)
            {
                Debug.Log($"CAUGHT! Enemy saw you hide. Waiting {teleportDelay} seconds to enter...");
            }
            else
            {
                Debug.Log($"Enemy saw you use a door. Waiting {teleportDelay} seconds to follow...");
            }

            // Instantly stop the enemy from moving
            isWaitingToTeleport = true;
            hasTarget = false;
            path.Clear();
            rb.linearVelocity = Vector2.zero;

            // Start the timer to teleport!
            StartCoroutine(DelayedTeleport(destination));
        }
    }

    // ─────────────────────────────────────────────────────
    //  NEW: The Coroutine that creates the delay!
    // ─────────────────────────────────────────────────────
    private IEnumerator DelayedTeleport(Transform destination)
    {
        // Wait for the specified amount of seconds
        yield return new WaitForSeconds(teleportDelay);

        // Teleport the enemy!
        if (destination != null)
        {
            transform.position = destination.position;
            transform.position = TileCenter(transform.position); // Snap to new grid
        }

        // Allow the enemy to move and think again
        isWaitingToTeleport = false;
        Debug.Log("Enemy entered the room!");
    }

    // ─────────────────────────────────────────────────────
    //  Path refresh – runs every 0.25 s
    // ─────────────────────────────────────────────────────
    IEnumerator RefreshPath()
    {
        while (true)
        {
            if (player != null)
                RecalcPath();

            yield return new WaitForSeconds(0.25f);
        }
    }

    void RecalcPath()
    {
        // If we are currently paused outside a door waiting to teleport, don't think!
        if (isWaitingToTeleport) return; 

        // If the player successfully hid (and the enemy didn't catch them), give up!
        if (DoorOrPortal.PlayerIsHidden)
        {
            hasTarget = false;
            path.Clear();
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector3Int startCell = wallTilemap.WorldToCell(transform.position);
        Vector3Int goalCell  = wallTilemap.WorldToCell(player.position);

        if (IsWall(goalCell)) return;   

        var newPath = AStar(startCell, goalCell);
        if (newPath == null || newPath.Count == 0) return;

        path      = newPath;
        pathIndex = 0;
        AdvanceTarget();
    }

    // ─────────────────────────────────────────────────────
    //  Update – smooth movement toward current waypoint
    // ─────────────────────────────────────────────────────
    void FixedUpdate()
    {
        // Don't move if we are waiting at a door
        if (!hasTarget || isWaitingToTeleport) 
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = ((Vector2)currentTarget - rb.position).normalized;
        float dist  = Vector2.Distance(rb.position, currentTarget);

        if (dist <= ARRIVE_DIST)
        {
            rb.MovePosition(currentTarget);
            AdvanceTarget();
        }
        else
        {
            rb.linearVelocity = dir * moveSpeed;
        }
    }

    void AdvanceTarget()
    {
        if (pathIndex >= path.Count)
        {
            rb.linearVelocity = Vector2.zero;
            hasTarget = false;
            return;
        }

        currentTarget = TileCenterFromCell(path[pathIndex]);
        pathIndex++;
        hasTarget = true;
    }

    // ─────────────────────────────────────────────────────
    //  Grid-Based Line of Sight (Bresenham's Line)
    // ─────────────────────────────────────────────────────
    bool CheckLineOfSight(Vector3Int start, Vector3Int end)
    {
        int x0 = start.x; int y0 = start.y;
        int x1 = end.x; int y1 = end.y;
        
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy, e2;

        while (true)
        {
            if (IsWall(new Vector3Int(x0, y0, 0))) 
            {
                if (x0 != x1 || y0 != y1) 
                {
                    return false; 
                }
            }
            
            if (x0 == x1 && y0 == y1) break; 
            
            e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
        return true; 
    }

    // ─────────────────────────────────────────────────────
    //  A* (BFS-quality, 4-directional, tile-based)
    // ─────────────────────────────────────────────────────
    List<Vector3Int> AStar(Vector3Int start, Vector3Int goal)
    {
        if (IsWall(start)) return null;

        var openSet   = new List<ANode>();
        var closedSet = new HashSet<Vector3Int>();
        var nodeMap   = new Dictionary<Vector3Int, ANode>();

        var startNode = new ANode(start, null, 0, Manhattan(start, goal));
        openSet.Add(startNode);
        nodeMap[start] = startNode;

        int limit = 2000;

        while (openSet.Count > 0 && limit-- > 0)
        {
            int bestIdx = 0;
            for (int i = 1; i < openSet.Count; i++)
                if (openSet[i].F < openSet[bestIdx].F) bestIdx = i;

            ANode cur = openSet[bestIdx];
            openSet.RemoveAt(bestIdx);
            closedSet.Add(cur.pos);

            if (cur.pos == goal)
                return Reconstruct(cur);

            Vector3Int[] neighbors = {
                cur.pos + Vector3Int.up,
                cur.pos + Vector3Int.down,
                cur.pos + Vector3Int.left,
                cur.pos + Vector3Int.right
            };

            foreach (var nb in neighbors)
            {
                if (closedSet.Contains(nb)) continue;
                if (IsWall(nb)) continue;

                float g = cur.g + 1f;

                if (nodeMap.TryGetValue(nb, out ANode existing))
                {
                    if (g < existing.g)
                    {
                        existing.g      = g;
                        existing.parent = cur;
                        if (!openSet.Contains(existing))
                            openSet.Add(existing);
                    }
                }
                else
                {
                    var nbNode = new ANode(nb, cur, g, Manhattan(nb, goal));
                    openSet.Add(nbNode);
                    nodeMap[nb] = nbNode;
                }
            }
        }

        return null; 
    }

    List<Vector3Int> Reconstruct(ANode end)
    {
        var result = new List<Vector3Int>();
        ANode cur = end;
        while (cur != null)
        {
            result.Add(cur.pos);
            cur = cur.parent;
        }
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
        public Vector3Int pos; public ANode parent;
        public float g; public float h; public float F => g + h;
        public ANode(Vector3Int p, ANode par, float g, float h)
        { pos = p; parent = par; this.g = g; this.h = h; }
    }
}
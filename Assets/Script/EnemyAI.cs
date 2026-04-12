using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Tilemap wallTilemap;
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;

    // ── internals ──────────────────────────────────────────
    private Rigidbody2D rb;
    private List<Vector3Int> path = new List<Vector3Int>();
    private int pathIndex = 0;
    private Vector3 currentTarget;
    private bool hasTarget = false;

    // how close (world units) the enemy must be to a waypoint
    // before it advances — slightly larger than float error
    private const float ARRIVE_DIST = 0.08f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        // Snap enemy to nearest tile center on start
        transform.position = TileCenter(transform.position);

        StartCoroutine(RefreshPath());
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
        // Always start A* from the NEAREST TILE CENTER the enemy has already passed or is on
        Vector3Int startCell = wallTilemap.WorldToCell(transform.position);
        Vector3Int goalCell  = wallTilemap.WorldToCell(player.position);

        if (IsWall(goalCell)) return;   // player inside wall – keep old path

        var newPath = AStar(startCell, goalCell);
        if (newPath == null || newPath.Count == 0) return;

        path      = newPath;
        pathIndex = 0;

        // Set the very next waypoint immediately
        AdvanceTarget();
    }

    // ─────────────────────────────────────────────────────
    //  Update – smooth movement toward current waypoint
    // ─────────────────────────────────────────────────────
    void FixedUpdate()
    {
        if (!hasTarget) return;

        Vector2 dir = ((Vector2)currentTarget - rb.position).normalized;
        float dist  = Vector2.Distance(rb.position, currentTarget);

        if (dist <= ARRIVE_DIST)
        {
            // Snap exactly to waypoint to eliminate drift
            rb.MovePosition(currentTarget);
            AdvanceTarget();
        }
        else
        {
            // Move toward waypoint — no physics forces, just direct velocity
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
            // Find lowest F
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

        return null; // no path
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
        if (result.Count > 0) result.RemoveAt(0); // skip tile enemy is already on
        return result;
    }

    // ─────────────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────────────
    bool IsWall(Vector3Int cell) => wallTilemap != null && wallTilemap.HasTile(cell);

    float Manhattan(Vector3Int a, Vector3Int b)
        => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

    // Returns world-space center of the tile the world position sits in
    Vector3 TileCenter(Vector3 worldPos)
    {
        Vector3Int cell = wallTilemap.WorldToCell(worldPos);
        return TileCenterFromCell(cell);
    }

    Vector3 TileCenterFromCell(Vector3Int cell)
    {
        // GetCellCenterWorld respects your tilemap's cell size & offset
        return wallTilemap.GetCellCenterWorld(cell);
    }

    // ─────────────────────────────────────────────────────
    //  A* Node (class so it can be mutated in open set)
    // ─────────────────────────────────────────────────────
    class ANode
    {
        public Vector3Int pos;
        public ANode      parent;
        public float      g;
        public float      h;
        public float      F => g + h;

        public ANode(Vector3Int p, ANode par, float g, float h)
        {
            pos = p; parent = par; this.g = g; this.h = h;
        }
    }
}
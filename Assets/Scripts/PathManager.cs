using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance;

    private LevelDefinition level;
    private Dictionary<Vector2Int, int> endpointColor = new Dictionary<Vector2Int, int>();
    private Dictionary<int, ColorPair> pairsById = new Dictionary<int, ColorPair>();

    private Dictionary<int, List<Vector2Int>> paths = new Dictionary<int, List<Vector2Int>>();
    private Dictionary<Vector2Int, int> cellOwner = new Dictionary<Vector2Int, int>();
    private Dictionary<int, LineRenderer> lines = new Dictionary<int, LineRenderer>();
    private HashSet<int> solvedColors = new HashSet<int>();

    private int activeColor = -1;
    private Vector2Int lastCell;
    private bool dragging = false;

    public float lineWidth = 0.25f;

    void Awake() { Instance = this; }

    public void Init(LevelDefinition lvl)
    {
        level = lvl;
        endpointColor.Clear();
        pairsById.Clear();
        paths.Clear();
        cellOwner.Clear();
        solvedColors.Clear();
        activeColor = -1;
        dragging = false;

        foreach (Transform child in transform) Destroy(child.gameObject);
        lines.Clear();

        foreach (var p in lvl.pairs)
        {
            endpointColor[p.start] = p.colorId;
            endpointColor[p.end] = p.colorId;
            pairsById[p.colorId] = p;
            paths[p.colorId] = new List<Vector2Int>();
            lines[p.colorId] = CreateLine(p.colorId);
        }
    }

    LineRenderer CreateLine(int colorId)
    {
        GameObject go = new GameObject($"Line_{colorId}");
        go.transform.SetParent(transform);
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = LevelDatabase.Colors[colorId];
        lr.startWidth = lr.endWidth = lineWidth;
        lr.positionCount = 0;
        lr.sortingOrder = 1;
        lr.useWorldSpace = true;
        return lr;
    }

    void Update()
    {
        if (level == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleDown(GetCellUnderPointer());
        }
        else if (Input.GetMouseButton(0) && dragging)
        {
            Vector2Int cell = GetCellUnderPointer();
            if (cell != lastCell)
            {
                HandleDrag(cell);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
            activeColor = -1;
        }
    }

    Vector2Int GetCellUnderPointer()
    {
        Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return GridManager.Instance.WorldToCell(world);
    }

    void HandleDown(Vector2Int cell)
    {
        if (!GridManager.Instance.IsInside(cell)) return;

        if (endpointColor.TryGetValue(cell, out int color))
        {
            StartNewPath(color, cell);
            return;
        }

        if (cellOwner.TryGetValue(cell, out int owner))
        {
            var path = paths[owner];
            int idx = path.IndexOf(cell);
            if (idx >= 0)
            {
                TruncatePath(owner, idx);
                activeColor = owner;
                lastCell = cell;
                dragging = true;
                if (solvedColors.Remove(owner))
                    GameManager.Instance.OnColorUnsolved(owner);
            }
        }
    }

    void StartNewPath(int color, Vector2Int startCell)
    {
        bool wasSolved = solvedColors.Remove(color);
        ClearPath(color);
        paths[color].Add(startCell);
        cellOwner[startCell] = color;
        activeColor = color;
        lastCell = startCell;
        dragging = true;
        if (wasSolved) GameManager.Instance.OnColorUnsolved(color);
        RedrawLine(color);
        CheckWinCondition();
    }

    void HandleDrag(Vector2Int cell)
    {
        if (!GridManager.Instance.IsInside(cell)) { lastCell = cell; return; }
        if (activeColor < 0) return;

        var path = paths[activeColor];
        if (path.Count == 0) return;

        if (path.Count >= 2 && cell == path[path.Count - 2])
        {
            Vector2Int removed = path[path.Count - 1];
            path.RemoveAt(path.Count - 1);
            cellOwner.Remove(removed);
            lastCell = cell;
            RedrawLine(activeColor);
            return;
        }

        Vector2Int head = path[path.Count - 1];
        int dx = Mathf.Abs(cell.x - head.x);
        int dy = Mathf.Abs(cell.y - head.y);
        bool adjacent = (dx + dy) == 1; 
        if (!adjacent) { lastCell = cell; return; }
        if (path.Contains(cell)) { lastCell = cell; return; }
        if (endpointColor.TryGetValue(cell, out int epColor) && epColor != activeColor)
        {
            lastCell = cell;
            return;
        }
        if (cellOwner.TryGetValue(cell, out int otherColor) && otherColor != activeColor)
        {
            bool otherWasSolved = solvedColors.Remove(otherColor);
            ClearPath(otherColor);
            if (otherWasSolved) GameManager.Instance.OnColorUnsolved(otherColor);
        }

        path.Add(cell);
        cellOwner[cell] = activeColor;
        lastCell = cell;
        RedrawLine(activeColor);
        var pair = pairsById[activeColor];
        Vector2Int otherEnd = (path[0] == pair.start) ? pair.end : pair.start;
        if (cell == otherEnd)
        {
            solvedColors.Add(activeColor);
            dragging = false;
            activeColor = -1;
        }

        CheckWinCondition();
    }
    void CheckWinCondition()
    {
        bool allConnected = solvedColors.Count == pairsById.Count;
        bool boardFull = cellOwner.Count == GridManager.Instance.Width * GridManager.Instance.Height;

        if (allConnected && boardFull)
            GameManager.Instance.OnLevelWin();
        else
            GameManager.Instance.OnColorUnsolved(-1);
    }

    void TruncatePath(int color, int index)
    {
        var path = paths[color];
        for (int i = path.Count - 1; i > index; i--)
        {
            cellOwner.Remove(path[i]);
            path.RemoveAt(i);
        }
        RedrawLine(color);
        CheckWinCondition();
    }

    void ClearPath(int color)
    {
        var path = paths[color];
        foreach (var c in path) cellOwner.Remove(c);
        path.Clear();
        RedrawLine(color);
    }

    void RedrawLine(int color)
    {
        var path = paths[color];
        var lr = lines[color];
        lr.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            lr.SetPosition(i, GridManager.Instance.CellToWorld(path[i]));
        }
    }
    public void ClearAll()
    {
        foreach (var colorId in paths.Keys.ToList())
        {
            ClearPath(colorId);
        }
        solvedColors.Clear();
        activeColor = -1;
        dragging = false;
    }
}

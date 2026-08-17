using UnityEngine;


public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Prefabs (optional manual assignment, otherwise auto-loaded from Resources)")]
    public GameObject tilePrefab;
    public GameObject nodePrefab;

    public float cellSize = 1f;

    public int Width { get; private set; }
    public int Height { get; private set; }

    void Awake()
    {
        Instance = this;
        if (tilePrefab == null) tilePrefab = Resources.Load<GameObject>("Tile");
        if (nodePrefab == null) nodePrefab = Resources.Load<GameObject>("Node");
    }

    public void BuildLevel(LevelDefinition level)
    {
        Clear();
        Width = level.width;
        Height = level.height;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector3 pos = CellToWorld(new Vector2Int(x, y));
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                tile.name = $"Tile_{x}_{y}";
                tile.transform.localScale = Vector3.one * cellSize * 0.92f;
                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = new Color(0.16f, 0.16f, 0.20f);
            }
        }

        foreach (var pair in level.pairs)
        {
            SpawnNode(pair.start, pair.colorId);
            SpawnNode(pair.end, pair.colorId);
        }

        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(
                (Width - 1) * cellSize / 2f,
                (Height - 1) * cellSize / 2f,
                -10f);

            float aspect = Camera.main.aspect;
            float sizeForHeight = Height * cellSize * 0.55f;
            float sizeForWidth = (Width * cellSize * 0.55f) / aspect;
            Camera.main.orthographicSize = Mathf.Max(sizeForHeight, sizeForWidth);
        }
    }

    void SpawnNode(Vector2Int cell, int colorId)
    {
        Vector3 pos = CellToWorld(cell);
        GameObject node = Instantiate(nodePrefab, pos, Quaternion.identity, transform);
        node.transform.localScale = Vector3.one * cellSize * 0.62f;
        SpriteRenderer sr = node.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = LevelDatabase.Colors[colorId];
        node.name = $"Node_{colorId}_{cell.x}_{cell.y}";
    }

    public Vector3 CellToWorld(Vector2Int cell)
    {
        return new Vector3(cell.x * cellSize, cell.y * cellSize, 0f);
    }

    public Vector2Int WorldToCell(Vector3 world)
    {
        int x = Mathf.RoundToInt(world.x / cellSize);
        int y = Mathf.RoundToInt(world.y / cellSize);
        return new Vector2Int(x, y);
    }

    public bool IsInside(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < Width && cell.y >= 0 && cell.y < Height;
    }

    void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}

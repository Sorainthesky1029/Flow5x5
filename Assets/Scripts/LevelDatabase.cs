using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ColorPair
{
    public int colorId;
    public Vector2Int start;
    public Vector2Int end;

    public ColorPair(int colorId, Vector2Int start, Vector2Int end)
    {
        this.colorId = colorId;
        this.start = start;
        this.end = end;
    }
}
[System.Serializable]
public class LevelDefinition
{
    public int width;
    public int height;
    public List<ColorPair> pairs;

    public LevelDefinition(int width, int height, List<ColorPair> pairs)
    {
        this.width = width;
        this.height = height;
        this.pairs = pairs;
    }
}
public static class LevelDatabase
{
    public static readonly Color[] Colors = new Color[]
    {
        new Color(0.90f, 0.20f, 0.20f), 
        new Color(0.20f, 0.45f, 0.95f), 
        new Color(0.95f, 0.85f, 0.20f), 
        new Color(0.25f, 0.80f, 0.30f), 
        new Color(0.95f, 0.55f, 0.15f), 
    };
    public static List<LevelDefinition> Levels = new List<LevelDefinition>
    {
        new LevelDefinition(5, 5, new List<ColorPair>
        {
            new ColorPair(0, new Vector2Int(0,0), new Vector2Int(4,1)),
            new ColorPair(1, new Vector2Int(0,2), new Vector2Int(4,3)),
            new ColorPair(2, new Vector2Int(0,4), new Vector2Int(4,4)),
        }),
        new LevelDefinition(5, 5, new List<ColorPair>
        {
            new ColorPair(0, new Vector2Int(0,0), new Vector2Int(4,1)),
            new ColorPair(1, new Vector2Int(0,2), new Vector2Int(4,2)),
            new ColorPair(2, new Vector2Int(0,3), new Vector2Int(4,3)),
            new ColorPair(3, new Vector2Int(0,4), new Vector2Int(4,4)),
        }),
        new LevelDefinition(6, 6, new List<ColorPair>
        {
            new ColorPair(0, new Vector2Int(0,0), new Vector2Int(5,0)),
            new ColorPair(1, new Vector2Int(0,2), new Vector2Int(5,2)),
            new ColorPair(2, new Vector2Int(0,4), new Vector2Int(5,4)),
            new ColorPair(3, new Vector2Int(0,5), new Vector2Int(5,5)),
        }),
        new LevelDefinition(6, 6, new List<ColorPair>
        {
            new ColorPair(0, new Vector2Int(0,0), new Vector2Int(5,0)),
            new ColorPair(1, new Vector2Int(0,2), new Vector2Int(5,2)),
            new ColorPair(2, new Vector2Int(0,4), new Vector2Int(5,4)),
            new ColorPair(3, new Vector2Int(0,5), new Vector2Int(2,5)),
            new ColorPair(4, new Vector2Int(3,5), new Vector2Int(5,5)),

        }),
        new LevelDefinition(7, 7, new List<ColorPair>
        {
            new ColorPair(0, new Vector2Int(0,0), new Vector2Int(6,1)),
            new ColorPair(1, new Vector2Int(0,2), new Vector2Int(6,3)),
            new ColorPair(2, new Vector2Int(0,4), new Vector2Int(6,5)),
            new ColorPair(3, new Vector2Int(0,6), new Vector2Int(3,6)),
            new ColorPair(4, new Vector2Int(4,6), new Vector2Int(6,6)),
        }),
    };
}

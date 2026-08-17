using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentLevelIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= LevelDatabase.Levels.Count)
        {
            UIManager.Instance.ShowAllLevelsComplete();
            return;
        }

        currentLevelIndex = index;
        var level = LevelDatabase.Levels[index];
        GridManager.Instance.BuildLevel(level);
        PathManager.Instance.Init(level);
        UIManager.Instance.OnLevelLoaded(index + 1, LevelDatabase.Levels.Count);
    }

    public void RestartLevel()
    {
        LoadLevel(currentLevelIndex);
    }

    public void NextLevel()
    {
        LoadLevel(currentLevelIndex + 1);
    }

    public void OnColorUnsolved(int colorId)
    {
        UIManager.Instance.HideNextButton();
    }

    public void OnLevelWin()
    {
        UIManager.Instance.ShowNextButton();
    }
}

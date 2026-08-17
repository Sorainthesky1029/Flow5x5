using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("References (assign in Inspector)")]
    public GameObject nextButton;
    public Text levelText;
    public GameObject winLabel; 

    void Awake() { Instance = this; }

    void Start()
    {
        HideNextButton();
    }

    public void OnLevelLoaded(int levelNumber, int totalLevels)
    {
        if (levelText != null) levelText.text = $"Level {levelNumber} / {totalLevels}";
        HideNextButton();
    }

    public void ShowNextButton()
    {
        if (nextButton != null) nextButton.SetActive(true);
        if (winLabel != null) winLabel.SetActive(true);
    }

    public void HideNextButton()
    {
        if (nextButton != null) nextButton.SetActive(false);
        if (winLabel != null) winLabel.SetActive(false);
    }

    public void ShowAllLevelsComplete()
    {
        if (levelText != null) levelText.text = "All levels complete!";
        HideNextButton();
    }
}

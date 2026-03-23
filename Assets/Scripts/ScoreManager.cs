using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static int currentScore = 0;
    public TextMeshProUGUI scoreText;

    void Start()
    {
        currentScore = 0; // Réinitialise au début
    }

    void Update()
    {
        scoreText.text = "SCORE: " + currentScore;
    }

    public static void AddPoints(int points)
    {
        currentScore += points;
    }
}
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static Score instance;

    private int scoreCount;

    [SerializeField]
    private Text text;

    public int ScoreCount
    {
        set
        {
            scoreCount += value;
            text.text = "Score: " + scoreCount;
        }
    }

    private void Start()
    {
        instance = this;
        scoreCount = 0;
    }

    public void AddToScore(int toScore)
    {
        int i = toScore;

        if (toScore == 3)
        {
            ScoreCount = 30;
        }
        else if (toScore > 3)
        {
            i -= 3;
            ScoreCount = 30;
            ScoreCount = i * 15;
        }
    }
}

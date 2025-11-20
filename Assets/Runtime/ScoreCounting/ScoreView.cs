using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private Text _scoreText;

    public void UpdateScore(int score)
    {
        if (_scoreText != null)
        {
            _scoreText.text = $"Score: {score}";
        }
    }
}

using Infrastructure.Data;
using Infrastructure.States;
using TMPro;
using UnityEngine;

namespace UI.Entities
{
    public class ScoreContainer : Window, ISavedProgress
    {
        public TextMeshProUGUI ScoreText;
        public int Score { get; private set; } = 1;
        public int HighestScore { get; private set; }

        public void RaiseScore()
        {
            Score++;
            Debug.Log(HighestScore);
            if (Score > HighestScore) 
                HighestScore = Score;

            ScoreText.text = Score.ToString();
        }

        public void ResetScore()
        {
            Score = 1;
            ScoreText.text = "1";
        }

        public void LoadProgress(PlayerProgress progress)
        {
            HighestScore = progress.HighestScore;
            Debug.Log("Highest score on load: " + HighestScore);
        }

        public void UpdateProgress(PlayerProgress progress)
        {
            progress.HighestScore = HighestScore;
            Debug.Log("Highest score on save: " + HighestScore);
        }
    }
}
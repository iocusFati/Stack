using System.Collections;
using Infrastructure.Data.UIData;
using TMPro;
using UnityEngine;

namespace Infrastructure.States
{
    public class LoseWindow : Window
    {
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private TextMeshProUGUI _record;
        [SerializeField] private TextMeshProUGUI _replayLabel;

        private Color _replayColor1;
        private Color _replayColor2;
        private float _blinkDuration;

        private int _currRecord;
        
        private Coroutine _labelBlink;

        public void Construct(UIStaticData UIData)
        {
            _replayColor1 = UIData.ReplayLabelColor1;
            _replayColor2 = UIData.ReplayLabelColor2;
            _blinkDuration = UIData.ReplayBlinkDuration;
        }

        public override void Show()
        {
            base.Show();
            _labelBlink = StartCoroutine(LabelBlink(_replayColor1, _replayColor2, _blinkDuration));
        }

        public override void Hide()
        {
            base.Hide();
            StopCoroutine(_labelBlink);
        }

        public void SetScore(int score, int recordScore)
        {
            _score.text = new string($"Score: {score}");
            _record.text = new string($"Record: {recordScore}");
        }
        
        private IEnumerator LabelBlink(Color color1, Color color2, float duration)
        {
            while (true)
            {
                for (float time = 0 ; time < duration * 2 ; time += Time.deltaTime)
                {
                    float progress = Mathf.PingPong(time, duration) / duration;
                    _replayLabel.color = Color.Lerp(color1, color2, progress - 0.1f);
                
                    yield return null;
                }
            }
        }
    }
}

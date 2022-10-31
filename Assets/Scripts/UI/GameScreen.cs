using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private TextMeshProUGUI _currentHPText;

    // [SerializeField] private Slider _explosionSlider;

    [SerializeField] private RectMask2D _explosionSlider;
    void Awake()
    {
        GameloopManager.instance.OnFishTakeHit += UpdateCurrentHPText;

        void UpdateHighScoreOnStart()
        {
            UpdateHighScoreText();
            UpdateCurrentHPText(5); //this is bad, but I don't have time to fix it
            GameloopManager.instance.OnGameLoopStart -= UpdateHighScoreOnStart;
        };

        GameloopManager.instance.OnGameLoopStart += UpdateHighScoreOnStart;


        GameloopManager.instance.OnKillEnemy += UpdateHighScoreText;
    }


    private void LateUpdate()
    {
        if (!GameloopManager.instance.LoopIsActive) return;
        UpdateExplosionBar();
    }

    private const string _highScoreTextPrefix = "High Score \n";
    private void UpdateHighScoreText()
    {
        string newValue = string.Format("{0:000000}", GameloopManager.instance.CollectedHightScore);
        _highScoreText.text = _highScoreTextPrefix + newValue;

    }

    private const string _currentHPTextPrefix = "HP: ";
    private void UpdateCurrentHPText(int currentHP)
    {
        _currentHPText.text = _currentHPTextPrefix + currentHP.ToString();
    }

    private void UpdateExplosionBar()
    {
        // if (_explosionSlider.value < 1)
        // {
        var finalVal = Vector4.zero;
        finalVal.w = Mathf.Lerp(153, 0, GameloopManager.instance.ExplosionBarTracker.GetRatio());
        _explosionSlider.padding = finalVal;
        // }
    }
}

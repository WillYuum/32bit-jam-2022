using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
            UpdateCurrentHPText(GameloopManager.instance.FishHP); //this is bad, but I don't have time to fix it
            GameloopManager.instance.OnGameLoopStart -= UpdateHighScoreOnStart;
        };

        GameloopManager.instance.OnGameLoopStart += () =>
        {
            GameloopManager.instance.OnKillEnemy += UpdateHighScoreText;
            GameloopManager.instance.ExplosionBarTracker.OnUpdate += UpdateExplosionBar;
            UpdateHighScoreOnStart();
        };

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

    [SerializeField] private GameObject _boomImage;
    private void UpdateExplosionBar()
    {
        float ratio = GameloopManager.instance.ExplosionBarTracker.GetRatio();

        var finalVal = Vector4.zero;
        finalVal.w = Mathf.Lerp(153, 0, ratio);
        _explosionSlider.padding = finalVal;


        if (ratio >= 1)
        {
            _boomImage.SetActive(true);
            _boomImage.transform.DOShakeScale(0.5f, 0.5f, 10, 90, false);
        }
        else
        {
            _boomImage.SetActive(false);
        }
    }
}

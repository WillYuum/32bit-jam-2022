using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class GameScreen : MonoBehaviour
{
    public struct LoadConfig
    {
        public Action<Action> OpenScreen;
    }

    private const string _highScoreTextPrefix = "High Score \n";
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private TextMeshProUGUI _currentHPText;


    [SerializeField] private GameObject _boomImage;
    [SerializeField] private RectMask2D _explosionSlider;
    private Color _orangeLightColor = new Color(238.0f / 255.0f, 189.0f / 255.0f, 92.0f / 255.0f, 255.0f / 255.0f);


    [SerializeField] private Slider _momentumBar;

    private void OnEnable()
    {
        print("GameScreen.OnEnable");
    }

    private void OnDisable()
    {
        print("GameScreen.OnDisable");
    }

    public LoadConfig Load()
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
            UpdateExplosionBar();
        };

        GameloopManager.instance.OnMomentumChange += UpdateMomentumBar;

        return new LoadConfig()
        {
            OpenScreen = (cb) =>
            {
                gameObject.SetActive(true);

                cb.Invoke();
            }
        };

    }


    private void UpdateHighScoreText()
    {
        string newValue = string.Format("{0:000000}", GameloopManager.instance.CollectedHightScore);
        _highScoreText.text = _highScoreTextPrefix + newValue;

        _highScoreText.DOColor(_orangeLightColor, 0.1f).OnComplete(ResetToColorWhite);
    }

    private void ResetToColorWhite()
    {
        _highScoreText.DOColor(Color.white, 0.1f);
        _currentHPText.DOColor(Color.white, 0.1f);
    }

    private const string _currentHPTextPrefix = "HP: ";
    private void UpdateCurrentHPText(int currentHP)
    {
        _currentHPText.text = _currentHPTextPrefix + currentHP.ToString();
        _currentHPText.DOColor(_orangeLightColor, 0.1f).OnComplete(ResetToColorWhite);
    }

    private void UpdateExplosionBar()
    {
        float ratio = GameloopManager.instance.ExplosionBarTracker.GetRatio();

        Vector4 finalVal = Vector4.zero;
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

    private void UpdateMomentumBar(float ratio)
    {
        _momentumBar.value = ratio;
    }
}

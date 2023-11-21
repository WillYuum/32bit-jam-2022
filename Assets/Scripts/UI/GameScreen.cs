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


    private Color _orangeLightColor = new(238.0f / 255.0f, 189.0f / 255.0f, 92.0f / 255.0f, 255.0f / 255.0f);

    private BigBoomCanvas _bigBoomCanvas;

    [SerializeField] private Slider _momentumBar;

    public LoadConfig Load()
    {
        GameloopManager.instance.OnFishTakeHit += UpdateCurrentHPText;

        _bigBoomCanvas = GameObject.FindObjectOfType<BigBoomCanvas>(true);
        _bigBoomCanvas.Toggle(true);

        void UpdateHighScoreOnStart()
        {
            UpdateHighScoreText();
            UpdateCurrentHPText(GameloopManager.instance.FishHP); //this is bad, but I don't have time to fix it
            GameloopManager.instance.OnGameLoopStart -= UpdateHighScoreOnStart;
        };

        GameloopManager.instance.OnGameLoopStart += () =>
        {
            GameloopManager.instance.OnKillEnemy += UpdateHighScoreText;
            GameloopManager.instance.ExplosionBarTracker.OnUpdate += _bigBoomCanvas.UpdateExplosionBar;
            UpdateHighScoreOnStart();
            _bigBoomCanvas.UpdateExplosionBar();
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

    private void UpdateMomentumBar(float ratio)
    {
        _momentumBar.value = ratio;
    }


    public void EnableTapOnButton(Action cb)
    {
        _bigBoomCanvas.EnableTapOnButton(cb);
    }

    public void DisableTapOnButton()
    {
        _bigBoomCanvas.DisableTapOnButton();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class LoseScreen : MonoBehaviour
{

    public struct LoadConfig
    {
        public Action<Action> OpenScreen;
    }

    [SerializeField] private Image _background;
    [SerializeField] private Button _restartButton;
    [SerializeField] private TextMeshProUGUI _highScoreText;

    private void Awake()
    {
        _restartButton.onClick.AddListener(OnClickRestartButton);
    }

    void OnEnable()
    {
        _background.DOColor(new Color(0, 0, 0, 0.85f), 1.5f);
        RenderLoseScreen();
    }

    public LoadConfig Load()
    {
        LoadConfig config = new LoadConfig();

        config.OpenScreen = (cb) =>
        {
            gameObject.SetActive(true);
            cb.Invoke();
        };

        return config;
    }

    private void RenderLoseScreen()
    {
        string newValue = string.Format("{0:000000}", GameloopManager.instance.CollectedHightScore);
        _highScoreText.text = newValue;
    }

    private void OnClickRestartButton()
    {
        GameManager.instance.RestartScene();
        // GameManager.instance.SwitchToScene(GameScenes.MainMenu);
    }



}

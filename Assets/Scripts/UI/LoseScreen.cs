using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoseScreen : MonoBehaviour
{
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

    private void RenderLoseScreen()
    {
        string newValue = string.Format("{0:000000}", GameloopManager.instance.CollectedHightScore);
        _highScoreText.text = newValue;
    }

    private void OnClickRestartButton()
    {
        GameManager.instance.RestartScene();
    }



}

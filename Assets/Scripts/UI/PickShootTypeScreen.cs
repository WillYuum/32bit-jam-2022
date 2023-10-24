using UnityEngine;
using System;
using TMPro;
using DG.Tweening;
using System.Runtime.InteropServices;

public class PickShootTypeScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countDownVisual;
    [SerializeField] private TextMeshProUGUI _tutorialText;

    public struct LoadConfig
    {
        public Action OpenScreen;
        public Action<Action> StartCountDown;
    }



    public LoadConfig Load()
    {
        LoadConfig config = new LoadConfig();

        _countDownVisual.gameObject.SetActive(false);

        config.OpenScreen = () =>
        {
            _tutorialText.gameObject.SetActive(true);
            gameObject.SetActive(true);
        };

        config.StartCountDown = (cb) =>
        {
            StartShowingCountDownVisuals(cb);
        };

        return config;
    }


    private void StartShowingCountDownVisuals(Action cb)
    {
        _tutorialText.gameObject.SetActive(false);
        _countDownVisual.gameObject.SetActive(true);

        string tutorialText = "3";
        _countDownVisual.text = tutorialText;

        _countDownVisual.transform.DOScale(1.5f, 0.5f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        {
            tutorialText = "2";
            _countDownVisual.text = tutorialText;

            _countDownVisual.transform.DOScale(1.5f, 0.5f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                tutorialText = "1";
                _countDownVisual.text = tutorialText;

                _countDownVisual.transform.DOScale(1.5f, 0.5f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                {
                    tutorialText = "GO!";
                    _countDownVisual.text = tutorialText;

                    _countDownVisual.transform.DOScale(1.5f, 0.5f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                    {
                        HideVisuals();
                        cb.Invoke();
                    });
                });
            });
        });
    }


    private void HideVisuals()
    {
        gameObject.SetActive(false);
        enabled = false;
    }
}

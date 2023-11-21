using UnityEngine;
using System;
using TMPro;
using DG.Tweening;

public class PickShootTypeScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countDownVisual;
    [SerializeField] private TextMeshProUGUI _tutorialText;
    [SerializeField] private TextMeshProUGUI _tutorialTextInMobile;


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
            ShowTutorialText(GameManager.instance.PlayedOnMobileBrowser());
            gameObject.SetActive(true);
        };

        config.StartCountDown = (cb) =>
        {
            StartShowingCountDownVisuals(cb);
        };

        return config;
    }


    private void ShowTutorialText(bool onMobile)
    {
        _tutorialTextInMobile.gameObject.SetActive(onMobile);
        _tutorialText.gameObject.SetActive(!onMobile);
    }


    private void StartShowingCountDownVisuals(Action cb)
    {
        _tutorialText.gameObject.SetActive(false);
        _tutorialTextInMobile.gameObject.SetActive(false);
        _countDownVisual.gameObject.SetActive(true);


        float maxScale = 2.5f;
        float scaleDuration = 0.5f;
        int loops = 2;
        LoopType loopType = LoopType.Yoyo;

        string tutorialText = "3";
        _countDownVisual.text = tutorialText;

        _countDownVisual.transform.DOScale(maxScale, scaleDuration).SetLoops(loops, loopType).OnComplete(() =>
        {
            tutorialText = "2";
            _countDownVisual.text = tutorialText;

            _countDownVisual.transform.DOScale(maxScale, scaleDuration).SetLoops(loops, loopType).OnComplete(() =>
            {
                tutorialText = "1";
                _countDownVisual.text = tutorialText;

                _countDownVisual.transform.DOScale(maxScale, scaleDuration).SetLoops(loops, loopType).OnComplete(() =>
                {
                    tutorialText = "GO!";
                    _countDownVisual.text = tutorialText;

                    _countDownVisual.transform.DOScale(maxScale, scaleDuration).SetLoops(loops, loopType).OnComplete(() =>
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

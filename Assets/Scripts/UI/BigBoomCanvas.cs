using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BigBoomCanvas : MonoBehaviour
{
    [SerializeField] private GameObject _boomImage;
    [SerializeField] private RectMask2D _explosionSlider;
    [SerializeField] private Button _triggerBoomButton;

    private Vector2 _boomImageOriginalScale;


    private void Awake()
    {
        _boomImageOriginalScale = _boomImage.transform.localScale;
    }

    public void Toggle(bool toggle)
    {
        gameObject.SetActive(toggle);
    }

    private const int _maxExplosionBarWidth = 153;
    private const int _minExplosionBarWidth = 0;
    public void UpdateExplosionBar()
    {
        float ratio = GameloopManager.instance.ExplosionBarTracker.GetRatio();

        Vector4 finalVal = Vector4.zero;
        finalVal.w = Mathf.Lerp(_maxExplosionBarWidth, _minExplosionBarWidth, ratio);
        _explosionSlider.padding = finalVal;


        if (ratio >= 1)
        {
            transform.DOKill();

            _boomImage.transform.localScale = _boomImageOriginalScale;
            _boomImage.SetActive(true);
            _boomImage.transform.DOShakeScale(0.5f, 0.5f, 10, 90, false);
        }
        else
        {
            _boomImage.SetActive(false);
        }
    }

    public void EnableTapOnButton(Action cb)
    {
        DisableTapOnButton();
        _triggerBoomButton.onClick.AddListener(() =>
        {
            cb.Invoke();
        });
    }

    public void DisableTapOnButton()
    {
        _triggerBoomButton.onClick.RemoveAllListeners();
    }
}

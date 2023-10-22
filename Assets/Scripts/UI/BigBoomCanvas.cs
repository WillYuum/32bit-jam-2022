using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BigBoomCanvas : MonoBehaviour
{
    [SerializeField] private GameObject _boomImage;
    [SerializeField] private RectMask2D _explosionSlider;

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
            _boomImage.SetActive(true);
            _boomImage.transform.DOShakeScale(0.5f, 0.5f, 10, 90, false);
        }
        else
        {
            _boomImage.SetActive(false);
        }
    }


}

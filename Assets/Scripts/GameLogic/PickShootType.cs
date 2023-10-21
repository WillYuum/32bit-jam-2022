using System;
using UnityEngine;
using DG.Tweening;

public class PickShootType : MonoBehaviour
{
    private Action<TypeOfShots> _onSelectType;
    [SerializeField] private GameObject[] shotTypeVisuals;

    public struct LoadConfig
    {
        public Action<Action<TypeOfShots>> WaitToSelectShootType;
    }

    public LoadConfig Load()
    {
        EnableVisuals();

        _onSelectType = null;
        return new LoadConfig
        {
            WaitToSelectShootType = (cb) =>
            {
                _onSelectType += (result) =>
                {
                    cb.Invoke(result);
                    _onSelectType = null;
                };
            }
        };
    }

    private void EnableVisuals()
    {
        gameObject.SetActive(true);
        enabled = true;

        //tween scale up and down in loop for shotTypeVisuals
        foreach (var shotTypeVisual in shotTypeVisuals)
        {
            shotTypeVisual.transform.DOScale(1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void HideVisuals()
    {
        gameObject.SetActive(false);
        enabled = false;

        //stop tweening on shotTypeVisuals
        foreach (var shotTypeVisual in shotTypeVisuals)
        {
            shotTypeVisual.transform.DOKill();
        }


    }


    private TypeOfShots GetTypeShotFromPosition(Vector2 playerPosition)
    {
        return playerPosition.x > 0 ? TypeOfShots.Laser : TypeOfShots.PeaShots;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            TypeOfShots typeOfShot = GetTypeShotFromPosition(other.transform.position);
            _onSelectType.Invoke(typeOfShot);
            HideVisuals();
        }
    }
}

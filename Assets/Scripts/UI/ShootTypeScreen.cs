using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShootTypeScreen : MonoBehaviour
{

    public struct LoadConfig
    {
        public Action<Action<TypeOfShots>> OpenScreen;

    }


    [System.Serializable]
    class ShootTypeSide
    {
        [SerializeField] public Image _backgroundPanel;
    }




    [SerializeField] private ShootTypeSide _peaShootSide;
    [SerializeField] private ShootTypeSide _laserShootSide;



    private void Update()
    {
        IncreaseAlphaOnMouseOver();

        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.x < Screen.width / 2)
            {
                SelectShotType(TypeOfShots.PeaShots);
            }
            else
            {
                SelectShotType(TypeOfShots.Laser);
            }
        }
    }



    private Action<TypeOfShots> onSelectType;
    public LoadConfig Load()
    {
        LoadConfig loadConfig = new LoadConfig();

        loadConfig.OpenScreen = (cb) =>
        {
            gameObject.SetActive(true);

            onSelectType += (shootType) =>
            {
                cb.Invoke(shootType);
            };
        };

        return loadConfig;
    }


    private void SelectShotType(TypeOfShots typeOfShots)
    {
        onSelectType?.Invoke(typeOfShots);
        gameObject.SetActive(false);
    }

    private void IncreaseAlphaOnMouseOver()
    {
        Color peaShooterColor = _peaShootSide._backgroundPanel.color;
        Color laserShooterColor = _laserShootSide._backgroundPanel.color;

        if (Input.mousePosition.x < Screen.width / 2)
        {
            peaShooterColor.a = 0.85f;
            laserShooterColor.a = 0.4f;
        }
        else
        {
            peaShooterColor.a = 0.4f;
            laserShooterColor.a = 0.85f;
        }

        _peaShootSide._backgroundPanel.color = peaShooterColor;
        _laserShootSide._backgroundPanel.color = laserShooterColor;
    }

}

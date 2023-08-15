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

    private TypeOfShots _currentTypeOfShots;


    private void Update()
    {
        IncreaseAlphaOnMouseOver();


        //switch types by pressing K or S
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            if (_currentTypeOfShots == TypeOfShots.PeaShots)
            {
                _currentTypeOfShots = TypeOfShots.Laser;
            }
            else
            {
                _currentTypeOfShots = TypeOfShots.PeaShots;
            }

            IncreaseAlphaOnMouseOver();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SelectShotType(_currentTypeOfShots);
        }
    }



    private Action<TypeOfShots> onSelectType;
    public LoadConfig Load()
    {

        onSelectType = null;

        LoadConfig loadConfig = new LoadConfig();

        _currentTypeOfShots = TypeOfShots.PeaShots;

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
        if (onSelectType != null)
        {
            onSelectType.Invoke(typeOfShots);
        }
        gameObject.SetActive(false);
        onSelectType = null;
    }

    private void IncreaseAlphaOnMouseOver()
    {
        Color peaShooterColor = _peaShootSide._backgroundPanel.color;
        Color laserShooterColor = _laserShootSide._backgroundPanel.color;

        if (_currentTypeOfShots == TypeOfShots.PeaShots)
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

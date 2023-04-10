using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenuScene
{
    public class MainMenu : MonoBehaviour
    {

        public struct LoadConfig
        {
            public Action OpenScreen;
        }

        [SerializeField] private MainMenuScreens _mainMenuScreens;

        [SerializeField] private Button _playButton;
        [SerializeField] private Button _creditsButton;
        [SerializeField] private Button _exitButton;


        public LoadConfig LoadUpScreen(Action onClickPlay)
        {
            LoadConfig loadConfig = new LoadConfig();

            loadConfig.OpenScreen = () =>
            {
                gameObject.SetActive(true);

                _creditsButton.onClick.AddListener(OnCreditsButtonClicked);
                _exitButton.onClick.AddListener(OnExitButtonClicked);

                _mainMenuScreens.ShowScreen(MainMenuScreens.ScreenType.MainScreen);
            };


            _playButton.onClick.AddListener(() =>
            {
                _creditsButton.onClick.RemoveListener(OnCreditsButtonClicked);
                _exitButton.onClick.RemoveListener(OnExitButtonClicked);

                onClickPlay.Invoke();
            });

            return loadConfig;
        }

        private void OnCreditsButtonClicked()
        {
            _mainMenuScreens.ShowScreen(MainMenuScreens.ScreenType.Credits);
        }

        private void OnExitButtonClicked()
        {
            GameManager.instance.ExitGame();
        }

    }


    [System.Serializable]
    public class MainMenuScreens
    {
        public enum ScreenType
        {
            MainScreen,
            Credits
        }

        [SerializeField] private GameObject _gameScreen;
        [SerializeField] private GameObject _creditsScreen;

        private GameObject _currentOpenScreen;


        public void ShowScreen(ScreenType screen)
        {
            if (_currentOpenScreen != null)
            {
                _currentOpenScreen.SetActive(false);
            }

            switch (screen)
            {
                case ScreenType.MainScreen:
                    _currentOpenScreen = _gameScreen;
                    break;
                case ScreenType.Credits:
                    _currentOpenScreen = _creditsScreen;
                    break;
                default:
                    break;
            }

            _currentOpenScreen.SetActive(true);
        }

    }
}
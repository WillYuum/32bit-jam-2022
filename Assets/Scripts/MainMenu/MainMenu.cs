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
            public Action<Action> OpenScreen;
            public Action<Action> OnPlayButtonClicked;
        }

        [SerializeField] private MainMenuScreens _mainMenuScreens;

        [SerializeField] private Button _playButton;
        [SerializeField] private Button _creditsButton;
        [SerializeField] private Button _exitButton;


        public LoadConfig LoadUpScreen()
        {
            LoadConfig loadConfig = new LoadConfig();

            loadConfig.OpenScreen = (cb) =>
            {
                gameObject.SetActive(true);

                _creditsButton.onClick.AddListener(OnCreditsButtonClicked);
                _exitButton.onClick.AddListener(OnExitButtonClicked);

                _mainMenuScreens.ShowScreen(MainMenuScreens.ScreenType.MainScreen);

                cb += () =>
                {
                    _creditsButton.onClick.RemoveListener(OnCreditsButtonClicked);
                    _exitButton.onClick.RemoveListener(OnExitButtonClicked);
                };

                cb.Invoke();
            };


            loadConfig.OnPlayButtonClicked = (cb) =>
            {
                cb.Invoke();
            };

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
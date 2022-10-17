using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenuScene
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private MainMenuScreens _mainMenuScreens;

        [SerializeField] private Button _playButton;
        [SerializeField] private Button _creditsButton;
        [SerializeField] private Button _exitButton;

        private void Awake()
        {
            _playButton.onClick.AddListener(OnPlayButtonClicked);
            _creditsButton.onClick.AddListener(OnCreditsButtonClicked);
            _exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        private void Start()
        {
            _mainMenuScreens.ShowScreen(MainMenuScreens.ScreenType.MainScreen);
        }


        private void OnPlayButtonClicked()
        {
            GameManager.instance.SwitchToScene(GameScenes.Game);
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
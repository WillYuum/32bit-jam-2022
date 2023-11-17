using System;
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


        public LoadConfig LoadUpScreen(Action onClickPlay)
        {
            LoadConfig loadConfig = new LoadConfig();

            loadConfig.OpenScreen = () =>
            {
                gameObject.SetActive(true);

                _mainMenuScreens.ShowScreen(MainMenuScreens.ScreenType.MainScreen);
            };


            _playButton.onClick.AddListener(() =>
            {
                onClickPlay.Invoke();
            });

            return loadConfig;
        }
    }


    [System.Serializable]
    public class MainMenuScreens
    {
        public enum ScreenType
        {
            MainScreen,
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

                default:
                    break;
            }

            _currentOpenScreen.SetActive(true);
        }

    }
}
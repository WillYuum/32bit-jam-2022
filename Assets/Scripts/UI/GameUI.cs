using UnityEngine;
using Utils.GenericSingletons;

public class GameUI : MonoBehaviourSingleton<GameUI>
{
    public enum Screens
    {
        PauseScreen,
        GameUI,
        LoseScreen,
    }

    [SerializeField] private GameObject _pauseScreen;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private GameObject _loseScreen;

    private GameObject _currentActiveScreen = null;

    public void SwitchToScreen(GameUI.Screens screen)
    {
        if (_currentActiveScreen != null) _currentActiveScreen.SetActive(false);

        switch (screen)
        {
            case Screens.PauseScreen:
                _currentActiveScreen = _pauseScreen.gameObject;
                break;
            case Screens.GameUI:
                _currentActiveScreen = _gameUI.gameObject;
                break;
            case Screens.LoseScreen:
                _currentActiveScreen = _loseScreen.gameObject;
                break;

        }

        _currentActiveScreen.SetActive(true);
    }
}

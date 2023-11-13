using UnityEngine;
using Utils.GenericSingletons;

public class GameUI : MonoBehaviourSingleton<GameUI>
{
    public enum Screens
    {
        PauseScreen,
        GameUI,
        LoseScreen,
        PickShootType,
    }

    [SerializeField] private GameObject _pauseScreen;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private GameObject _loseScreen;
    [SerializeField] private GameObject _pickShootTypePrefab;

    private GameObject _currentActiveScreen = null;


    public void Init()
    {
        GameloopManager.instance.OnRestartGame += () =>
        {
            _currentActiveScreen = null;
        };


        //Hide all screens
        _pauseScreen.SetActive(false);
        _gameUI.SetActive(false);
        _loseScreen.SetActive(false);
        _pickShootTypePrefab.SetActive(false);
    }

    private void SwitchToScreen(GameUI.Screens screen)
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
            case Screens.PickShootType:
                _currentActiveScreen = _pickShootTypePrefab.gameObject;
                break;
        }

        _currentActiveScreen?.SetActive(true);
    }

    public PickShootTypeScreen.LoadConfig LoadPickShotTypeScreen()
    {
        SwitchToScreen(Screens.PickShootType);
        return _pickShootTypePrefab.GetComponent<PickShootTypeScreen>().Load();
    }


    public GameScreen.LoadConfig LoadGameScreen()
    {
        SwitchToScreen(Screens.GameUI);
        return _gameUI.GetComponent<GameScreen>().Load();
    }

    public LoseScreen.LoadConfig LoadLoseScreen()
    {
        SwitchToScreen(Screens.LoseScreen);
        return _loseScreen.GetComponent<LoseScreen>().Load();
    }



    public T GetCurrentScreen<T>() where T : MonoBehaviour
    {
        return _currentActiveScreen.GetComponent<T>();
    }
}

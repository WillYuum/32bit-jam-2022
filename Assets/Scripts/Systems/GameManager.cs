using System;
using SpawnManagerMod.Configs;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.GenericSingletons;

public enum GameScenes
{
    MainMenu,
    Game,
}

public enum GameFlowState
{
    MainMenu,
    PickShootType,
    Game,
    Lose,
}

public class GameManager : MonoBehaviourSingleton<GameManager>
{

    private PlayerActionsController _turretActionsController;

    private void Awake()
    {
        AudioManager.instance.Load();

        _turretActionsController = gameObject.GetComponent<PlayerActionsController>();
        _turretActionsController.Init();

        string deviceId = SystemInfo.deviceUniqueIdentifier;
        transform.parent.GetComponentInChildren<AnalyticsManager>().SendAnalyticsData(deviceId, (result, context) =>
        {
            print("Analtyics result: " + result + " context: " + context);
        });
    }

    void Start()
    {
        print("GameManager Start");


#if UNITY_EDITOR
        //check current scene and load the correct screen
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            LoadGameFromMainMenu();
        }
        else if (SceneManager.GetActiveScene().name == "MainScene")
        {
            LoadGameFromMainGame();
        }
#else
        LoadGameFromMainMenu();
#endif
    }

    [SerializeField] private PrefabConfig _pickShootTypePrefab;

    private void LoadGameFromMainGame()
    {
        GameUI.instance.Init();
        EnterPickShootTypeState();
    }

    private void EnterPickShootTypeState()
    {
        GameObject pickScreenLogicObject = _pickShootTypePrefab.CreateGameObject(Vector3.zero, Quaternion.identity);
        PickShootType.LoadConfig loadConfig = pickScreenLogicObject.GetComponent<PickShootType>().Load();

        _turretActionsController.SwitchToActions(GameFlowState.PickShootType);
        var pickShooTypeUI = GameUI.instance.LoadPickShotTypeScreen();


        pickShooTypeUI.OpenScreen();
        loadConfig.WaitToSelectShootType((shootType) =>
        {

            pickShooTypeUI.StartCountDown(() =>
            {
                var gameScreen = GameUI.instance.LoadGameScreen();
                _turretActionsController.SwitchToActions(GameFlowState.Game);
                GameloopManager.instance.StartGameLoop(new StartGameLoopStruct
                {
                    SelectTypeShot = shootType
                });
            });
        });
    }


    private void LoadGameFromMainMenu()
    {
        void onClickPlay()
        {
            GameManager.instance.SwitchToScene(GameScenes.Game, EnterPickShootTypeState);
        }

        _turretActionsController.SwitchToActions(GameFlowState.MainMenu);

        var mainMenuActions = GameObject.FindObjectOfType<MainMenuScene.MainMenu>().LoadUpScreen(onClickPlay);
        mainMenuActions.OpenScreen();
    }



    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameloopManager.instance.InvokeRestartGame();

        //Looks like a race condition bug that I need to fix later
        Invoke(nameof(DelayStartGame), 0.1f);
    }

    private void DelayStartGame()
    {
        LoadGameFromMainGame();
    }

    public void LoseGame()
    {

    }

    public void ExitGame()
    {
        Application.Quit();
    }


    public void SwitchToScene(GameScenes scene, Action cb = null)
    {

        string sceneName = scene switch
        {
            GameScenes.MainMenu => "MainMenu",
            GameScenes.Game => "MainScene",
            _ => ""
        };


        void onSceneLoaded(Scene loadedScene, LoadSceneMode mode)
        {
            if (loadedScene.name == sceneName)
            {
                cb?.Invoke();
            }

            SceneManager.sceneLoaded -= onSceneLoaded;
        }

        SceneManager.sceneLoaded += onSceneLoaded;

        SceneManager.LoadScene(sceneName);
    }
}

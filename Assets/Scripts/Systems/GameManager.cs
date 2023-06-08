using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.GenericSingletons;

public enum GameScenes
{
    MainMenu,
    Game,
}

public class GameManager : MonoBehaviourSingleton<GameManager>
{

    private void Awake()
    {
        AudioManager.instance.Load();
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


    private void LoadGameFromMainGame()
    {
        GameUI.instance.Init();

        var selectScreenUI = GameUI.instance.LoadSelectShootType();
        selectScreenUI.OpenScreen((shootType) =>
        {
            var gameScreen = GameUI.instance.LoadGameScreen();
            gameScreen.OpenScreen(() =>
            {
                GameloopManager.instance.StartGameLoop(new StartGameLoopStruct
                {
                    SelectTypeShot = shootType
                });
            });
        });
    }



    private void LoadGameFromMainMenu()
    {
        void startGameScene()
        {
            var selectScreenUI = GameUI.instance.LoadSelectShootType();
            selectScreenUI.OpenScreen((shootType) =>
            {
                var gameScreen = GameUI.instance.LoadGameScreen();
                gameScreen.OpenScreen(() =>
                {
                    GameloopManager.instance.StartGameLoop(new StartGameLoopStruct
                    {
                        SelectTypeShot = shootType
                    });
                });
            });
        }

        void onClickPlay()
        {
            GameManager.instance.SwitchToScene(GameScenes.Game, startGameScene);
        }

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

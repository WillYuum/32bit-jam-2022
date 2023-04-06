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
        var selectScreenUI = GameUI.instance.LoadSelectScreen();
        selectScreenUI.OpenScreen((shootType) =>
        {
            GameloopManager.instance.SetShootType(shootType);
            GameloopManager.instance.StartGameLoop();
            var gameScreen = GameUI.instance.LoadGameScreen();
            gameScreen.OpenScreen(() =>
            {
                GameloopManager.instance.SetShootType(shootType);
                GameloopManager.instance.StartGameLoop();
            });
        });
    }



    private void LoadGameFromMainMenu()
    {
        void startGameScene()
        {
            var selectScreenUI = GameUI.instance.LoadSelectScreen();
            selectScreenUI.OpenScreen((shootType) =>
            {
                var gameScreen = GameUI.instance.LoadGameScreen();
                gameScreen.OpenScreen(() =>
                {
                    GameloopManager.instance.SetShootType(shootType);
                    GameloopManager.instance.StartGameLoop();
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

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
        Turret turret = FindObjectOfType<Turret>();
        if (turret != null)
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
        else
        {
            LoadStartGame();
        }
#else
        LoadStartGame();
#endif
    }

    private void LoadStartGame()
    {
        var mainMenuActions = GameObject.FindObjectOfType<MainMenuScene.MainMenu>().LoadUpScreen();
        mainMenuActions.OnPlayButtonClicked(() =>
        {
            GameManager.instance.SwitchToScene(GameScenes.Game, () =>
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
            });
        });
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

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

    private void Start()
    {
    }

    public void StartGame()
    {

    }

    public void WinGame()
    {

    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoseGame()
    {

    }

    public void ExitGame()
    {
        Application.Quit();
    }


    public void SwitchToScene(GameScenes scene)
    {
        switch (scene)
        {
            case GameScenes.MainMenu:
                SceneManager.LoadScene("MainMenu");
                break;
            case GameScenes.Game:
                SceneManager.LoadScene("MainScene");
                break;
            default:
                break;
        }
    }
}

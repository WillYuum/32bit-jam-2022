using UnityEngine;
using System;

public enum RotationDirection
{
    ClockWise,
    AntiClockWise,
}

public enum TurretMoveDirection
{
    None,
    ClockWise,
    AntiClockWise,
}

public class PlayerActionsController : MonoBehaviour
{
    private PlayerActionStates _currentPlayerActionsInStage;


    private void Start()
    {
        // GameloopManager.instance.OnGameLoopStart += () =>
        // {
        //     Move();
        // };
    }

    public void Init()
    {
        print("Init turret actions");
        _currentPlayerActionsInStage = new MainMenuStage();
    }




    private void Update()
    {
        _currentPlayerActionsInStage.Update();
    }



    public void SwitchToActions(PlayerStates newPlyerState)
    {
        PlayerActionStates newActions = newPlyerState switch
        {
            PlayerStates.MainMenu => new MainMenuStage(),
            PlayerStates.PickShootType => new SelectShootTypeState(GetTurret()),
            PlayerStates.Game => new GameState(GetTurret()),
            PlayerStates.Lose => new MainMenuStage(),
            _ => throw new NotImplementedException(),
        };

        _currentPlayerActionsInStage?.OnExit();
        _currentPlayerActionsInStage = newActions;
        _currentPlayerActionsInStage.OnEnter();
    }


    private Turret GetTurret()
    {
        return GameObject.FindObjectOfType<Turret>();
    }

}


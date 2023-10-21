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



    public void SwitchToActions(GameFlowState gameFlowState)
    {
        PlayerActionStates newActions = gameFlowState switch
        {
            GameFlowState.MainMenu => new MainMenuStage(),
            GameFlowState.PickShootType => new SelectShootTypeState(GetTurret()),
            GameFlowState.Game => new GameState(GetTurret()),
            GameFlowState.Lose => new MainMenuStage(),
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


using UnityEngine;

public abstract class PlayerActionStates
{
    public abstract void Update();
    public abstract void OnEnter();
    public abstract void OnExit();
}


public class MainMenuStage : PlayerActionStates
{
    public override void OnEnter()
    {

    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }
}




public class SelectShootTypeState : PlayerActionStates
{
    private FishMoveLogic _turretMoveLogic;


    public SelectShootTypeState(Turret turret)
    {
        _turretMoveLogic = new(turret, turret.GetComponent<ITurretActions>());
    }

    public override void OnEnter()
    {
        _turretMoveLogic.TurretPlatfromTracker.SetToStartingPosition();
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        _turretMoveLogic.Update();
    }
}

public class GameState : PlayerActionStates
{
    private FishShootLogic _turrestShootLogic;
    private FishMoveLogic _turretMoveLogic;
    private ITurretActions _turretActions;
    private TurretEvents _turretEvents;

    public GameState(Turret turret)
    {
        _turretActions = turret.GetComponent<ITurretActions>();
        _turrestShootLogic = new FishShootLogic(_turretActions);
        _turretMoveLogic = new(turret, _turretActions);
        _turretEvents = turret.GetTurretEvents();
    }

    public override void OnEnter()
    {
        //Set tracker to turret position since the tracker is being initialized after select shoot type
        Vector3 turretPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        _turretMoveLogic.TurretPlatfromTracker.SetToPosition(turretPosition);

        _turretEvents.OnTurretShoot += ShootProjectile;
    }

    public override void OnExit()
    {
        _turretEvents.OnTurretShoot -= ShootProjectile;
    }

    public override void Update()
    {
        _turrestShootLogic.Update();
        _turretMoveLogic.Update();
    }

    private void ShootProjectile()
    {
        _turretActions.ShootProjectile();
        _turrestShootLogic.ShootController.SetWaitTillShootAnimation(false);
    }
}





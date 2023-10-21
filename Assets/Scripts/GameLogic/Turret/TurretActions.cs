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
    private TurretMoveLogic _turretMoveLogic;


    public SelectShootTypeState(Turret turret)
    {
        _turretMoveLogic = new(turret, turret.GetComponent<ITurretActions>());
    }

    public override void OnEnter()
    {
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
    private TurretShootLogic _turrestShootLogic;
    private TurretMoveLogic _turretMoveLogic;
    private ITurretActions _turretActions;
    private TurretEvents _turretEvents;

    public GameState(Turret turret)
    {
        _turretActions = turret.GetComponent<ITurretActions>();
        _turrestShootLogic = new TurretShootLogic(_turretActions);
        _turretMoveLogic = new(turret, _turretActions);
        _turretEvents = turret.GetTurretEvents();
    }

    public override void OnEnter()
    {
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





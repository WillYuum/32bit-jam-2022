using UnityEngine;

public class Elite : EnemyCore<Elite>
{

    [field: SerializeField] public ShootController ShootController { get; private set; }
    protected override void OnAwake()
    {
        base.OnAwake();
        ShootController = new ShootController(1.0f);
    }


    public void HandleShoot()
    {
        ShootController.ResetShootTimer();
    }



    public class AttackTurret : EnemyStateCore<Elite>
    {
        public override void EnterState(Elite enemy)
        {
            base.EnterState(enemy);
        }

        public override void Act()
        {
            base.Act();
            _owner.ShootController.UpdateShootTimer();
            if (_owner.ShootController.CanShoot)
            {
                _owner.HandleShoot();
            }
        }
    }
}

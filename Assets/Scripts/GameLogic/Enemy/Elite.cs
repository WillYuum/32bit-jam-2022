using UnityEngine;
using SpawnManagerMod;

public class Elite : EnemyCore<Elite>
{

    [field: SerializeField] public ShootController ShootController { get; private set; }
    [HideInInspector] public Transform _target;

    protected override void OnAwake()
    {
        ShootController = new ShootController(1.0f);
        _target = GameObject.Find("Turret").transform;
    }



    public void HandleShoot()
    {
        Vector3 directionToTarget = _target.position - transform.position;

        var projectile = SpawnManager.instance.EliteProjectilePrefab.CreateGameObject(transform.position, transform.rotation);
        projectile.GetComponent<Projectile>().SetShootDirection(directionToTarget.normalized);
        ShootController.ResetShootTimer();
    }
}

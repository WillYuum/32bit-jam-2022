using System;
using UnityEngine;

public class TurretEvents : MonoBehaviour
{
    public event Action OnTurretShoot;

    //WARNING: This function is accessed by the animation event
    //DON'T REMOVE THIS FUNCTION
    private void ShootBullet()
    {
        OnTurretShoot?.Invoke();
    }
}

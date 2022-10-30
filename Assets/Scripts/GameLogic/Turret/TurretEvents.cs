using System;
using UnityEngine;

public class TurretEvents : MonoBehaviour
{
    public event Action OnTurretShoot;
    private void ShootBullet()
    {
        OnTurretShoot?.Invoke();
    }
}

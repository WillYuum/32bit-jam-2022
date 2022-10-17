using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurretMovement
{
    ClickWise,
    AntiClockWise,
}

public class TurretActions : MonoBehaviour, ITurretActions
{

    public void Shoot()
    {

    }


    public void Move(TurretMovement movement)
    {

    }
}


public interface ITurretActions
{
    void Shoot();
    void Move(TurretMovement movement);
}

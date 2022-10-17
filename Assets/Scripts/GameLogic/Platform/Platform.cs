using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private BoxColliderController[] _endPoints;

    private void Awake()
    {
        foreach (var endPoint in _endPoints)
        {
            endPoint._OnTriggerEnter += OnEndPointTriggerEnter;
            endPoint._OnTriggerExit += OnEndPointTriggerExit;
        }
    }

    public void KeepTurruetPerpendicularyAligned(Transform turret)
    {
        Vector3 turretPosition = turret.position;
        Vector3 platformPosition = transform.position;
        Vector3 direction = turretPosition - platformPosition;
        direction.y = 0;
        turret.rotation = Quaternion.LookRotation(direction);
    }

    private void OnEndPointTriggerEnter(Collider2D other)
    {
        //enable switching to another platform
    }

    private void OnEndPointTriggerExit(Collider2D other)
    {
        //disable to switch to another platform
    }

}

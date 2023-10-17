using System;
using UnityEngine;

public class PickShootType : MonoBehaviour
{
    private Action<TypeOfShots> _onSelectType;

    public struct LoadConfig
    {
        public Action<Action<TypeOfShots>> WaitToSelectShootType;
    }


    private void Awake()
    {
        enabled = true; //Don't want to spawn the gameobject and be disabled by mistake
    }

    public LoadConfig Load()
    {
        _onSelectType = null;
        return new LoadConfig
        {
            WaitToSelectShootType = (cb) =>
            {
                _onSelectType += (result) =>
                {
                    cb.Invoke(result);
                    _onSelectType = null;
                };
            }
        };
    }


    private TypeOfShots GetTypeShotFromPosition(Vector2 playerPosition)
    {
        return playerPosition.x > 0 ? TypeOfShots.Laser : TypeOfShots.PeaShots;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            TypeOfShots typeOfShot = GetTypeShotFromPosition(other.transform.position);
            _onSelectType.Invoke(typeOfShot);
            Destroy(gameObject);
        }
    }
}

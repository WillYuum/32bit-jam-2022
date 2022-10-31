using UnityEngine;
using Utils.GenericSingletons;

public class GameVariables : MonoBehaviourSingleton<GameVariables>
{
    [field: SerializeField] public PointSystem PointsData { get; private set; }
    [field: SerializeField] public EnemyHP EnemyHPData { get; private set; }

    [field: SerializeField] public ExplosionBarVariables ExplosionBarData { get; private set; }


    [field: SerializeField] public float PlayerSpeed { get; private set; }
    [field: SerializeField] public float PlayerShootInterval { get; private set; }
    [field: SerializeField] public float BomberExplodeRange { get; private set; }

    [System.Serializable]
    public class EnemyHP
    {
        [field: SerializeField] public int Elite { get; private set; }
        [field: SerializeField] public int Bomber { get; private set; }
        [field: SerializeField] public int Dasher { get; private set; }
    }

    [System.Serializable]
    public class PointSystem
    {
        [field: SerializeField] public int Elite { get; private set; }
        [field: SerializeField] public int Bomber { get; private set; }
        [field: SerializeField] public int Dasher { get; private set; }
    }


    [System.Serializable]
    public class ExplosionBarVariables
    {
        [field: SerializeField] public float MaxExplosionBarValue { get; private set; }
        [field: SerializeField] public float Elite { get; private set; }
        [field: SerializeField] public float Bomber { get; private set; }
        [field: SerializeField] public float Dasher { get; private set; }
    }

}


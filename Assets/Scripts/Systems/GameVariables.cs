using UnityEngine;
using Utils.GenericSingletons;

public class GameVariables : MonoBehaviourSingleton<GameVariables>
{
    [field: SerializeField] public PointSystem PointsData { get; private set; }
    [field: SerializeField] public EnemyHP EnemyHPData { get; private set; }


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
}


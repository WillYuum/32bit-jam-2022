using UnityEngine;
using Utils.GenericSingletons;

public class GameVariables : MonoBehaviourSingleton<GameVariables>
{
    [field: SerializeField] public PointSystem PointsData { get; private set; }
    [field: SerializeField] public EnemyHP EnemyHPData { get; private set; }

    [field: SerializeField] public ExplosionBarVariables ExplosionBarData { get; private set; }

    [field: SerializeField] public float InvinsibilityWindowDuration { get; private set; }

    [field: SerializeField] public TurretPeaShotMomentunUnlock TurretPeaShotUnlockData { get; private set; }
    [field: SerializeField] public TurretLaserShotMomentunUnlock TurretLaserShotUnlockData { get; private set; }

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



    interface IShotUnlockData
    {
        int GetLevelRelativeToMomentum(float momentumRatio);
    }

    [System.Serializable]
    public class TurretPeaShotMomentunUnlock : IShotUnlockData
    {
        [field: SerializeField, Range(0.0f, 1.0f)] public float SinglePeaShot { get; private set; }
        [field: SerializeField, Range(0.0f, 1.0f)] public float DoublePeaShot { get; private set; }
        [field: SerializeField, Range(0.0f, 1.0f)] public float TriplePeaShot { get; private set; }

        public int GetLevelRelativeToMomentum(float momentumRatio)
        {
            if (momentumRatio < SinglePeaShot)
            {
                return 0;
            }
            else if (momentumRatio < DoublePeaShot)
            {
                return 1;
            }
            else if (momentumRatio < TriplePeaShot)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }
    }

    [System.Serializable]
    public class TurretLaserShotMomentunUnlock : IShotUnlockData
    {
        [field: SerializeField, Range(0.0f, 1.0f)] public float thinLaserShot { get; private set; }
        [field: SerializeField, Range(0.0f, 1.0f)] public float thickLaserShot { get; private set; }

        public int GetLevelRelativeToMomentum(float momentumRatio)
        {
            if (momentumRatio < thinLaserShot)
            {
                return 0;
            }
            else if (momentumRatio < thickLaserShot)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }

}


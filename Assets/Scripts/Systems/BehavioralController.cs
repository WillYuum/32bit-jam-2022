using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.GenericSingletons;

public class BehavioralController : MonoBehaviourSingleton<BehavioralController>
{
    private List<BehavioralDataWithTimer> _behavioralsWithTimers = new List<BehavioralDataWithTimer>();
    private List<BehavioralData> _behaviorals = new List<BehavioralData>();


    public void AddBehavioral(BehavioralDataWithTimer behavioral)
    {
        _behavioralsWithTimers.Add(behavioral);
    }

    public void AddBehavioral(BehavioralData behavioral)
    {
        _behaviorals.Add(behavioral);
    }

    public void RemoveBehavioral(BehavioralDataWithTimer behavioral)
    {
        _behavioralsWithTimers.Remove(behavioral);
    }

    public void RemoveBehavioral(BehavioralData behavioral)
    {
        _behaviorals.Remove(behavioral);
    }

    private void LateUpdate()
    {
        foreach (BehavioralDataWithTimer behavior in _behavioralsWithTimers)
        {
            behavior.DurationOfBehavior -= Time.deltaTime;
            behavior.UpdateBehavior.Invoke();


            if (behavior.DurationOfBehavior <= 0)
            {
                _behavioralsWithTimers.Remove(behavior);
                behavior.OnBehaviorEnd.Invoke();
            }
        }

        foreach (BehavioralData behavior in _behaviorals)
        {
            behavior.UpdateBehavior.Invoke();
        }
    }
}

public class BehavioralDataWithTimer
{
    public float DurationOfBehavior;
    public Action UpdateBehavior;
    public Action OnBehaviorEnd;
}

public class BehavioralData
{
    public Action UpdateBehavior;
    public Action OnBehaviorEnd;
}

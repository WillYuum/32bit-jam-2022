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

    private void Update()
    {
        foreach (BehavioralDataWithTimer behaviorWithTimer in _behavioralsWithTimers)
        {
            behaviorWithTimer.DurationOfBehavior -= Time.deltaTime;
            behaviorWithTimer.UpdateBehavior.Invoke();


            if (behaviorWithTimer.DurationOfBehavior <= 0)
            {
                behaviorWithTimer.OnBehaviorEnd.Invoke();
                _behavioralsWithTimers.Remove(behaviorWithTimer);
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

using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.GenericSingletons;

public class BehavioralController : MonoBehaviourSingleton<BehavioralController>
{
    private List<BehavioralDataWithTimer> _behavioralsWithTimers = new List<BehavioralDataWithTimer>();
    private List<BehavioralData> _behaviorals = new List<BehavioralData>();


    public static Action NULL_BEHAVIOR = () => { };


    void Update()
    {
        for (int i = 0; i < _behavioralsWithTimers.Count; i++)
        {
            BehavioralDataWithTimer behaviorWithTimer = _behavioralsWithTimers[i];
            behaviorWithTimer.DurationOfBehavior -= Time.deltaTime;
            behaviorWithTimer.UpdateBehavior.Invoke();


            if (behaviorWithTimer.DurationOfBehavior <= 0)
            {
                if (behaviorWithTimer != null)
                {
                    behaviorWithTimer.OnBehaviorEnd.Invoke();
                    _behavioralsWithTimers.Remove(behaviorWithTimer);
                }
            }
        }

        for (int i = 0; i < _behaviorals.Count; i++)
        {
            BehavioralData behavior = _behaviorals[i];
            if (behavior != null)
            {
                behavior.UpdateBehavior.Invoke();
            }
            else
            {
                _behaviorals.Remove(behavior);
            }
        }
    }

    public void AddBehavioral(BehavioralDataWithTimer behavioral)
    {
        _behavioralsWithTimers.Add(behavioral);
    }

    public void AddBehavioral(BehavioralData behavioral)
    {
        _behaviorals.Add(behavioral);
    }

    public void RemoveBehavioral(BehavioralDataWithTimer behavioral, bool invokeOnBehaviorEnd = false)
    {
        if (invokeOnBehaviorEnd) behavioral.OnBehaviorEnd.Invoke();

        _behavioralsWithTimers.Remove(behavioral);
    }

    public void RemoveBehavioral(BehavioralData behavioral, bool invokeOnBehaviorEnd = false)
    {
        if (invokeOnBehaviorEnd) behavioral.OnBehaviorEnd.Invoke();

        _behaviorals.Remove(behavioral);
    }

    public void Reset()
    {

        if (_behavioralsWithTimers.Count > 0)
        {
            for (int i = 0; i < _behavioralsWithTimers.Count; i++)
            {
                BehavioralDataWithTimer behavior = _behavioralsWithTimers[i];
                RemoveBehavioral(behavior);
            }
        }

        if (_behaviorals.Count > 0)
        {
            for (int i = 0; i < _behaviorals.Count; i++)
            {
                BehavioralData behavior = _behaviorals[i];
                RemoveBehavioral(behavior);
            }
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

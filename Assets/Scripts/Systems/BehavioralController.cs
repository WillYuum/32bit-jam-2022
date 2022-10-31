using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.GenericSingletons;

public class BehavioralController : MonoBehaviourSingleton<BehavioralController>
{
    private List<BehavioralData> _behaviorals = new List<BehavioralData>();

    public void AddBehavioral(BehavioralData behavioral)
    {
        _behaviorals.Add(behavioral);
    }

    public void RemoveBehavioral(BehavioralData behavioral)
    {
        _behaviorals.Remove(behavioral);
    }

    private void LateUpdate()
    {
        for (int i = 0; i < _behaviorals.Count; i++)
        {
            var behavior = _behaviorals[i];
            behavior.DurationOfBehavior -= Time.deltaTime;
            behavior.UpdateBehavior.Invoke();


            if (behavior.DurationOfBehavior <= 0)
            {
                _behaviorals.Remove(behavior);
                behavior.OnBehaviorEnd.Invoke();
            }
        }
        if (_behaviorals.Count > 0)
        {
            print("_behaviorals.Count" + _behaviorals.Count);
            // _behaviorals.Clear();
            print("behavior.DurationOfBehavior" + _behaviorals[0].DurationOfBehavior);
        }
    }
}

public class BehavioralData
{
    public float DurationOfBehavior;
    public Action UpdateBehavior;
    public Action OnBehaviorEnd;

}

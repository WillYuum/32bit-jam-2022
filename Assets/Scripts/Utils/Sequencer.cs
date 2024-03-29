using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : MonoBehaviour
{
    List<SequenceAction> _allSequences = new List<SequenceAction>();
    private bool _isRunning = false;
    private int _currentActionIndex = 0;

    public int Count
    {
        get
        {
            return _allSequences.Count;
        }
    }

    class SequenceAction
    {
        private Func<SequenceState> _action;
        private SequenceState _sequenceState;
        public float DelayToStart { get; private set; }


        public bool IsDone
        {
            get
            {
                return _sequenceState.IsDone;
            }
        }

        public void DecreaseDelayToStart()
        {
            DelayToStart -= Time.deltaTime;
        }

        public SequenceAction(Func<SequenceState> action, float delayToStart = 0.0f)
        {
            DelayToStart = delayToStart;
            _action = action;
        }

        public void InvokeSequenceAction()
        {
            _sequenceState = _action.Invoke();
        }

    }

    public class SequenceState
    {
        public bool IsDone { get; private set; }
        private Action _cb;

        public void FinishSequence()
        {
#if UNITY_EDITOR
            if (IsDone)
            {
                Debug.LogError("Sequence is already done");
                return;
            }
#endif


            if (_cb != null)
            {
                _cb.Invoke();
            }

            IsDone = true;
        }

        public void Handle(Action cb)
        {
            _cb += cb;
        }

        public static SequenceState Finish()
        {
            return new SequenceState()
            {
                IsDone = true
            };
        }
    }


    public static Sequencer CreateSequencer(string name = "Sequencer")
    {
        return new GameObject(name).AddComponent<Sequencer>();
    }

    public static SequenceState CreateSequenceState()
    {
        return new SequenceState();
    }


    private void Update()
    {
        if (_isRunning == false) return;

        SequenceAction currentAction = _allSequences[_currentActionIndex];

        if (currentAction.DelayToStart < 0.0f)
        {
            if (currentAction.IsDone)
            {
                InvokeNext();
            }
        }
        else
        {
            currentAction.DecreaseDelayToStart();
        }
    }

    public void StartSequencer()
    {
#if UNITY_EDITOR
        if (_allSequences.Count == 0)
        {
            Debug.LogError("No actions to run");
            return;
        }

        if (_isRunning)
        {
            Debug.LogError("Sequencer is already running");
            return;
        }
#endif

        _isRunning = true;
        _currentActionIndex = 0;
        _allSequences[_currentActionIndex].InvokeSequenceAction();
    }

    public void AddSequence(Func<SequenceState> action, float delayToStart = 0.0f)
    {
        _allSequences.Add(new SequenceAction(action, delayToStart));
    }


    private void InvokeNext()
    {
        _currentActionIndex++;

        if (_currentActionIndex >= _allSequences.Count)
        {
            _isRunning = false;
            _currentActionIndex = 0;
            Destroy(gameObject);
            return;
        }

        _allSequences[_currentActionIndex].InvokeSequenceAction();
    }
}
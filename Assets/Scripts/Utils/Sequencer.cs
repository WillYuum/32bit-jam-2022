using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : MonoBehaviour
{
    class SequenceAction
    {
        public SequenceAction(Func<SequenceState> action)
        {
            _action = action;
        }

        private Func<SequenceState> _action;
        private SequenceState _sequenceState;

        public bool IsDone
        {
            get
            {
                return _sequenceState.IsDone;
            }
        }
        public void InvokeSequenceAction()
        {
            _sequenceState = _action.Invoke();
        }

    }

    public class SequenceState
    {
        public bool IsDone { get; private set; }

        public void FinishSequence()
        {
            IsDone = true;
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

    List<SequenceAction> _allSequences = new List<SequenceAction>();
    private bool _isRunning = false;

    private int _currentActionIndex = 0;


    private void Update()
    {
        if (_isRunning == false) return;


        if (_allSequences[_currentActionIndex].IsDone)
        {
            InvokeNext();
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

    public void AddSequence(Func<SequenceState> action)
    {
        _allSequences.Add(new SequenceAction(action));
    }


    private void InvokeNext()
    {
        _currentActionIndex++;

        print("Current index: " + _currentActionIndex + " All sequences count: " + _allSequences.Count + " Is running: " + _isRunning);
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
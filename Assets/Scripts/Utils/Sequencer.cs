using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Sequencer
{
    private List<Func<Task>> sequenceActions = new List<Func<Task>>();
    private int currentIndex = 0;
    private bool isRunning = false;
    private TaskCompletionSource<bool> sequenceCompletionSource;

    public void AddSequence(Func<Task> action)
    {
        sequenceActions.Add(action);
    }

    public async Task StartSequence()
    {
#if UNITY_EDITOR
        if (isRunning)
        {
            Debug.LogError("Sequence is already running");
        }

        if (sequenceActions.Count == 0)
        {
            Debug.LogError("Sequence is empty");
        }

#endif


        if (!isRunning && sequenceActions.Count > 0)
        {
            currentIndex = 0;
            isRunning = true;
            sequenceCompletionSource = new TaskCompletionSource<bool>();
            await InvokeNext();
            await sequenceCompletionSource.Task;
        }
    }

    public void StopSequence()
    {
        currentIndex = 0;
        isRunning = false;
        if (sequenceCompletionSource != null)
        {
            sequenceCompletionSource.TrySetCanceled();
        }
    }

    public async Task InvokeNext()
    {
        if (currentIndex < sequenceActions.Count)
        {
            Func<Task> nextAction = sequenceActions[currentIndex];
            await nextAction.Invoke();
            currentIndex++;
            await InvokeNext();
        }
        else
        {
            sequenceCompletionSource.TrySetResult(true);
        }
    }

    public Task GetCurrentRunningTask()
    {
        return sequenceActions[currentIndex].Target as Task;
    }
}

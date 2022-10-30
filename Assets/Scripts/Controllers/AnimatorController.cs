using System;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private RuntimeAnimatorController _controller;

    private void Awake()
    {
        _controller = _animator.runtimeAnimatorController;
    }


    public void SetEventOnAnimation(string animationName, UnityEngine.Object component, string functionName, float timeOnInvokefunction)
    {
        for (int i = 0; i < _controller.animationClips.Length; i++)                 //For all animations
        {
            if (_controller.animationClips[i].name == animationName)        //If it has the same name as your clip
            {
                print("Set event on animation: " + animationName);
                _controller.animationClips[i].AddEvent(new AnimationEvent()
                {
                    functionName = functionName,
                    time = timeOnInvokefunction,
                    objectReferenceParameter = component,
                });

                break;
            }
        }

    }


    public float GetAnimDuration(string stateAnimName)
    {
        float delay = 0;
        for (int i = 0; i < _controller.animationClips.Length; i++)                 //For all animations
        {
            if (_controller.animationClips[i].name == stateAnimName)        //If it has the same name as your clip
            {
                delay = _controller.animationClips[i].length;
            }
        }

        return delay;
    }
}

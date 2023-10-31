using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum KeyEventType
{
    OnKeyDown,
    OnKeyHold,
    OnKeyUp,
}


public class InputConnector
{
    private List<Action> _createInputActions = new();

    public static InputConnector Create()
    {
        return new InputConnector();
    }

    public InputActionsHandler Build()
    {
        OrganizeActions();
        return new InputActionsHandler(_createInputActions);
    }


    public class InputActionsHandler
    {
        private List<Action> _inputActions = new();
        public InputActionsHandler(List<Action> inputActions)
        {
            _inputActions = inputActions;
        }


        public void CheckForInput()
        {
            foreach (var action in _inputActions)
            {
                action.Invoke();
            }
        }
    }

    public InputConnector MapInput(KeyCode keyCode, Action action, KeyEventType conditionType)
    {
        Func<KeyCode, bool> inputCheck = conditionType switch
        {
            KeyEventType.OnKeyDown => Input.GetKeyDown,
            KeyEventType.OnKeyUp => Input.GetKeyUp,
            KeyEventType.OnKeyHold => Input.GetKey,
            _ => throw new ArgumentOutOfRangeException(nameof(conditionType), conditionType, null)
        };

        void inputAction() { if (inputCheck(keyCode)) action.Invoke(); }
        _createInputActions.Add(inputAction);
        return this;
    }
    private void OrganizeActions()
    {
        _createInputActions = _createInputActions.Where(action => action != null).ToList();
    }
}
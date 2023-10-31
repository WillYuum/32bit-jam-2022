using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum KeyEventType
{
    OnKeyDown = 0,
    OnKeyHold = 1,
    OnKeyUp = 2,
}


public class InputConnector
{
    private List<InputEvent> _createdInputActions = new();

    public static InputConnector Create()
    {
        return new InputConnector();
    }

    public InputActionsHandler Build()
    {
        _createdInputActions.Sort((x, y) => x.Order.CompareTo(y.Order));
        var inputActions = _createdInputActions.Select(inputEvent => inputEvent.Action).ToList();
        return new InputActionsHandler(inputActions);
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

        _createdInputActions.Add(new InputEvent
        {
            KeyCode = keyCode,
            Action = inputAction,
            EventType = conditionType,
            Order = (int)conditionType
        });

        return this;
    }
}


public class InputEvent
{
    public KeyCode KeyCode { get; set; }
    public Action Action { get; set; }
    public KeyEventType EventType { get; set; }
    public int Order { get; set; }
}
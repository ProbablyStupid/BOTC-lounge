using System;
using UnityEngine;

public class ControlAction : MonoBehaviour
{
    [SerializeField] private Action callback;
    public void Invoke()
    {
        callback.Invoke();
    }
}

using System;
using UnityEngine;

public class Controlable : Interactible
{
    Action listener;

    public override void Interact(PlayerInteraction interacter)
    {
        listener?.Invoke();
    }

    public void RegisterListener(Action Listener)
    {
        // stupidity of capitalization. This will cause confusion later!
        listener = Listener;
    }

    public void ClearListener()
    {
        listener = null;
    }

    public override void Destroy()
    {
        Debug.Log("Cannot destroy controllables!");
        // do nothing
    }
}

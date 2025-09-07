using System;
using UnityEngine;

public class Controlable : Interactible
{
    Action listener;

    PlayerInteraction latestInteracter;

    public override void Interact(PlayerInteraction interacter)
    {
        latestInteracter = interacter;
        listener?.Invoke();
    }

    public PlayerInteraction GetInteractor()
    {
        return latestInteracter;
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

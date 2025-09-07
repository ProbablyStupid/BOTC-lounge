using UnityEngine;
using Unity.Netcode;

public abstract class Interactible : NetworkBehaviour 
{
    [SerializeField] protected NetworkVariable<bool> isHeld = new NetworkVariable<bool>();

    [SerializeField] protected PlayerInteraction currentHolder;
    
    public bool IsHeld()
    {
        return isHeld.Value;
    }

    //public void Hold(GameObject newHolder)
    //{
    //    Debug.Log("Trying to hold object by " + newHolder);

    //    // perhaps check if the transaction is eligible?

    //    currentHolder = newHolder;

    //    // TODO: implement anti-gravity style physics holding!
    //    transform.parent = newHolder.transform;
    //}


    /// <summary>
    /// For a pickupable object, this will toggle the picking up.
    /// For a control object, this will trigger the interaction.
    /// </summary>
    /// <param name="interacter"></param>
    public abstract void Interact(PlayerInteraction interacter);

    public abstract void Destroy();

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
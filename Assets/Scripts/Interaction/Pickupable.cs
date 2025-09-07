using UnityEngine;

public class Pickupable : Interactible
{
    public override void Interact(PlayerInteraction interacter)
    {
        // this way around because we don't know whether we have a current holder.
        if (interacter.Equals(currentHolder))
        {
            Debug.Log("Getting dropped by interacter " + interacter);

            currentHolder = null;
            isHeld.Value = false;

            interacter.UnregisterHeldObject(this);

            // we do this because a maximum of 1 action can happend for
            // 1 key press
            return;
        }

        if (currentHolder != null)
        {
            Debug.Log("Getting stolen by interacter " + interacter);

            currentHolder.UnregisterHeldObject(this);
            interacter.RegisterHeldObject(this);

            currentHolder = interacter;

            return;
        }

        if (currentHolder == null)
        {
            Debug.Log("Getting picked up by interacter " + interacter);
            interacter.RegisterHeldObject(this);

            currentHolder = interacter;

            return;
        }

        Debug.Log("Pickupable-Interaction failed!");
    }

    public override void Destroy()
    {
        Debug.Log("Destroying self " + this.ToString());
        Destroy(gameObject);
    }
}

using UnityEngine;
using Unity.Netcode;

public class PlayerInteraction : NetworkBehaviour 
{
    // TODO:
    // This is tricky because what if somebody else tries to grab the object you're currently holding?
    // Does the Object being held know whether it is being held and who picked it up? Does "ownership" transfer?
    [SerializeField] bool holding = false;

    public void AttempInteraction(GameObject hit)
    {
        // -> get the tag of the game object to determine whether it is a player, an object, or an interactible
        switch (hit.tag)
        {
            case "Player":
                // @implement something to do with player interactions
                //              - should the storyteller be able to do something to the players?
                break;
        }
    }
}

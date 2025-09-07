using UnityEngine;
using Unity.Netcode;

public class PlayerInteraction : NetworkBehaviour 
{
    [SerializeField] Camera raycastingCamera;

    // TODO:
    // This is tricky because what if somebody else tries to grab the object you're currently holding?
    // Does the Object being held know whether it is being held and who picked it up? Does "ownership" transfer?
    [SerializeField] bool holding = false;

    [SerializeField] public float pickup_distance = 20;

    [SerializeField] Interactible heldObject;
    [SerializeField] Rigidbody heldRigidbody;

    [SerializeField] Transform holdingArea;

    [SerializeField] float objectSpeed = 20;

    public void AttemptInteraction()
    {
        //Ray myRay = raycastingCamera.ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(raycastingCamera.transform.position, raycastingCamera.transform.forward, out RaycastHit rayHit, pickup_distance))
        {
            var hit = rayHit.transform.gameObject;
            Debug.Log("PlayerInteraction -> hit an object : " + hit);

            //Interactible interactible;
            //interactible = hitObject.GetComponent<Interactible>();
            //if (interactible == null) 
            //    return;

            //interactible.Hold(transform.gameObject);

            // -> get the tag of the game object to determine whether it is a player, an object, or an interactible
            switch (hit.tag)
            {
                case "Player":
                    Debug.Log("PlayerInteraction -> hit a player");

                    // @implement something to do with player interactions
                    //              - should the storyteller be able to do something to the players?
                    break;
                
                case "Interact":
                    Debug.Log("PlayerInteraction -> hit an interactible");
                    
                    Interactible interactible = hit.GetComponent<Interactible>();
                    interactible.Interact(this);
                    
                    break;
                
                default:
                    Debug.Log("PlayerInteraction -> not interaction possible");
                    break;
            }
        } else
        {
            Debug.Log("Empty Raycast for interaction");
        }
    }

    public void RegisterHeldObject(Interactible newHeldObject)
    {
        holding = true;
        heldObject = newHeldObject;
        heldRigidbody = newHeldObject.GetGameObject().GetComponent<Rigidbody>();
    }

    public void UnregisterHeldObject(Interactible prevHeldObject)
    {
        if (heldObject.Equals(prevHeldObject))
        {
            holding = false;
            heldObject = null;
            heldRigidbody = null;
        }
    }

    public void Update()
    {
        if (heldObject != null)
        {
            HoldingUpdate();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnInteractionKeyPress();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnDestroyKeyPress();
        }

        Debug.DrawRay(raycastingCamera.transform.position, raycastingCamera.transform.forward * pickup_distance);
    }

    // note: this is almost exactly the same algorithm used in Anti-Gravity.
    void HoldingUpdate()
    {
        var gameObject = heldObject.GetGameObject();

        gameObject.transform.rotation = transform.rotation;

        Vector3 directionToHoldPosition = (holdingArea.position - gameObject.transform.position);
        directionToHoldPosition.Normalize();

        Vector3 force = directionToHoldPosition * objectSpeed;

        float distanceToHoldPosition = Vector3.Distance(holdingArea.position, gameObject.transform.position);

        if (distanceToHoldPosition <= 0.1f)
        {
            force /= 10;
        }

        distanceToHoldPosition = Mathf.Clamp(distanceToHoldPosition, 0.0f, 1.0f);
        force *= distanceToHoldPosition;

        heldRigidbody.AddForce(force, ForceMode.VelocityChange);
    }

    //void StopHolding()
    //{
    //    holding = false;
    //    heldObject.Drop(gameObject);
    //}

    public void OnInteractionKeyPress()
    {
        Debug.Log("OnInteractionKeyPress for " + gameObject);

        //if (holding)
        //{
        //    StopHolding();
        //    return;
        //}

        if (holding)
        {
            heldObject.Interact(this);
        } else
        {
            AttemptInteraction();
        }
    }

    public void OnDestroyKeyPress()
    {

        if (Physics.Raycast(raycastingCamera.transform.position, raycastingCamera.transform.forward, out RaycastHit rayHit, pickup_distance))
        {
            var hit = rayHit.transform.gameObject;
            Debug.Log("PlayerInteraction -> hit an object : " + hit);

            if (hit.tag == "Interact")
            {
                Debug.Log("Destroying game object " + hit);
                //Destroy(hit);
                hit.GetComponent<Interactible>().Destroy();
            }
        }
        else
        {
            Debug.Log("Empty Raycast for interaction");
        }
    }
}

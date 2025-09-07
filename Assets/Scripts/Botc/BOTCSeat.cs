using UnityEngine;

public class BOTCSeat : MonoBehaviour
{
    [SerializeField] BotcPlayer assignedPlayer = null;

    [SerializeField] Controlable thisControlable;

    [SerializeField] bool sitting = false;

    public void Start()
    {
        thisControlable.RegisterListener(OnInteraction);
    }
    public void OnInteraction()
    {
        if (assignedPlayer == null)
        {
            assignedPlayer = thisControlable.GetInteractor().gameObject.GetComponent<BotcPlayer>();
            assignedPlayer.AssignSeat(this);
        }

        sitting = !sitting;
    }
}

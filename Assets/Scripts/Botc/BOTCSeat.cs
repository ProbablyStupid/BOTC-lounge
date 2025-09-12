using TMPro;
using UnityEngine;

public class BOTCSeat : MonoBehaviour
{
    [SerializeField] BotcPlayer assignedPlayer = null;

    [SerializeField] Controlable thisControlable;

    [SerializeField] TMP_Text playerName;

    [SerializeField] bool sitting = false;

    public void Start()
    {
        thisControlable.RegisterListener(OnInteraction);
    }
    public void OnInteraction()
    {
        if (assignedPlayer == null)
        {
            if (thisControlable.GetInteractor().gameObject.GetComponent<BotcPlayer>().HasSeat() == true)
            {
                Debug.Log("Player already has a seat!");
                return;
            }

            assignedPlayer = thisControlable.GetInteractor().gameObject.GetComponent<BotcPlayer>();
            assignedPlayer.AssignSeat(this);

            playerName.text = assignedPlayer.GetPlayerName();
        }

        sitting = !sitting;
    }
}

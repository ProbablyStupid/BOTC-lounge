using UnityEngine;

public class BecomeStoryteller : MonoBehaviour
{
    Controlable myButton;

    [SerializeField] BotcMaster botcMaster;

    public void Start()
    {
        myButton = gameObject.GetComponent<Controlable>();
        myButton.RegisterListener(OnAction);
    }

    public void OnAction()
    {
        BotcPlayer player = myButton.GetInteractor().gameObject.GetComponent<BotcPlayer>();
        // 2 for storyteller. -> TODO: turn this into constants.
        player.SetPlayerType(2);

        Destroy(gameObject);
    }
}

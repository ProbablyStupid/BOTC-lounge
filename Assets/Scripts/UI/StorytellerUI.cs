using UnityEngine;

public class StorytellerUI : MonoBehaviour
{
    [SerializeField] BOTCCommonArea commonArea;

    [SerializeField] GameObject storyTellerUI;

    [SerializeField] private bool enabled = false;

    [SerializeField] BotcPlayer ownedPlayer = null;

    [SerializeField] PlayerMovement playerMovement = null;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnStorytellerUIButtonPressed();
        }
    }

    public void OnStorytellerUIButtonPressed()
    {
        if (ownedPlayer == null)
        {
            Debug.Log("No BOTCPlayer present!");
            return;
        }

        if (ownedPlayer.BOTC_PlayerType.Value != 2)
        {
            Debug.Log("BOTCPlayer is not storyteller!");
            return;
        }

        enabled = !enabled;
        storyTellerUI.SetActive(enabled);

        if (enabled)
        {
            playerMovement.Unlock();
        } else 
        {
            playerMovement.Lock();
        }
    }

    public void AssignPlayer(BotcPlayer player)
    {
        ownedPlayer = player;
        playerMovement = player.gameObject.GetComponent<PlayerMovement>();
    }
}

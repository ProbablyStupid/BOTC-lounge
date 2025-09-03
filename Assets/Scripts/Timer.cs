using TMPro;
using UnityEngine;


/// <summary>
/// The text itself should not be a network object.
/// It should instead receive orders from the BOTC Master or Storyteller for the timer, display that timer locally and always point in the direction of the player.
/// </summary>
public class Timer : MonoBehaviour
{
    // assigned by inspector
    [SerializeField] TMP_Text text;

    // assigned by the player
    [SerializeField] Transform playerTransform = null;

    public void Update()
    {
        if (playerTransform == null)
            return;

        // if I remember vectors correctly, this will get us the direction of the player
        text.transform.forward = text.transform.position - playerTransform.position;
    }

    public void RegisterPlayer(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
    }
}

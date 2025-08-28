using Unity.Netcode;
using UnityEngine;

public class NetworkCamera : NetworkBehaviour
{
    [SerializeField] Camera thisCamera;
    private void Start()
    {
        if (!IsOwner) return;

        // this only gets executed if we own this game object!

        thisCamera.gameObject.SetActive(true);
    }
}

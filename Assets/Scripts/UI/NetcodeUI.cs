using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class NetcodeUI : MonoBehaviour
{

    [SerializeField] Button hostButton;
    [SerializeField] Button clientButton;
    [SerializeField] TMP_InputField ipInput;
    [SerializeField] UnityTransport unityTransport;
    [SerializeField] Camera uiCamera;

    void Start()
    {
        hostButton.onClick.AddListener(() => {
            print("Starting host!");
            NetworkManager.Singleton.StartHost();

            uiCamera.enabled = false;
        });
        clientButton.onClick.AddListener(() =>
        {
            print("starting client");

            // first set the IP address from the ipInput
            print("Using " + ipInput.text + " as the IP address from the ip Input Field");
            unityTransport.ConnectionData.Address = ipInput.text;

            NetworkManager.Singleton.StartClient();

            uiCamera.enabled = false;
        });
    }
}

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
    [SerializeField] Camera startCamera;
    [SerializeField] TMP_InputField nameInput;

    // TODO: implement player name transmission here somehow...

    [SerializeField] MasterUI masterUI;

    void Start()
    {
        //masterUI = GetComponent<MasterUI>();
        print("Adding listeners for NetcodeUI!");
        hostButton.onClick.AddListener(() => {
            if (nameInput.text.Equals(""))
                return;

            print("Starting host!");

            var nameBytes = System.Text.Encoding.UTF8.GetBytes(nameInput.text);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = nameBytes;

            NetworkManager.Singleton.StartHost();

            startCamera.gameObject.SetActive(false);
            clientButton.gameObject.SetActive(false);
            ipInput.gameObject.SetActive(false);
            hostButton.gameObject.SetActive(false);
            nameInput.gameObject.SetActive(false);

            print("Disabled UI components!");

            masterUI.switchToGameState();
        });
        clientButton.onClick.AddListener(() =>
        {
            if (nameInput.text.Equals(""))
                return;

            print("starting client");

            var nameBytes = System.Text.Encoding.UTF8.GetBytes(nameInput.text);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = nameBytes;

            // first set the IP address from the ipInput
            print("Using " + ipInput.text + " as the IP address from the ip Input Field");
            unityTransport.ConnectionData.Address = ipInput.text;

            NetworkManager.Singleton.StartClient();

            startCamera.gameObject.SetActive(false);
            clientButton.gameObject.SetActive(false);
            ipInput.gameObject.SetActive(false);
            hostButton.gameObject.SetActive(false);
            nameInput.gameObject.SetActive(false);

            masterUI.switchToGameState();
        });
    }
}

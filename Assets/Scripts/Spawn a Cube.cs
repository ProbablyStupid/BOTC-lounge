using UnityEngine;

public class SpawnaCube : MonoBehaviour
{
    [SerializeField] Controlable button;
    [SerializeField] GameObject cubeToSpawn;

    public void Start()
    {
        button.RegisterListener(OnButtonPush);
    }

    public void OnButtonPush()
    {
        // TODO: check if this works over the network!

        // also, this is stupid because it puts the new cube where the old cube was initially,
        // but this is just for testing, so who cares??
        Instantiate(cubeToSpawn);
    }
}

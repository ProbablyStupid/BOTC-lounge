using UnityEngine;
using Unity.Netcode;

public class BOTCCommonArea : NetworkBehaviour 
{
    [SerializeField] Transform seatsParent;
    [SerializeField] Transform center;

    [SerializeField] GameObject chairPrefab;

    [SerializeField] Transform[] chairs;

    [SerializeField] float radius = 10;

    [SerializeField] GameObject chairsPrefab;

    public void Start()
    {
        // something?
    }

    public void Regenerate(int players)
    {
        //CircleGenerate(players);
        PrefabGeneration();
    }
    
    void PrefabGeneration()
    {
        // Do not make a Typo here! It needs to be "chairsPrefab" not "chairPrefab" (Plural v. Singular)!!
        GameObject chairsObject = Instantiate(chairsPrefab);
        chairsObject.GetComponent<NetworkObject>().Spawn();
        //chairsObject.transform.parent = seatsParent;
        chairsObject.transform.parent = transform;
        chairsObject.transform.position = center.position;

        chairs = chairsObject.GetComponentsInChildren<Transform>();
    }

    void CircleGenerate(int players)
    {
        chairs = new Transform[players];

        // TODO: generate seats that are equally distant from one another in a circular fashion around the center
        Vector3 startingChair = seatsParent.transform.position + new Vector3(radius, 0, 0);

        GameObject firstChair = Instantiate(chairPrefab);
        firstChair.transform.position = startingChair;
        firstChair.transform.forward = (firstChair.transform.position - center.position);
        chairs[0] = firstChair.transform;

        Vector3 offset = new Vector3(radius * (1-(1/players)), radius * (1-(1/players)), 0);

        for (int i = 1; i < players; i++)
        {

        }
    }
}

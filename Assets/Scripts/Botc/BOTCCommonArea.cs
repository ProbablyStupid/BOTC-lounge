using UnityEngine;

public class BOTCCommonArea : MonoBehaviour
{
    [SerializeField] Transform seatsParent;
    [SerializeField] Transform center;

    [SerializeField] GameObject chairPrefab;

    public void Start()
    {
        // something?
    }

    public void Regenerate(int players)
    {
        // TODO: generate seats that are equally distant from one another in a circular fashion around the center
    
        // note: the seating order needs to be decided somehow
    }
}

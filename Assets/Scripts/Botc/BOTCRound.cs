using UnityEngine;
using Unity.Netcode;
using System;

public class BOTCRound : NetworkBehaviour, IEquatable<BOTCRound>
{
    [SerializeField] private long id = 0;

    [SerializeField] BotcPlayer[] players;

    /// <summary>
    /// round works like this:
    /// 
    /// 0 -> starting position; storyteller assigns roles
    /// 1 -> initial night
    /// 
    /// even numbers are days
    /// odd numbers are nights
    /// 
    /// example:
    /// 
    /// 2 -> first day
    /// 3 -> first night (between day 2 and day 4)
    /// 
    /// </summary>
    [SerializeField] int round = 0;

    public void Awake()
    {
        // if this is the case then we already have an id, for whatever reason
        if (id != 0)
            return;

        System.Random random = new System.Random();
        this.id = random.Next(1, 20000000);
    }

    /// <summary>
    /// Has to be called before Initialize!!!
    /// </summary>
    /// <param name="players"></param>
    public void SetPlayers(BotcPlayer[] players)
    {
        this.players = players;
    }

    public void Initialize()
    {
        foreach (var player in players)
        {
            player.ParentRound = this;
        }
    }

    /// <summary>
    /// Switches from day to night to day
    /// </summary>
    public void Advance()
    {
        round += 1;
    }

    public bool IsNight()
    {
        return round %2 != 0;
    }

    public bool Equals(BOTCRound other)
    {
        // Stupid C#. Why am I allowed to access a private member of a different instance???
        return (other.id == this.id);
    }
}

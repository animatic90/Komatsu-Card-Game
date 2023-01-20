using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    //This simple GameManager script is attached to a Server-only game object, demonstrating how to implement game logic tracked by the Server
    public int CardsPlayed = 0;
    public void UpdateCardsPlayed()
    {
        CardsPlayed++;
    }
}


using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    //This simple GameManager script is attached to a Server-only game object, demonstrating how to implement game logic tracked by the Server
    public int cardsDealed = 0;
    public int cardsPlayed = 0;
    //public int conectedPlayers = -1; //-1 si hay server dedicado , 0 si es client y host
    public int conectedPlayers; //ahora lo traemos desde el CustomNetworkManager
    public List<uint> playersId = new List<uint>();
    
    public void UpdateConectedPlayers(int numPlayers)
    {
        //conectedPlayers++;
        conectedPlayers = numPlayers;
    }

    public void UpdateIdPlayers(uint id)
    {
         playersId.Add(id); // TODO: falta revisar la desconexion de usuarios
    }

    public void UpdateCardsDealed()
    {
        cardsDealed++;
    }

    public void UpdateCardsPlayed()
    {
        cardsPlayed++;
    }

 
}


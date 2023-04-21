using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    //This simple GameManager script is attached to a Server-only game object, demonstrating how to implement game logic tracked by the Server
    public int cardsDealed = 0;
    public int riskCardsDealed = 0;
    public int cardsPlayed = 0;
    public int conectedPlayers; //ahora lo traemos desde el CustomNetworkManager
    public List<uint> playersId = new List<uint>();
    public PlayerManager currentPlayer;


    private static GameManager instance;

    public static GameManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        // Verificar si ya existe una instancia, en cuyo caso destruir este objeto
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Turn Manager
    public List<PlayerManager> players = new List<PlayerManager>();
    public int currentPlayerIndex = 0;

    public void AddPlayer(PlayerManager player)
    {
        players.Add(player);
    }

    public void RemovePlayer(PlayerManager player)
    {
        players.Remove(player);
    }

    //public void SetPlayerTurn(PlayerManager player)
    //{
    //    foreach (PlayerManager p in players)
    //    {
    //        p.isPlayerTurn = (p == player);
    //    }
    //}

    public void SetPlayerTurn(PlayerManager currentPlayer, bool isTurn)
    {
        foreach (PlayerManager player in players)
        {
            player.isPlayerTurn = (player == currentPlayer) ? isTurn : false;
        }
    }

    public void EndTurn(PlayerManager currentPlayer, bool isTurn)
    {

        if (isTurn)
        {
            // El jugador actual aún tenía el turno, así que lo marcamos como finalizado
            currentPlayer.isPlayerTurn = false;
            currentPlayerIndex++;
            if (currentPlayerIndex >= players.Count)
            {
                currentPlayerIndex = 0;
            }

            //currentPlayerIndex++;
            //currentPlayerIndex %= players.Count;

            SetPlayerTurn(players[currentPlayerIndex], isTurn);
        }
    }

    //Aquí solo se llega cuando se va a pasar al sgte jugador
    //Es decir, cuando por lo menos ya hubo un jugador
    public void EndTurn()
    {
        //Bandera para definir el nextPlayer  
        bool NextPlayerTurn = false; 
        //Obtenemos allPlayers
        PlayerManager[] allPlayers = FindObjectsOfType<PlayerManager>();
        Array.Reverse(allPlayers);

        Debug.Log("Antes -> Config de los Players: " + "Primero: " + allPlayers[0].isPlayerTurn + " Segundo: " + allPlayers[1].isPlayerTurn);

        //Obtenemos currentPlayer
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        currentPlayer = networkIdentity.GetComponent<PlayerManager>();
        //Definimos nextPlayer en true y los demás en false

        for (int i = 0; i < allPlayers.Length; i++)
        {
            if (allPlayers[i] == currentPlayer)
            {
                NextPlayerTurn = true;
                allPlayers[i].isPlayerTurn = false; //currentPlayer
            }
            else
            {
                NextPlayerTurn = false;
            }

            if (i == (allPlayers.Length - 1))
            {
                allPlayers[0].isPlayerTurn = NextPlayerTurn;
            }
            else
            {
                allPlayers[i+1].isPlayerTurn = NextPlayerTurn;
            }
        }
        //Mostraremos el log solo para dos jugadores
        Debug.Log("Después -> Config de los Players: "+"Primero: "+ allPlayers[0].isPlayerTurn + " Segundo: " + allPlayers[1].isPlayerTurn);
    
    }

    #endregion



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

    public void UpdateRiskCardsDealed()
    {
        riskCardsDealed++;
    }

    public void UpdateCardsPlayed()
    {
        cardsPlayed++;
    }

 
}


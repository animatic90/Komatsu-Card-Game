using Mirror;
using Mirror.Examples.Basic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : NetworkBehaviour
{
    //Card1 and Card2 are located in the inspector, whereas PlayerArea, EnemyArea, and DropZone are located at runtime within OnStartClient()    
    //public GameObject controlCard;
    public GameObject playerArea;
    public GameObject enemyAreaL;
    public GameObject enemyAreaR;
    public GameObject dropZone;

    public List<GameObject> riskCards = new List<GameObject>();
    //the cards List represents our deck of cards
    public List<GameObject> controlCards = new List<GameObject>();


    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    public string playerName;

    [SyncVar(hook = nameof(OnPlayerTurnChanged))]
    public bool isPlayerTurn;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        playerArea = GameObject.Find("PlayerArea");
        enemyAreaL = GameObject.Find("EnemyAreaL");
        enemyAreaR = GameObject.Find("EnemyAreaR");
        dropZone = GameObject.Find("DropZone");

    }

    //when the server starts, store Card1 and Card2 in the cards deck. Note that server-only methods require the [Server] attribute immediately preceding them!
    [Server]
    public override void OnStartServer()
    {

        // controlCards.Add(controlCard);//todas las cartas    
        // cards.Sort();

        // int playerId = gameObject.GetInstanceID();

        UpdateIdPlayers(netId);

    }

    public override void OnStopServer()
    {
        gameManager.RemovePlayer(this);
    }



    #region Turn Manager
    private void OnPlayerNameChanged(string oldValue, string newValue)
    {
        Debug.Log($"Player name changed from {oldValue} to {newValue}");
    }

    private void OnPlayerTurnChanged(bool oldValue, bool newValue)
    {
        Debug.Log($"{playerName}'s turn changed to {newValue}");
    }

    //public void EndTurn()
    //{
    //    if (isPlayerTurn)
    //    {
    //        gameManager.EndTurn();
    //    }
    //}

    public void EndTurn()
    {
        if (!isLocalPlayer)
            return;

        CmdEndTurn();
    }

    [Command]
    private void CmdEndTurn()
    {
        gameManager.EndTurn(this, isPlayerTurn);
    }
    #endregion





    public void dealRiskCards()
    {
        if (!isPlayerTurn) return;
        CmdDealRiskCards();
    }

    [Command]
    public void CmdDealRiskCards()
    {

        GameManager _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        //  _gm.playersId.ForEach(s => Debug.Log("id de jugador " + s));

        int dealedRiskCardsNumber = _gm.riskCardsDealed;


        if (dealedRiskCardsNumber < riskCards.Count)
        {
            GameObject riskCard = Instantiate(riskCards[dealedRiskCardsNumber], new Vector3(0, 0, 0), Quaternion.identity);
            NetworkServer.Spawn(riskCard, connectionToClient);
            RpcShowRiskCard(riskCard);

            if (isServer)
            {
                UpdateRiskCardsDealed();
            }
        }
        else
        {
            RpcLogToClients("cartas de riesgo agotadas!");
        }



    }
    [ClientRpc]
    void RpcShowRiskCard(GameObject riskCard)
    {
        riskCard.transform.SetParent(dropZone.transform, false);
    }



    [Server]
    void UpdateRiskCardsDealed()
    {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.UpdateRiskCardsDealed();

        RpcLogToClients("Cartas de riesgo repartidas: " + gm.riskCardsDealed);

    }


    public void dealControlCards()
    {

        if (!isPlayerTurn) return;

        CmdDealControlCards(this.netId);
    }

    //Commands are methods requested by Clients to run on the Server, and require the [Command] attribute immediately preceding them. CmdDealCards() is called by the DrawCards script attached to the client Button
    [Command]
    public void CmdDealControlCards(uint _netId)
    {
        //Spawn a random card from the cards deck on the Server, assigning authority over it to the Client that requested the Command. Then run RpcShowCard() and indicate that this card was "Dealt"
        for (int i = 0; i < 5; i++)
        {
            GameManager _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            //  _gm.playersId.ForEach(s => Debug.Log("id de jugador " + s));

            int dealedCardsNumber = _gm.cardsDealed;
            List<uint> playersIdList = _gm.playersId;


            if (dealedCardsNumber < controlCards.Count)//TODO: utilizar este cuando esten todas las cartas
                                                       //if (true)
            {
                GameObject card = Instantiate(controlCards[dealedCardsNumber], new Vector2(0, 0), Quaternion.identity); //TODO: utilizar este cuando esten todas las cartas
                                                                                                                        //GameObject card = Instantiate(controlCards[0], new Vector2(0, 0), Quaternion.identity);
                NetworkServer.Spawn(card, connectionToClient);

                RpcShowCard(card, "Dealt", _netId, playersIdList);



                if (isServer)
                {
                    UpdateCardsDealed();
                }

            }
            else
            {
                // RpcLogToClients("cartas de control agotadas!");
            }



        }
    }

    //PlayCard() is called by the DragDrop script when a card is placed in the DropZone, and requests CmdPlayCard() from the Server
    public void PlayCard(GameObject card)
    {

        CmdPlayCard(card, this.netId);
    }

    //CmdPlayCard() uses the same logic as CmdDealCards() in rendering cards on all Clients, except that it specifies that the card has been "Played" rather than "Dealt"
    [Command]
    void CmdPlayCard(GameObject card, uint _netID)
    {
        GameManager _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        List<uint> playersIdList = _gm.playersId;

        RpcShowCard(card, "Played", _netID, playersIdList);

        //If this is the Server, trigger the UpdateTurnsPlayed() method to demonstrate how to implement game logic on card drop
        if (isServer)
        {
            UpdateCardsPlayed(); //UpdateTurnsPlayed(); 
        }
    }

    [Command]
    public void CmdUpdateIdPlayers(uint _netid)
    {
        UpdateIdPlayers(_netid);
    }

    [Server]
    void UpdateIdPlayers(uint _netid)
    {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.UpdateIdPlayers(_netid);

    }

    //UpdateTurnsPlayed() is run only by the Server, finding the Server-only GameManager game object and incrementing the relevant variable
    [Server]
    void UpdateCardsDealed()
    {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.UpdateCardsDealed();

        RpcLogToClients("Cartas Repartidas: " + gm.cardsDealed);

    }

    [Server]
    void UpdateCardsPlayed()
    {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.UpdateCardsPlayed();

        RpcLogToClients("Cartas Jugadas: " + gm.cardsDealed);

    }

    //RpcLogToClients demonstrates how to request all clients to log a message to their respective consoles
    [ClientRpc]
    void RpcLogToClients(string message)
    {
        Debug.Log(message);
    }

    //ClientRpcs are methods requested by the Server to run on all Clients, and require the [ClientRpc] attribute immediately preceding them
    [ClientRpc]
    void RpcShowCard(GameObject card, string type, uint _cardNetId, List<uint> playersIdList)
    {
        //if the card has been "Dealt," determine whether this Client has authority over it, and send it either to the PlayerArea or EnemyArea, accordingly. For the latter, flip it so the player can't see the front!

        uint _localUid = NetworkClient.localPlayer.netId;

        if (type == "Dealt")
        {

            if (_localUid == _cardNetId)
            {
                // Obtener el componente RectTransform del objeto
                RectTransform rectTransform = playerArea.GetComponent<RectTransform>();
                // Modificar el ancho del objeto
                //rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x + card.GetComponent<RectTransform>().sizeDelta.x, rectTransform.sizeDelta.y);
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x + 185, rectTransform.sizeDelta.y);

                //card.transform.Rotate(0, 0, -90);
                card.transform.localScale = new Vector3((float)1, (float)1, 1);
                card.transform.SetParent(playerArea.transform, false);

            }
            else
            {
                setEnemyCardParent(card, _cardNetId, playersIdList);

            }


        }
        //if the card has been "Played," send it to the DropZone. If this Client doesn't have authority over it, flip it so the player can now see the front!
        else if (type == "Played")
        {
            // card.transform.SetParent(dropZone.transform, false);
            NetworkServer.Destroy(card);
            if (!isOwned)
            {
                //TODO: aqui se podria manjear el puntaje
            }
        }
    }


    void setEnemyCardParent(GameObject card, uint _cardNetId, List<uint> playersIdList)
    {
        //playersIdList.ForEach(s => Debug.Log("iduser" + s));
        //Debug.Log("not Localplayer netid: " + NetworkClient.localPlayer.netId); 
        //Debug.Log("card _netid: " + _cardNetId);        

        uint localUid = NetworkClient.localPlayer.netId;

        int userIndex = playersIdList.IndexOf(localUid);

        uint leftUserId = 0;
        uint rightUserId;

        if (userIndex == 0)
        {
            leftUserId = playersIdList[playersIdList.Count - 1];
            rightUserId = playersIdList[userIndex + 1];
        }
        else if (userIndex == playersIdList.Count - 1)
        {
            leftUserId = playersIdList[userIndex - 1];
            rightUserId = playersIdList[0];
        }
        else
        {
            leftUserId = playersIdList[userIndex - 1];
            rightUserId = playersIdList[userIndex + 1];
        }

        if (_cardNetId == leftUserId)
        {
            // card.transform.Rotate(Vector3.forward, 90f);
            card.transform.SetParent(enemyAreaL.transform, false);
            card.GetComponent<CardFlipper>().Flip();
        }
        if (_cardNetId == rightUserId)
        {
            // card.transform.Rotate(Vector3.back, 90f);
            card.transform.SetParent(enemyAreaR.transform, false);
            card.GetComponent<CardFlipper>().Flip();
        }


    }

    //CmdTargetSelfCard() is called by the TargetClick script if the Client hasAuthority over the gameobject that was clicked
    [Command]
    public void CmdTargetSelfCard()
    {
        TargetSelfCard();
    }

    //CmdTargetOtherCard is called by the TargetClick script if the Client does not hasAuthority (err...haveAuthority?!?) over the gameobject that was clicked
    [Command]
    public void CmdTargetOtherCard(GameObject target)
    {
        NetworkIdentity opponentIdentity = target.GetComponent<NetworkIdentity>();
        TargetOtherCard(opponentIdentity.connectionToClient);
    }

    //TargetRpcs are methods requested by the Server to run on a target Client. If no NetworkConnection is specified as the first parameter, the Server will assume you're targeting the Client that hasAuthority over the gameobject
    [TargetRpc]
    void TargetSelfCard()
    {
        Debug.Log("Targeted by self!");
    }

    [TargetRpc]
    void TargetOtherCard(NetworkConnection target)
    {
        Debug.Log("Targeted by other!");
    }

    //CmdIncrementClick() is called by the IncrementClick script
    [Command]
    public void CmdIncrementClick(GameObject card)
    {
        RpcIncrementClick(card);//clicks on the same card
    }

    //RpcIncrementClick() is called on all clients to increment the NumberOfClicks SyncVar within the IncrementClick script and log it to the debugger to demonstrate that it's working
    [ClientRpc]
    void RpcIncrementClick(GameObject card)
    {
        card.GetComponent<IncrementClick>().NumberOfClicks++;
        // Debug.Log("This card has been clicked " + card.GetComponent<IncrementClick>().NumberOfClicks + " times!");
    }

    public void EndTurn_GameManager()
    {
        CmdEndTurnGameManagerCon(this);
        Debug.Log("PlayerManager - EndTurn_GameManager - Se presionó el botón cambiar");
    }

    [Command]
    public void CmdEndTurnGameManagerCon(PlayerManager currentPlayer)
    {
        Debug.Log("PlayerManager - Se presionó el botón cambiar");

        //GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        //gm.EndTurn();
        //GameManager.Instance.EndTurn();
        Asignarturno(currentPlayer);
    }

    [Server]
    public void Asignarturno(PlayerManager currentPlayer)
    {
        if (currentPlayer.isPlayerTurn == true)
        {
            //Bandera para definir el nextPlayer  
            bool NextPlayerTurn = false;
            //Obtenemos allPlayers
            PlayerManager[] allPlayers = FindObjectsOfType<PlayerManager>();
            Array.Reverse(allPlayers);

            Debug.Log("Antes -> Config de los Players: " + "Primero: " + allPlayers[0].isPlayerTurn + " Segundo: " + allPlayers[1].isPlayerTurn);

            //Obtenemos currentPlayer
            //NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            //currentPlayer = networkIdentity.GetComponent<PlayerManager>();
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
                    allPlayers[i + 1].isPlayerTurn = NextPlayerTurn;
                }
            }
            //Mostraremos el log solo para dos jugadores
            Debug.Log("Después -> Config de los Players: " + "Primero: " + allPlayers[0].isPlayerTurn + " Segundo: " + allPlayers[1].isPlayerTurn);

            EnviarTurnoATodosLosJugadores(allPlayers);
        }

    }

    //[ClientRpc]
    //public void EnviarTurnoATodosLosJugadores(PlayerManager[] allPlayers)
    //{

    //    //        AsignarTurnoALosJugadoresLocalmente(allPlayers);
    //    PlayerManager[] allLocalPlayers = FindObjectsOfType<PlayerManager>();
    //    Array.Reverse(allLocalPlayers);
    //    //foreach (var player in allPlayers)
    //    for (int i = 0; i < allPlayers.Length; i++)
    //    {
    //        allLocalPlayers[i] = allPlayers[i];
    //    }
    //}

    [ClientRpc]

    public void EnviarTurnoATodosLosJugadores(PlayerManager[] allPlayers)
    {
        foreach (var player in allPlayers)
        {
            if (player.isPlayerTurn == true)
            {
                if (player.netId == this.netId)
                {
                    this.isPlayerTurn = true;
                }
            }
            else
            {
                this.isPlayerTurn = false;
            }
        }
    }

}

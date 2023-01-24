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
    public GameObject controlCard;
    public GameObject controlCard1;
    public GameObject controlCard2;
    public GameObject controlCard3;
    public GameObject controlCard4;
    public GameObject playerArea;
    public GameObject enemyAreaL;
    public GameObject enemyAreaR;
    public GameObject dropZone;


    //the cards List represents our deck of cards
    List<GameObject> controlCards = new List<GameObject>();

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
        

        controlCards.Add(controlCard);//todas las cartas
        controlCards.Add(controlCard1);
        controlCards.Add(controlCard2);
        controlCards.Add(controlCard3);
        controlCards.Add(controlCard4);



        //  UpdateIdPlayers(netId);
        // cards.Sort();

        // int playerId = gameObject.GetInstanceID();

        //  Debug.Log("Player ID en pm: " + playerId);

        //if (isServer) //creo que es innecesaria esta comprobacion  ya que es [Server]
        //{
        // UpdateConectedPlayers();
        // UpdateIdPlayers(playerId);   
        //    SavePlayersGameObject(gameObject);//NO USADO , estudiar que el objeto no sea siempre el mismo
        //}

        // GameManager _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        // Debug.Log("numero de jugadores en player manager: " + _gm.conectedPlayers);

        // gameObject.tag = _gm.getTag(playerId);


        UpdateIdPlayers(netId);

    }

    public void dealCards()
    {
        CmdDealCards(this.netId);

        //Debug.Log("deal cards local player net" + NetworkClient.localPlayer.netId);
        //Debug.Log("deal send player net" + netId);
    }

    //Commands are methods requested by Clients to run on the Server, and require the [Command] attribute immediately preceding them. CmdDealCards() is called by the DrawCards script attached to the client Button
    [Command]
    public void CmdDealCards(uint _netId)
    {
        //Debug.Log("local player net" + NetworkClient.localPlayer.netId);
        //Debug.Log("send player net" + _netId);

        //Spawn a random card from the cards deck on the Server, assigning authority over it to the Client that requested the Command. Then run RpcShowCard() and indicate that this card was "Dealt"
        for (int i = 0; i < 1; i++)
        {
            GameManager _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
          //  _gm.playersId.ForEach(s => Debug.Log("id de jugador " + s));

            int playedCardsNumber = _gm.cardsDealed;
            List<uint> playersIdList = _gm.playersId;
           // List<int> playersId = _gm.playersId;

            //  Debug.Log("Numero de jugadores: " + _gm.playersId.Count);
            // List<string> strings = gm.playerIds.ConvertAll<string>(x => x.ToString());
            // _gm.playersId.ForEach(s => Debug.Log("id de jugador " + s));


            if (playedCardsNumber < controlCards.Count)
            {
                GameObject card = Instantiate(controlCards[playedCardsNumber], new Vector2(0, 0), Quaternion.identity);
               // card.tag = connectionToClient.connectionId.ToString(); //solo se realiza en el servidor
               // Debug.Log("connectionToClient: " + connectionToClient.connectionId);
                NetworkServer.Spawn(card, connectionToClient);
                
                RpcShowCard(card, "Dealt", _netId, playersIdList);
               

                 
                if (isServer)
                {
                    UpdateCardsDealed();
                }  
                
            }
            else
            {
                RpcLogToClients("cartas de control agotadas!");
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

        //  card.tag = _netId.ToString();




        //Debug.Log("local player netid:" + NetworkClient.localPlayer.netId);
        //Debug.Log("send player net" + _cardNetId);

        //playersIdList.ForEach(s => Debug.Log("iduser" + s));





        // int playersNumber = playersId.Count;

        //Debug.Log(" card Tag: " + card.tag);
        // Debug.Log(" player Tag: " + gameObject.tag);



        //Debug.Log(" played _netId: " + _netId);
        //Debug.Log("player netid: " + netId);
        // Debug.Log("player conectionID: " + connectionToServer.connectionId);

        //Debug.Log("card Tag: " + card.tag);
        //  Debug.Log("card ID: " + card.GetInstanceID());

        //Debug.Log("player Tag: " + gameObject.tag);

        //if the card has been "Dealt," determine whether this Client has authority over it, and send it either to the PlayerArea or EnemyArea, accordingly. For the latter, flip it so the player can't see the front!

        uint _localUid = NetworkClient.localPlayer.netId;
        
        if (type == "Dealt")
        {
           
            if  (_localUid == _cardNetId)  
            {
              //  Debug.Log("owned card Tag: " + card.tag);
              //  Debug.Log("owned player Tag: " + gameObject.tag);

                

              //  Debug.Log("isowned player netid: " + NetworkClient.localPlayer.netId);

                //card.transform.Rotate(0, 0, -90);
                //card.gameObject.transform.localScale = new Vector3((float)0.7, (float)0.7, 0);
                card.transform.SetParent(playerArea.transform, false);
               // setEnemyCardParent(card);

            }
            else{
                setEnemyCardParent(card, _cardNetId, playersIdList);
                //Debug.Log("not owned card Tag: " + card.tag);
                //Debug.Log("not owned player Tag: " + gameObject.tag);

                // card.transform.SetParent(enemyAreaR.transform, false);

                // setEnemyCardParent(card);     
            }


            //else if (playersNumber <= 2)
            //{
            //    card.transform.Rotate(0, 0, 90);
            //    card.gameObject.transform.localScale = new Vector3((float)0.7, (float)0.7, 0);
            //    //card.transform.Rotate(0, 0, 90);
            //    card.transform.SetParent(enemyAreaR.transform, false);

            //    // Debug.Log("rotacion: " + card.transform.rotation);

            //    // card.GetComponent<CardFlipper>().Flip();
            //}
            //else
            //{                

            //    card.transform.Rotate(0, 0, -90);
            //    card.gameObject.transform.localScale = new Vector3((float)0.7, (float)0.7, 0);
            //    //card.transform.Rotate(0, 0, 90);
            //    card.transform.SetParent(enemyAreaL.transform, false);
            //}
        }
        //if the card has been "Played," send it to the DropZone. If this Client doesn't have authority over it, flip it so the player can now see the front!
        else if (type == "Played")
        {
            card.transform.SetParent(dropZone.transform, false);


            NetworkServer.Destroy(card);
            if (!isOwned)
            {
                
               // card.GetComponent<CardFlipper>().Flip(); //no es necesario ya que se destruye
            }
        }
    }

  
    void setEnemyCardParent(GameObject card, uint _cardNetId, List<uint> playersIdList)
    {
        // Debug.Log("not owned card Tag: " + card.tag);
        // Debug.Log("not owned player Tag: " + gameObject.tag);
        playersIdList.ForEach(s => Debug.Log("iduser" + s));
        Debug.Log("not Localplayer netid: " + NetworkClient.localPlayer.netId);
        Debug.Log("card _netid: " + _cardNetId);
        

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

        if(_cardNetId == leftUserId)
        {
            card.transform.SetParent(enemyAreaL.transform, false);
        }
        if (_cardNetId == rightUserId)
        {
            card.transform.SetParent(enemyAreaR.transform, false);
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
       // Debug.Log("Targeted by self!");
    }

    [TargetRpc]
    void TargetOtherCard(NetworkConnection target)
    {
       // Debug.Log("Targeted by other!");
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
}

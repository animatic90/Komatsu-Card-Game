using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : NetworkBehaviour
{
    //Card1 and Card2 are located in the inspector, whereas PlayerArea, EnemyArea, and DropZone are located at runtime within OnStartClient()    
    public GameObject ControlCard;
    public GameObject ControlCard1;
    public GameObject ControlCard2;
    public GameObject ControlCard3;
    public GameObject ControlCard4;
    public GameObject PlayerArea;
    public GameObject EnemyArea;
    public GameObject DropZone;

    //the cards List represents our deck of cards
    List<GameObject> cards = new List<GameObject>();

    public override void OnStartClient()
    {
        base.OnStartClient();

        PlayerArea = GameObject.Find("PlayerArea");
        EnemyArea = GameObject.Find("EnemyAreaL");
        DropZone = GameObject.Find("DropZone");
    }

    //when the server starts, store Card1 and Card2 in the cards deck. Note that server-only methods require the [Server] attribute immediately preceding them!
    [Server]
    public override void OnStartServer()
    {
        cards.Add(ControlCard);//todas las cartas
        cards.Add(ControlCard1);
        cards.Add(ControlCard2);
        cards.Add(ControlCard3);
        cards.Add(ControlCard4);
    }

    //Commands are methods requested by Clients to run on the Server, and require the [Command] attribute immediately preceding them. CmdDealCards() is called by the DrawCards script attached to the client Button
    [Command]
    public void CmdDealCards()
    {
        //(5x) Spawn a random card from the cards deck on the Server, assigning authority over it to the Client that requested the Command. Then run RpcShowCard() and indicate that this card was "Dealt"
        for (int i = 0; i < 1; i++)
        {
            GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            
            int pos = gm.CardsPlayed;
            Debug.Log("pos: " + pos );


            if (pos < cards.Count)
            {
                GameObject card = Instantiate(cards[pos], new Vector2(0, 0), Quaternion.identity);
                //Debug.Log("rotacionantes del spawn " + card.transform.rotation);
                NetworkServer.Spawn(card, connectionToClient);
                RpcShowCard(card, "Dealt");

                if (isServer)
                {
                    UpdateCardsPlayed();
                }
                
            }
            else
            {
                Debug.Log("Juego Terminado!!");
            }

            

        }
    }

    //PlayCard() is called by the DragDrop script when a card is placed in the DropZone, and requests CmdPlayCard() from the Server
    public void PlayCard(GameObject card)
    {
        CmdPlayCard(card);
    }

    //CmdPlayCard() uses the same logic as CmdDealCards() in rendering cards on all Clients, except that it specifies that the card has been "Played" rather than "Dealt"
    [Command]
    void CmdPlayCard(GameObject card)
    {
        RpcShowCard(card, "Played");

        //If this is the Server, trigger the UpdateTurnsPlayed() method to demonstrate how to implement game logic on card drop
        if (isServer)
        {
           //   UpdateCardsPlayed(); //UpdateTurnsPlayed(); 
        }
    }

    //UpdateTurnsPlayed() is run only by the Server, finding the Server-only GameManager game object and incrementing the relevant variable
    [Server]

    void UpdateCardsPlayed()
    {
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.UpdateCardsPlayed();//TODO
        RpcLogToClients("Cartas Jugadas: " + gm.CardsPlayed);

    }

    //RpcLogToClients demonstrates how to request all clients to log a message to their respective consoles
    [ClientRpc]
    void RpcLogToClients(string message)
    {
        Debug.Log(message);
    }

    //ClientRpcs are methods requested by the Server to run on all Clients, and require the [ClientRpc] attribute immediately preceding them
    [ClientRpc]
    void RpcShowCard(GameObject card, string type)
    {
        //if the card has been "Dealt," determine whether this Client has authority over it, and send it either to the PlayerArea or EnemyArea, accordingly. For the latter, flip it so the player can't see the front!
        if (type == "Dealt")
        {
            if (isOwned)
            {
                card.transform.Rotate(0, 0, -90);
                card.gameObject.transform.localScale = new Vector3((float)0.7, (float)0.7, 0);
                card.transform.SetParent(EnemyArea.transform, false);

               // Debug.Log("rotacion: " + card.transform.rotation);
              //  Debug.Log("son mias");
                
            }
            else
            {
                //card.transform.Rotate(0, 0, -90);
                //card.gameObject.transform.localScale = new Vector3((float)0.7, (float)0.7, 0);
                //card.transform.Rotate(0, 0, 90);
                card.transform.SetParent(PlayerArea.transform, false);

                Debug.Log("no son mias");
              //  Debug.Log("rotacion: " + card.transform.rotation);

                

                //card.GetComponent<CardFlipper>().Flip();

            }
        }
        //if the card has been "Played," send it to the DropZone. If this Client doesn't have authority over it, flip it so the player can now see the front!
        else if (type == "Played")
        {
            card.transform.SetParent(DropZone.transform, false);
            if (!isOwned)
            {
                card.GetComponent<CardFlipper>().Flip();
            }
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

using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassTurn : NetworkBehaviour
{

    public PlayerManager currentPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
      
    public void OnClick()
    {
        //PlayerManager[] allPlayers = FindObjectsOfType<PlayerManager>();

        //NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        //currentPlayer = networkIdentity.GetComponent<PlayerManager>(); //datos de quien hizo click en el botón

        //bool isTurn = currentPlayer.isPlayerTurn;
        //GameManager.Instance.EndTurn(currentPlayer, isTurn);

        EndTurn();

        //GameManager.Instance.EndTurn();
        //Debug.Log("Se presionó el botón cambiar");
        // FindObjectOfType<PlayerManager>().EndTurn();
    }
 
    public void EndTurn()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        currentPlayer = networkIdentity.GetComponent<PlayerManager>();

        //GameManager.Instance.EndTurn();
        currentPlayer.EndTurn_GameManager();
        Debug.Log("PassTurn - Se presionó el botón cambiar");

    }

}

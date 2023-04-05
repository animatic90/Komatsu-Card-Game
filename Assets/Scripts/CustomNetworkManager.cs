using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public override void OnClientConnect()
    {
        base.OnClientConnect();
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        //Debug.Log("numero de players en cnm: " + numPlayers);

        //GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        //gm.UpdateConectedPlayers(numPlayers);



       // GameObject player = Instantiate(playerPrefab);
        //NetworkServer.AddPlayerForConnection(conn, player);

        // Llenar el array de jugadores en el GameManager
       // PlayerManager playerManager = player.GetComponent<PlayerManager>();

        PlayerManager playerManager = conn.identity.GetComponent<PlayerManager>();
        if (numPlayers == 1)
        {   
            playerManager.isPlayerTurn = true;
        }
        GameManager.Instance.AddPlayer(playerManager);

        GameManager.Instance.UpdateConectedPlayers(numPlayers);

    }

}

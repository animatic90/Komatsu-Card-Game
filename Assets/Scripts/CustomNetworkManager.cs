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

       // NetworkIdentity test = conn.identity;
        Debug.Log("numero de players en cnm: " + numPlayers);

        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.UpdateConectedPlayers(numPlayers);

       // Debug.Log("numero de players en gm: " + gm.conectedPlayers);
        //   PlayerManager pm = conn.identity.GetComponent<PlayerManager>();
      
    }
  
}

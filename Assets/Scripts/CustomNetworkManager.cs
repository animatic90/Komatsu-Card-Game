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

        //GameManager gm = conn.identity.GetComponent<GameManager>();
        //gm.UpdateConectedPlayers(numPlayers);



        //   PlayerManager pm = conn.identity.GetComponent<PlayerManager>();
        // conn.identity.tag = gm.getTag(conn.identity.GetInstanceID());

        //Debug.Log("server cnm: " + conn.identity.connectionToServer.connectionId);
       // Debug.Log("connectionId cnm: " + conn.identity.connectionToClient.connectionId);
        conn.identity.tag = conn.identity.connectionToClient.connectionId.ToString();
        Debug.Log("tag cnm: " + conn.identity.tag);


    }
  
}

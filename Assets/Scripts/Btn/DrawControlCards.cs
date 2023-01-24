using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawControlCards : MonoBehaviour
{
    public PlayerManager PlayerManager;

    //OnClick() is called by the OnClick() event within the BtnControl component
    public void OnClick()
    {
        //locate the PlayerManager in this Client and request the Server to deal cards
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        PlayerManager.dealControlCards();
    }

}


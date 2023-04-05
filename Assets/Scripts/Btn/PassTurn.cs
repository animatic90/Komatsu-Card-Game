using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassTurn : MonoBehaviour
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

        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        currentPlayer = networkIdentity.GetComponent<PlayerManager>();

        bool isTurn = currentPlayer.isPlayerTurn;
        GameManager.Instance.EndTurn(currentPlayer, isTurn);

        // FindObjectOfType<PlayerManager>().EndTurn();
    }
}

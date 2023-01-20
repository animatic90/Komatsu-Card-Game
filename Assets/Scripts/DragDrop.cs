using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : NetworkBehaviour
{
    //Canvas is assigned locally at runtime in Start(), whereas the rest are assigned contextually as this gameobject is dragged and dropped
    public GameObject Canvas;
    public PlayerManager PlayerManager;

    private bool _isDragging = false;
    private bool _isOverDropZone = false;
    private bool _isDraggable = true;
    private GameObject _collisionCard; //when a card has collished with the dropzone
    private GameObject _startParent;
    private Vector2 _startPosition;

    private void Start()
    {
        Canvas = GameObject.Find("Main Canvas");

        //check whether this client hasAuthority to manipulate this gameobject
        if (!isOwned)
        {
            _isDraggable = false;
        }
    }
    void Update()
    {
        //check every frame to see if this gameobject is being dragged. If it is, make it follow the mouse and set it as a child of the Canvas to render above everything else
        if (_isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(Canvas.transform, true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //in our scene, if this gameobject collides with something, it must be the dropzone, as specified in the layer collision matrix (cards are part of the "Cards" layer and the dropzone is part of the "DropZone" layer)
        _isOverDropZone = true;
        _collisionCard = collision.gameObject;
       // Debug.Log("is enter");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _isOverDropZone = false;
        _collisionCard = null;
      //  Debug.Log("is exit"); 
    }

    //StartDrag() is called by the Begin Drag event in the Event Trigger component attached to this gameobject
    public void StartDrag()
    {
        //if the gameobject is draggable, store the parent and position of it so we know where to return it if it isn't put in a dropzone
        if (!_isDraggable) return;
        _startParent = transform.parent.gameObject;
        _startPosition = transform.position;
        _isDragging = true;
    }

    //EndDrag() is called by the End Drag event in the Event Trigger component attached to this gameobject
    public void EndDrag()
    {
        if (!_isDraggable) return;
        _isDragging = false;

        //if the gameobject is put in a dropzone, set it as a child of the dropzone and access the PlayerManager of this client to let the server know a card has been played
        if (_isOverDropZone)
        {
            //transform.SetParent(_collisionCard.transform, false);
            _isDraggable = false;
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            PlayerManager = networkIdentity.GetComponent<PlayerManager>();
            PlayerManager.PlayCard(gameObject);
        }
        //otherwise, send it back from whence it came
        else
        {
            transform.position = _startPosition;
            transform.SetParent(_startParent.transform, false);
        }
    }

    //private bool isOnArea()
    //{
    //    GameObject dropZone = GameObject.Find("DropZone");

        

    //    Debug.Log("is exit"+ gameObject.transform.position.x);
    //    Debug.Log("is exit" + gameObject.transform.position.y);
    //    Debug.Log("is exit" + gameObject.);

    //    if (gameObject.transform.position.x >1)
    //    {


    //    }
    //}
}



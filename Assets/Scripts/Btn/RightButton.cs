using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightButton : MonoBehaviour
{
    public GameObject playerArea;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AlaDerecha()
    {
        // Obtener una referencia al componente Transform del objeto
        Transform transform = playerArea.GetComponent<Transform>();

        // Obtener la posición actual del objeto
        Vector2 currentPosition = transform.position;

        // Añadir 100 unidades al eje X
        currentPosition.x += 185*3f;

        // Asignar la nueva posición al objeto
        transform.position = currentPosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    // Punto al que el jugador ser� teletransportado
    public Transform teleportDestination;

    // Tag del enemigo para detectar la colisi�n
    public string enemyTag = "Enemy";

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si colision� con un enemigo
        if (collision.gameObject.CompareTag(enemyTag))
        {
            // Teletransportar al jugador al punto de destino
            transform.position = teleportDestination.position;

            // Opcional: Si quieres mantener la rotaci�n del punto de destino
            transform.rotation = teleportDestination.rotation;

            Debug.Log("�Jugador teletransportado despu�s de colisionar con un enemigo!");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    // Punto al que el jugador será teletransportado
    public Transform teleportDestination;

    // Tag del enemigo para detectar la colisión
    public string enemyTag = "Enemy";

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si colisionó con un enemigo
        if (collision.gameObject.CompareTag(enemyTag))
        {
            // Teletransportar al jugador al punto de destino
            transform.position = teleportDestination.position;

            // Opcional: Si quieres mantener la rotación del punto de destino
            transform.rotation = teleportDestination.rotation;

            Debug.Log("¡Jugador teletransportado después de colisionar con un enemigo!");
        }
    }
}
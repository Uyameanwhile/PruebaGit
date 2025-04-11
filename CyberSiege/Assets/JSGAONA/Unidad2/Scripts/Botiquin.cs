using Assets.JSGAONA.Unidad2.Scripts;
using UnityEngine;

public class Botiquin : MonoBehaviour
{
    public PlayerCombat playerCombat;
    public int healAmount = 50; // Cantidad de vida que recupera el botiquín

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que el jugador tenga el tag "Player"
        {
            // Aquí llamas tu lógica de la vida
            other.GetComponent<PlayerCombat>().AddLifePoints(healAmount);
            Debug.Log("Vida recuperada mi pana");
            // Destruir el botiquín después de recogerlo
            Destroy(gameObject);
        }
    }
}
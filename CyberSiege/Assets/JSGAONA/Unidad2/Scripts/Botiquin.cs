using Assets.JSGAONA.Unidad2.Scripts;
using UnityEngine;

public class Botiquin : MonoBehaviour
{
    public PlayerCombat playerCombat;
    public int healAmount = 50; // Cantidad de vida que recupera el botiqu�n

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Aseg�rate de que el jugador tenga el tag "Player"
        {
            // Aqu� llamas tu l�gica de la vida
            other.GetComponent<PlayerCombat>().AddLifePoints(healAmount);
            Debug.Log("Vida recuperada mi pana");
            // Destruir el botiqu�n despu�s de recogerlo
            Destroy(gameObject);
        }
    }
}
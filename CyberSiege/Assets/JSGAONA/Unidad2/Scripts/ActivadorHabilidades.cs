using UnityEngine;
using Assets.JSGAONA.Unidad2.Scripts;

public class ActivadorHabilidades : MonoBehaviour
{
    [SerializeField] private PlayerCombat playerCombat;

    private void Start()
    {
        // Si no se asign� el playerCombat en el inspector, intentamos obtenerlo
        if (playerCombat == null)
        {
            playerCombat = FindObjectOfType<PlayerCombat>();
        }
    }

    // Este m�todo ser� visible para el bot�n en el Inspector
    public void ActivarPulsoAnulacion()
    {
        if (playerCombat != null)
        {
            playerCombat.PulsoDeAnulacion();
        }
    }
}
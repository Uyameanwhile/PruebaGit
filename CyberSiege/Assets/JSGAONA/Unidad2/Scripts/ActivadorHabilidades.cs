using UnityEngine;
using Assets.JSGAONA.Unidad2.Scripts;

public class ActivadorHabilidades : MonoBehaviour
{
    [SerializeField] private PlayerCombat playerCombat;

    private void Start()
    {
        // Si no se asignó el playerCombat en el inspector, intentamos obtenerlo
        if (playerCombat == null)
        {
            playerCombat = FindObjectOfType<PlayerCombat>();
        }
    }

    // Este método será visible para el botón en el Inspector
    public void ActivarPulsoAnulacion()
    {
        if (playerCombat != null)
        {
            playerCombat.PulsoDeAnulacion();
        }
    }
}
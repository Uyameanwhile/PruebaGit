using UnityEngine;

namespace Assets.JSGAONA.Unidad2.Scripts {

    // Script demo que permite realizar test de danio
    public class TestTakeDamage : MonoBehaviour {

        [SerializeField] private int damage = 10;

        // Metodo de llamada de Unity, se llama al momento que una colision ingresa a esta
        private void OnTriggerEnter(Collider other) {
            // Se valida que la colision que ingresa sea un jugador con el script adjunto
            if(other.TryGetComponent(out PlayerCombat player)){
                player.TakeDamage(damage);
            }
        }
    }
}
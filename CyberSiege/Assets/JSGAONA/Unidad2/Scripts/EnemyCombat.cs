using UnityEngine;
using System.Collections;

namespace Assets.JSGAONA.Unidad2.Scripts {

    public class EnemyCombat : MonoBehaviour {

        // Variables visibles desde el inspector de Unity
        [SerializeField] private GameObject attack;
        [SerializeField] private float speedAttack = 3;


        // Variables ocultas desde el inspector de Unity
        private float nextCheck = 3f;
        


        public void BasicAttack(){
            StartCoroutine(EnableAttack());
        }


        private IEnumerator EnableAttack(){
            attack.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            attack.SetActive(false);
        }

        
        // Se emplea este metodo para poder verificar el siguiente intervalo de ataque
        public bool CheckIntervalBasicAttack() {
            if (Time.timeSinceLevelLoad <= nextCheck) return false;
            // Se justa el tiempo para determinar el proximo chequeo
            nextCheck = Time.timeSinceLevelLoad + speedAttack;
            return true;
        }
    }
}
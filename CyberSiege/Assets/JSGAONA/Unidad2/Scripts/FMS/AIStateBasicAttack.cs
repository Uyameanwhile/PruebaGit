using UnityEngine;
using Assets.JSGAONA.Unidad2.Scripts.FMS.Temple;

namespace Assets.JSGAONA.Unidad2.Scripts.FMS {

    // Se emplea este scriptable object para generar el FMS del estado de ataque basico
    [CreateAssetMenu(fileName = "NewBasicAttackState", menuName = "CyberSiege/AI/BasicAttack State")]
    public class AIStateBasicAttack : AIState {

        private EnemyCombat combatEnemy;


        // Cuando el estado se inicializa
        public override void Initialize(EnemyAI enemyAi, AIStateTemplate template) {
            EnemyAi = enemyAi;
            combatEnemy = enemyAi.gameObject.GetComponent<EnemyCombat>();
        }


        // Cuando el estado Entra
        public override void Enter() {
            
        }


        // Cuando el estado se Ejecuta
        public override void Execute() {
            // Se valida el intervalo de chequeo
            if(!EnemyAi.CheckInterval()) return;
            float distance = EnemyAi.GetDistance();
            // Se valida si se encuentra a rango de alcance
            if(EnemyAi.Chase(distance)){
                EnemyAi.ChangeState(EnemyAi.GetChaseState());
            }else{                
                // Se verifica si se puede realizar un ataque
                if(!combatEnemy.CheckIntervalBasicAttack()) return;
                combatEnemy.BasicAttack();
            }
        }


        // Cuando el estado se Sale
        public override void Exit() {
            
        }
    }
}
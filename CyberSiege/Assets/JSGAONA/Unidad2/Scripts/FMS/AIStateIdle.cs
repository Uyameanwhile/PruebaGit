using UnityEngine;
using Assets.JSGAONA.Unidad2.Scripts.FMS.Temple;

namespace Assets.JSGAONA.Unidad2.Scripts.FMS {
    
    // Se emplea este scriptable object para generar el FMS del estado de reposo
    [CreateAssetMenu(fileName = "NewIdleState", menuName = "CyberSiege/AI/Idle State")]
    public class AIStateIdle : AIState {

        private AIStateIdleTemplate idleTemplate;

        // Cuando el estado se inicializa
        public override void Initialize(EnemyAI enemyMove, AIStateTemplate template) {
            EnemyAi = enemyMove;
            idleTemplate = (AIStateIdleTemplate)template;
        }


        // Cuando el estado Entra
        public override void Enter() {
            // EnemyAi.Idle();
        }


        // Cuando el estado se Ejecuta
        public override void Execute() {
            // Se valida el intervalo de chequeo
            if(!EnemyAi.CheckInterval()) return;
            float distance = EnemyAi.GetDistance();
            // El jugador a ingresado al rango de deteccion
            if(distance <= idleTemplate.DetectionDistance) {
                // Se verifica si existe no un obstaculo
                if(!EnemyAi.DetectedObstacle(distance)){
                    EnemyAi.ChangeState(EnemyAi.GetChaseState());
                }
            }
        }


        // Cuando el estado se Sale
        public override void Exit() {
            
        }
    }
}
using UnityEngine;
using Assets.JSGAONA.Unidad2.Scripts.FMS.Temple;

namespace Assets.JSGAONA.Unidad2.Scripts.FMS {

    // Se emplea este scriptable object para generar el FMS del estado de reposo
    [CreateAssetMenu(fileName = "NewResetState", menuName = "CyberSiege/AI/Reset State")]
    
    public class AIStateReset : AIState {
        // Cuando el estado se inicializa
        public override void Initialize(EnemyAI enemyMove, AIStateTemplate template) {
            EnemyAi = enemyMove;
        }


        // Cuando el estado Entra
        public override void Enter() {
            EnemyAi.ResetPositionEnemyAi();
        }


        // Cuando el estado se Ejecuta
        public override void Execute() {
            // Se valida el intervalo de chequeo
            if(!EnemyAi.CheckInterval()) return;
            if(EnemyAi.GetResetEnemyAi()) {
                EnemyAi.ChangeState(EnemyAi.GetDefaultState());
            }
        }


        // Cuando el estado se Sale
        public override void Exit() {
            
        }
    }
}
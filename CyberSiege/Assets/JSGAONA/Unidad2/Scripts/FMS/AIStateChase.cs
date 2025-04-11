using UnityEngine;
using Assets.JSGAONA.Unidad2.Scripts.FMS.Temple;

namespace Assets.JSGAONA.Unidad2.Scripts.FMS {

    // Se emplea este scriptable object para generar el FMS del estado de persecucion
    [CreateAssetMenu(fileName = "NewChaseState", menuName = "CyberSiege/AI/Chase State")]
    public class AIStateChase : AIState {

        private AIStateChaseTemplate chaseTemple;


        // Cuando el estado se inicializa
        public override void Initialize(EnemyAI entity, AIStateTemplate template) {
            EnemyAi = entity;
            chaseTemple = (AIStateChaseTemplate)template;
        }


        // Cuando el estado Entra
        public override void Enter() {
            EnemyAi.AdjustAgent(chaseTemple.MovementSpeedModifier, true);
        }


        // Cuando el estado se Ejecuta
        public override void Execute() {
            // Se valida el intervalo de chequeo
            if(!EnemyAi.CheckInterval()) return;
            float distance = EnemyAi.GetDistance();
            // La distancia es mayor al rango de persecucion
            if(distance > chaseTemple.LeaveDistance){
                // Estado de reinicio
                EnemyAi.ChangeState(EnemyAi.GetResetState());
                return;
            }
            // Ha llegado a la distancia minima, se cambia de estado
            if(!EnemyAi.Chase(distance)) {
                EnemyAi.ChangeState(EnemyAi.GetBasicAttackState());
            }
        }


        // Cuando el estado se Sale
        public override void Exit() {
            EnemyAi.AdjustAgent(chaseTemple.MovementSpeedModifier, false);
        }
    }
}
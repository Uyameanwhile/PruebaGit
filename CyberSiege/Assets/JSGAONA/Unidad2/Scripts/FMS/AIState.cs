using UnityEngine;
using Assets.JSGAONA.Unidad2.Scripts.FMS.Temple;

namespace Assets.JSGAONA.Unidad2.Scripts.FMS {
    
    // Interface que se utiliza para administrar los estados del FSM
    public abstract class AIState: ScriptableObject {

        // Propiedad que almacena la informacion del script ´EnemyMovement´
        public EnemyAI EnemyAi { get; set; }

        // Cuando el estado se inicializa
        public abstract void Initialize(EnemyAI enemyAi, AIStateTemplate template);

        // Cuando el estado Entra
        public abstract void Enter();

        // Cuando el estado se Ejecuta
        public abstract void Execute();
    
        // Cuando el estado se Sale
        public abstract void Exit();
    }
}
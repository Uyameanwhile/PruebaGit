using UnityEngine;
namespace Assets.JSGAONA.Unidad2.Scripts.FMS.Temple {

    // Se emplea este scriptable object para almacenar el comportamiento en reposo del enemigo
    [CreateAssetMenu(fileName = "NewIdleTemple", menuName = "CyberSiege/Behavior/Behavior Idle")]
    public class AIStateIdleTemplate : AIStateTemplate {
        
        // Angulo de vision
        public float ViewAngle = 60;

        // Distancia de deteccion
        public float DetectionDistance = 15;

        // Distancia de auto aggro
        public float AutoAggroDistance = 5;
    }
}
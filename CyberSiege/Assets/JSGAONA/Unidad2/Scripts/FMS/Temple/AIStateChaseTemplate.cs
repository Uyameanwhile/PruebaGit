using UnityEngine;
namespace Assets.JSGAONA.Unidad2.Scripts.FMS.Temple {

    // Se emplea este scriptable object para almacenar el comportamiento en persecucion del enemigo
    [CreateAssetMenu(fileName = "NewChaseTemple", menuName = "CyberSiege/Behavior/Behavior Chase")]
    public class AIStateChaseTemplate : AIStateTemplate {
        
        // Distancia a la que se deja de perseguir
        public float LeaveDistance = 10;

        // Velocidad de movimiento modificada
        [Range(0.1f, 5.0f)] public float MovementSpeedModifier = 1.0f;
    }
}
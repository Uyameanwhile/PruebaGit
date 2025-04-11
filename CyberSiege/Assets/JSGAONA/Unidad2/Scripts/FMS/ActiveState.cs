using Assets.JSGAONA.Unidad2.Scripts.FMS.Temple;

namespace Assets.JSGAONA.Unidad2.Scripts.FMS {
    
    // Se emplea este script para determinar los estados activos del FMS
    public class ActiveState {

        // Almacena el estado actual a ejecutar
        public AIState State;

        // Almacena los datos del estado actual a ejecutar
        public AIStateTemplate StateTemplate;
    }
}

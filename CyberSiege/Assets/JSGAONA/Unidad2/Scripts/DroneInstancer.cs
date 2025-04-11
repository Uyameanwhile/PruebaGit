using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace Assets.JSGAONA.Unidad2.Scripts {

    // Se emplea este script para generar instncias de drones con Graphics.RenderMeshInstanced
    public class DroneInstancer : MonoBehaviour {

        // Variables visibles desde el inspector de Unity
        [SerializeField] private Mesh droneMesh;
        [SerializeField] private Material droneMaterial;
        [SerializeField] private int droneCount = 50;
        [SerializeField] private float spacing = 3f;

        // Variables ocultas desde el inspector de Unity
        private float globalTime = 0;
        private readonly List<Vector4> droneParams = new();


        // Metodo de llamada de Unity, se llama una unica vez al iniciar el app, se ejecuta despues
        // de Awake, se realiza la asignacion de variables y configuracion del script
        private void Start() {
            // Se valida si el dispositivo soporta instancing en GPU 
            if (SystemInfo.supportsInstancing) {
                // Crea una cuadricula de instancias (por ejemplo, 50) separadas por spacing unidades
                for (int i = 0; i < droneCount; i++) {
                    // Posicion en eje X, Y, Z del entorno para cada instancia
                    float x = (i % 10) * spacing;
                    float z = (i / 10) * spacing;
                    float y = Random.Range(0.5f, 2.5f);

                    // Desface de animacion
                    float timeOffset = Random.Range(0f, 5f);
                    droneParams.Add(new Vector4(x, z, timeOffset, y));
                }
                
            // Se destruye el GameObject pra evitar errores
            } else {
                Destroy(gameObject);
            }
        }


        // Metodo de llamada de Unity, se llama en cada frame del computador
        // Se realiza la logica de renderizar en GPU las instncias
        private void Update() {
            //Unity solo permite instanciar hasta 1023 mallas en un solo draw call
            int maxPerCall = 1023;
            globalTime += Time.deltaTime;
            MaterialPropertyBlock mpb = new ();
            mpb.SetVectorArray("_DroneData", droneParams);
            mpb.SetFloat("_GlobalTime", globalTime);

            // Divide las instancias en lotes (batches) de hasta 1023
            // Extrae las matrices y colores para ese lote.
            for (int i = 0; i < droneParams.Count; i += maxPerCall) {
                int batchCount = Mathf.Min(maxPerCall, droneParams.Count - i);
                Matrix4x4[] matrixArray = new Matrix4x4[batchCount];

                // Convierte la posición local relativa al GameObject que contiene el script en una
                // posición global en la escena.
                for (int j = 0; j < batchCount; j++) {
                    Vector4 data = droneParams[i + j];
                    Vector3 localPos = new (data.x, 0, data.y);
                    matrixArray[j] = transform.localToWorldMatrix * Matrix4x4.TRS(localPos,
                        Quaternion.identity, Vector3.one);
                }

                // Renderizado de las instancias con un solo draw call
                Graphics.RenderMeshInstanced(
                    new RenderParams(droneMaterial) {
                        layer = gameObject.layer,
                        worldBounds = new Bounds(transform.position, Vector3.one * 1000),
                        shadowCastingMode = ShadowCastingMode.Off,
                        receiveShadows = false,
                        matProps = mpb
                    },
                    droneMesh,
                    0,
                    matrixArray
                );
            }
        }
    }
}
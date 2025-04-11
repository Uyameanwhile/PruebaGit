using UnityEngine;
using System.Collections.Generic;

namespace Assets.JSGAONA.Unidad2.Scripts {

    // Se emplea este script para generar instncias de enemigos con Graphics.RenderMeshInstanced
    public class EnemyWaveInstancer : MonoBehaviour {
        
        // Variables visibles desde el inspector de Unity
        [SerializeField] private Mesh enemyMesh;
        [SerializeField] private Material enemyMaterial;
        [SerializeField] private int enemiesPerWave = 50;
        [SerializeField] private float spacing = 2f;

        // Variables ocultas desde el inspector de Unity
        private float globalTime = 0;
        private readonly List<float> animTimes = new();
        private readonly List<Matrix4x4> matrices = new();



        // Metodo de llamada de Unity, se llama una unica vez al iniciar el app, se ejecuta despues
        // de Awake, se realiza la asignacion de variables y configuracion del script
        private void Start() {
            // Se valida si el dispositivo soporta instancing en GPU 
            if (SystemInfo.supportsInstancing) {
                // Crea una cuadricula de instancias (por ejemplo, 50x50) separadas por spacing unidades
                for (int i = 0; i < enemiesPerWave; i++) {
                    Vector3 pos = new (i % 10 * spacing, 0, i / 10 * spacing);
                    matrices.Add(Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one));
                    animTimes.Add(Random.Range(0f, 1f));
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
            mpb.SetFloatArray("_AnimTimeOffset", animTimes);
            mpb.SetFloat("_GlobalTime", globalTime);

            // Divide las instancias en lotes (batches) de hasta 1023
            // Extrae las matrices y colores para ese lote.
            for (int i = 0; i < matrices.Count; i += maxPerCall) {
                int count = Mathf.Min(maxPerCall, matrices.Count - i);
                var matrixArray = matrices.GetRange(i, count).ToArray();
                mpb.SetFloat("_GlobalTime", globalTime);

                // Renderizado de las instancias con un solo draw call
                Graphics.RenderMeshInstanced(
                    new RenderParams(enemyMaterial) {
                        matProps = mpb
                    },
                    enemyMesh,
                    0,
                    matrixArray
                );
            }
        }
    }
}
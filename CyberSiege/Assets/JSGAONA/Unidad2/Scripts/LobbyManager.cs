using Jsgaona;
using UnityEngine;

namespace Assets.JSGAONA.Unidad2.Scripts {

    public class LobbyManager : MonoBehaviour {

        // Indice de escena que se ha selecionado
        [SerializeField] private int indexScene = 0;

        // Referencia del prefab del cargador de escenas
        [SerializeField] private GameObject loadingSceneManager;
        
        

        // Metodo de llamada de Unity, se llamada una unica vez al iniciar el app despues de Awake
        // Se inicializa las variables principales de modificacion
        private void Start() {
            // Si no existe en escena una referencia valida del administrador de escenas, se lo crea
            if(FindObjectOfType<SceneLoadingManager>() == null){
                Instantiate(loadingSceneManager, Vector2.zero, Quaternion.identity);
            }
        }


        // Metodo que permite dar inicio al juego
        public void StartGame(){
            SceneLoadingManager.SceneInstance.LoadGameScene(indexScene);
        }



        // Metodo que permite cerrar el juego
        public void ExitGame(){
            Application.Quit();
        }
    }
}
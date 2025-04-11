using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Jsgaona {

    // Script que administra la gestion de cambio y carga de escenas
    public class SceneLoadingManager : MonoBehaviour {

        [Header("Component transition")]
        // Referencia de la imagen de transicion
        [SerializeField] private Image fadeImage;

        // duracion de la transicion de entrada
        [SerializeField] private float fadeInDuration = 1.5f;

        // duracion de la transicion de salida
        [SerializeField] private float fadeOutDuration = 0.75f;

        // Permite validar si una scena ya se encuentra cargandose
        private bool loading = false;

        // Se emplea el patron Singleton, para permitir una unica instancia del administrador
        public static SceneLoadingManager SceneInstance { private set; get; }



        // Metodo de llamada de Unity, se llama una unica vez al iniciar el aplicativo
        // Se declaran todos los componentes necesarios para el funcionamiento del script
        private void Awake(){
            // Asegura que solo haya una instancia de esta clase 'Patron de disenio Singleton'
            if(SceneInstance == null) {
                SceneInstance = this;
                DontDestroyOnLoad(gameObject);
            }else{
                Destroy(gameObject);
            }
        }


        // Metodo de llamada de Unity, se llama una unica vez cuando el objeto es destruido
        private void OnDestroy() {
            SceneInstance = null;
        }


        // Se utiliza este metodo para poder cargar una escena de manera asincrona
        public void LoadGameScene(int idScene){
            if(!loading) StartCoroutine(LoadSceneAsync(idScene));
        }
        

        // Coroutina que se emplea para generar transicion
        private IEnumerator Fade(float startAlpha, float endAlpha, float duration) {
            // Variables comunes para todos los casos
            float elapsedTime = 0;
            Color color = fadeImage.color;
            // Ciclo repetitivo que permite generar la transicion
            while (elapsedTime < duration) {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                color.a = alpha;
                fadeImage.color = color;
                yield return null;
            }
            color.a = endAlpha;
            fadeImage.color = color;
        }


        // Corutina que maneja la carga asincrona
        private IEnumerator LoadSceneAsync(int idScene){
            loading = true;
            // Transicion de salida
            yield return Fade(0, 1, fadeOutDuration);

            // Inicia la carga de la escena de manera asincrona
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(idScene);

            // Muestra un mensaje mientras la escena se esta cargando
            //Debug.Log("Cargando escena...");

            // Espera hasta que la escena este completamente cargada
            while (!asyncOperation.isDone){
                // Puedes mostrar una barra de progreso aqui, ya que asyncOperation.progress va de 0 a 0.9
                // Debug.Log($"Progreso de la carga: {asyncOperation.progress * 100}%");
                yield return null;  // Espera al siguiente frame
            }
            // La escena esta completamente cargada
            yield return Fade(1, 0, fadeInDuration);
            loading = false;
        }
    }
}
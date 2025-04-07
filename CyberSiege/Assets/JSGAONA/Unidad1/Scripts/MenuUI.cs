using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;
namespace Assets.JSGAONA.Unidad1.Scripts
{
    
    public class MenuUI : MonoBehaviour
    {
        public TMP_Dropdown dropdown;
        public int calidad;
        public AudioMixer audioMixer;

        private void Start()
        {
            calidad = PlayerPrefs.GetInt("numeroDeCalidad", 1);
            dropdown.value = calidad;
            AjustarCalidad();
        }

        public void AjustarCalidad()
        {
            QualitySettings.SetQualityLevel(dropdown.value);
            PlayerPrefs.SetInt("numeroDeCalidad", dropdown.value);
            calidad = dropdown.value;
        }
        public void SetVolume(float volume)
        {
            audioMixer.SetFloat("volume", volume);
        }
        public void PlayGame()
        {
            SceneManager.LoadScene(1);
        }
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
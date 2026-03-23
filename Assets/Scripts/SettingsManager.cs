using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider sensitivitySlider;
    public Slider volumeSlider;
    public GameObject optionsPanel;

    [Header("Menu Audion")]
    public AudioSource menuAudioSource;

    // Variables statiques accessibles partout
    public static int Sensitivity = 25;
    public static float GlobalVolume = 0.8f;

    void Start()
    {
        // On charge les anciennes valeurs ou on met les défauts
        sensitivitySlider.value = PlayerPrefs.GetInt("PrefSensitivity", 25);
        volumeSlider.value = PlayerPrefs.GetFloat("PrefVolume", 0.8f);

        UpdateValues();
    }

    // Appelé quand on bouge les sliders (Evénement OnValueChanged)
    public void UpdateValues()
    {
        Sensitivity = (int)sensitivitySlider.value;
        GlobalVolume = volumeSlider.value;

        if (menuAudioSource != null) // Son du menu doit aussi être affecté par le volume global
        {
            menuAudioSource.volume = GlobalVolume;
        }

        // Sauvegarde pour le prochain lancement
        PlayerPrefs.SetInt("PrefSensitivity", Sensitivity);
        PlayerPrefs.SetFloat("PrefVolume", GlobalVolume);
    }

    public void OpenOptions() { optionsPanel.SetActive(true); }
    public void CloseOptions() { optionsPanel.SetActive(false); }
}
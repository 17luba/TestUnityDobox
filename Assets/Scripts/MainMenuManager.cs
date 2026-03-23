using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Les Panneaux")]
    public GameObject choicePanel;
    public GameObject tutNormalPanel;
    public GameObject tutHandicapPanel;

    // Cette variable statique va "survivre" au changement de scène
    public static bool selectedHandicapMode = false;

    void Start()
    {
        // On cache tout au début
        choicePanel.SetActive(false);
        tutNormalPanel.SetActive(false);
        tutHandicapPanel.SetActive(false);
    }

    // --- ÉTAPE 1 : Cliquer sur PLAY ---
    public void OpenChoice()
    {
        choicePanel.SetActive(true);
    }

    // --- ÉTAPE 2 : Choisir le mode ---
    public void SelectNormal()
    {
        selectedHandicapMode = false;
        choicePanel.SetActive(false);
        tutNormalPanel.SetActive(true);
    }

    public void SelectHandicap()
    {
        selectedHandicapMode = true;
        choicePanel.SetActive(false);
        tutHandicapPanel.SetActive(true);
    }

    // --- ÉTAPE 3 : Lancer le jeu ---
    public void StartGame()
    {
        SceneManager.LoadScene(1); // Charge la scène de jeu (index 1)
    }

    public void QuitGame()
    {
        Debug.Log("Quitter le jeu...");
        Application.Quit();
    }
}
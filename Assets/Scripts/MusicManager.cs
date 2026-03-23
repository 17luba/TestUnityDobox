using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;


public class MusicManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject notePrefab;
    public Transform spawnLeft;
    public Transform spawnRight;
    public float spawnInterval = 1.5f;
    public float gameDuration = 60f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip soundLeft;
    public AudioClip soundRight;

    [Header("UI Gameplay")]
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI timerText;

    [Header("UI Fin de Partie")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;


    private bool gameStarted = false;
    private bool gameActive = false;
    private float timeRemaining;

    // Instance statique pour que les notes puissent y accéder facilement
    public static MusicManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        timeRemaining = gameDuration;
        StartCoroutine(StartGameRoutine());
    }

    void Update()
    {
        if (gameActive)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = "TEMPS: " + Mathf.CeilToInt(timeRemaining).ToString();
            if (timeRemaining <= 0f)
            {
                EndGame();
            }
        }

        // Volume global (pour les sons de notes)
        if (audioSource != null)
        {
            audioSource.volume = SettingsManager.GlobalVolume;
        }
    }

    // Fonction appelée par les notes au moment du Hit
    public void PlayNoteSound(string side)
    {
        if (side == "Gauche")
            audioSource.PlayOneShot(soundLeft);
        else
            audioSource.PlayOneShot(soundRight);
    }

    IEnumerator StartGameRoutine()
    {
        int count = 3;

        while (count > 0)
        {
            countdownText.text = count.ToString();

            countdownText.transform.localScale = Vector3.one * 1.5f; // petit effet visuel "pop"

            count--;
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "<size=100>GO!";
        yield return new WaitForSeconds(0.5f);

        countdownText.gameObject.SetActive(false); // On cache le texte après le compte à rebours


        gameActive = true;
        StartCoroutine(SpawnRoutine()); // Démarre la génération des notes après le compte à rebours
    }

    IEnumerator SpawnRoutine()
    {
        while (gameActive)
        {
            yield return new WaitForSeconds(spawnInterval);
            bool isLeft = Random.value > 0.5f;
            Transform t = isLeft ? spawnLeft : spawnRight;

            GameObject newNote = Instantiate(notePrefab, t.position, Quaternion.identity);
            NoteObject script = newNote.GetComponent<NoteObject>();
            script.side = isLeft ? "Gauche" : "Droite";
            script.speed = 4f;
        }
    }

    void EndGame()
    {
        gameActive = false;
        gameOverPanel.SetActive(true);

        // Gestion du meilleur score (Sauvegarde locale)
        int oldHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (ScoreManager.currentScore > oldHighScore)
        {
            PlayerPrefs.SetInt("HighScore", ScoreManager.currentScore);
            oldHighScore = ScoreManager.currentScore;
        }

        finalScoreText.text = "SCORE FINAL: " + ScoreManager.currentScore;
        highScoreText.text = "MEILLEUR SCORE: " + oldHighScore;
    }

    // Fonctions pour les boutons
    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu"); // On créera cette scène après
    }
}
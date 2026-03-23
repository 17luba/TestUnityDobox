using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public GameObject notePrefab;
    public Transform spawnLeft;
    public Transform spawnRight;
    public float spawnInterval = 1.5f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip soundLeft;
    public AudioClip soundRight;

    // Instance statique pour que les notes puissent y accéder facilement
    public static MusicManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    // Fonction appelée par les notes au moment du Hit
    public void PlayNoteSound(string side)
    {
        if (side == "Gauche")
            audioSource.PlayOneShot(soundLeft);
        else
            audioSource.PlayOneShot(soundRight);
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
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
}
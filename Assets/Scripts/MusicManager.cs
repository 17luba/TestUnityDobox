using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public GameObject notePrefab;
    public Transform spawnLeft;
    public Transform spawnRight;

    [Header("Réglages Rythme")]
    public float bpm = 60f; // Battements par minute
    public float noteSpeed = 4f;

    void Start()
    {
        float interval = 60f / bpm;
        StartCoroutine(SpawnRoutine(interval));
    }

    IEnumerator SpawnRoutine(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            // Choisir un côté au hasard
            bool isLeft = Random.value > 0.5f;
            Transform targetSpawn = isLeft ? spawnLeft : spawnRight;

            // Créer la note
            GameObject note = Instantiate(notePrefab, targetSpawn.position, Quaternion.identity);

            // Configurer la note
            NoteObject noteScript = note.GetComponent<NoteObject>();
            noteScript.side = isLeft ? "Gauche" : "Droite";
            noteScript.speed = noteSpeed;
        }
    }
}
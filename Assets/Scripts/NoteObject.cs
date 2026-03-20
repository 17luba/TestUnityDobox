using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public float speed = 5f;
    public string side; // "Gauche" ou "Droite"
    private bool isInHitZone = false;

    void Update()
    {
        // La note monte vers le haut
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // Si la note dépasse le haut de l'écran sans être touchée
        if (transform.position.y > 6f)
        {
            Destroy(gameObject);
            Debug.Log("Raté !");
        }

        // VÉRIFICATION DE LA FRAPPE
        // On regarde si l'action OpenCV correspond au côté de cette note
        if (isInHitZone && CameraManager.lastAction == side)
        {
            HitNote();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HitZone"))
        {
            isInHitZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HitZone"))
        {
            isInHitZone = false;
        }
    }

    void HitNote()
    {
        Debug.Log("<color=yellow>PARFAIT ! Note " + side + " validée.</color>");
        // Tu peux ajouter un effet de particules ici
        Destroy(gameObject);
    }
}
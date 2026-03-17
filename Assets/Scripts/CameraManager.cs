using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;

// On définit que par défaut, "Rect" dans ce script désigne celui de Unity pour tes variables de zone
using Rect = UnityEngine.Rect;

public class CameraManager : MonoBehaviour
{
    private WebCamTexture webCamTexture;
    private Mat rgbaMat;
    private Mat grayMat;
    private Mat prevGrayMat;
    private Mat diffMat;
    private Texture2D texture;
    public RawImage displayImage;

    [Header("Paramètres de Détection")]
    public float thresholdMovement = 25f; // Sensibilité (plus bas = plus sensible)
    public int minPixelCount = 500;       // Nombre de pixels minimum pour valider un mouvement

    // Zones de détection (en pixels)
    private Rect zoneLeft = new Rect(50, 50, 150, 150);
    private Rect zoneRight = new Rect(440, 50, 150, 150);

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            webCamTexture = new WebCamTexture(devices[0].name, 640, 480, 30);
            webCamTexture.Play();

            // Initialisation des matrices
            rgbaMat = new Mat(480, 640, MatType.CV_8UC4);
            grayMat = new Mat();
            prevGrayMat = new Mat();
            diffMat = new Mat();

            texture = new Texture2D(640, 480, TextureFormat.RGBA32, false);
            displayImage.texture = texture;
        }
    }

    void Update()
    {
        if (webCamTexture != null && webCamTexture.didUpdateThisFrame)
        {
            // 1. Conversion WebCam vers Mat
            rgbaMat = OpenCvSharp.Unity.TextureToMat(webCamTexture);

            // Effet miroir (très important pour le gameplay)
            Cv2.Flip(rgbaMat, rgbaMat, FlipMode.Y);

            // 2. Préparation de l'image pour la détection
            Cv2.CvtColor(rgbaMat, grayMat, ColorConversionCodes.RGBA2GRAY);
            Cv2.GaussianBlur(grayMat, grayMat, new Size(5, 5), 0);

            if (!prevGrayMat.Empty())
            {
                // 3. Calcul de la différence de mouvement
                Cv2.Absdiff(grayMat, prevGrayMat, diffMat);
                Cv2.Threshold(diffMat, diffMat, thresholdMovement, 255, ThresholdTypes.Binary);

                // 4. Analyse des zones
                CheckZone(diffMat, zoneLeft, "Gauche");
                CheckZone(diffMat, zoneRight, "Droite");
            }

            // Sauvegarde pour la frame suivante
            grayMat.CopyTo(prevGrayMat);

            // 5. Feedback visuel sur l'image finale
            DrawZones();

            // 6. Affichage
            OpenCvSharp.Unity.MatToTexture(rgbaMat, texture);
        }
    }

    void CheckZone(Mat source, Rect zone, string side)
    {
        // Conversion explicite du Rect Unity vers le Rect OpenCV
        OpenCvSharp.Rect cvRect = new OpenCvSharp.Rect((int)zone.x, (int)zone.y, (int)zone.width, (int)zone.height);

        // Vérification de sécurité pour ne pas sortir des limites de l'image
        cvRect = cvRect.Intersect(new OpenCvSharp.Rect(0, 0, source.Cols, source.Rows));

        // On extrait la zone d'intérêt (ROI)
        using (Mat roi = new Mat(source, cvRect))
        {
            int movementPixels = Cv2.CountNonZero(roi);

            if (movementPixels > minPixelCount)
            {
                Debug.Log("<color=green>MOUVEMENT DÉTECTÉ : </color>" + side + " (" + movementPixels + " px)");
            }
        }
    }

    void DrawZones()
    {
        // On dessine avec des OpenCvSharp.Point pour éviter les conflits
        Cv2.Rectangle(rgbaMat,
            new OpenCvSharp.Point((int)zoneLeft.x, (int)zoneLeft.y),
            new OpenCvSharp.Point((int)(zoneLeft.x + zoneLeft.width), (int)(zoneLeft.y + zoneLeft.height)),
            Scalar.Green, 2);

        Cv2.Rectangle(rgbaMat,
            new OpenCvSharp.Point((int)zoneRight.x, (int)zoneRight.y),
            new OpenCvSharp.Point((int)(zoneRight.x + zoneRight.width), (int)(zoneRight.y + zoneRight.height)),
            Scalar.Green, 2);
    }

    private void OnDestroy()
    {
        // Nettoyage de la mémoire native
        if (rgbaMat != null) rgbaMat.Dispose();
        if (grayMat != null) grayMat.Dispose();
        if (prevGrayMat != null) prevGrayMat.Dispose();
        if (diffMat != null) diffMat.Dispose();
    }
}
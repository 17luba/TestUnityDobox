using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;
using System.IO;

// On définit que par défaut, "Rect" dans ce script désigne celui de Unity
using Rect = UnityEngine.Rect;

public class CameraManager : MonoBehaviour
{
    [Header("Composants Affichage")]
    public RawImage displayImage;
    private WebCamTexture webCamTexture;
    private Texture2D texture;

    [Header("Matrices OpenCV")]
    private Mat rgbaMat;
    private Mat grayMat;
    private Mat prevGrayMat;

    [Header("Classifiers (Face & Eyes)")]
    private CascadeClassifier faceCascade;
    private CascadeClassifier eyeCascade;

    [Header("Réglages Gameplay")]
    public bool modeHandicap = false; // True = Tête, False = Mains
    public float angleThreshold = 15f; // Degrés d'inclinaison pour la tête
    public int thresholdMovement = 25; // Sensibilité mouvement pixels
    public int minPixelCount = 500;    // Seuil de détection pour les mains

    [Header("Zones de Détection (Mains)")]
    public Rect handZoneLeft = new Rect(50, 50, 150, 150);
    public Rect handZoneRight = new Rect(440, 50, 150, 150);

    // Variable Statique pour que le script des Notes puisse la lire
    public static string lastAction = "";
    private float actionTimer = 0f;

    void Start()
    {
        // Choix du mode (handicap ou normal) basé sur la sélection du menu
        modeHandicap = MainMenuManager.selectedHandicapMode;

        // 1. Chargement des fichiers XML (doivent être dans Assets/StreamingAssets)
        string facePath = Path.Combine(Application.streamingAssetsPath, "haarcascade_frontalface_default.xml");
        string eyePath = Path.Combine(Application.streamingAssetsPath, "haarcascade_eye.xml");

        faceCascade = new CascadeClassifier(facePath);
        eyeCascade = new CascadeClassifier(eyePath);

        // 2. Initialisation WebCam
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            webCamTexture = new WebCamTexture(devices[0].name, 640, 480, 30);
            webCamTexture.Play();

            rgbaMat = new Mat(480, 640, MatType.CV_8UC4);
            grayMat = new Mat();
            prevGrayMat = new Mat();

            texture = new Texture2D(640, 480, TextureFormat.RGBA32, false);
            displayImage.texture = texture;
        }
    }

    void Update()
    {
        if (webCamTexture != null && webCamTexture.didUpdateThisFrame)
        {
            // 1. On récupère l'image actuelle
            rgbaMat = OpenCvSharp.Unity.TextureToMat(webCamTexture);
            Cv2.Flip(rgbaMat, rgbaMat, FlipMode.Y);

            // 2. On prépare la version Grise pour les calculs
            Cv2.CvtColor(rgbaMat, grayMat, ColorConversionCodes.RGBA2GRAY);
            Cv2.GaussianBlur(grayMat, grayMat, new Size(5, 5), 0); // Réduit le bruit numérique

            // 3. ON DÉTECTE (Seulement si on a une image précédente avec laquelle comparer)
            if (prevGrayMat != null && !prevGrayMat.Empty())
            {
                if (modeHandicap)
                {
                    DetectTiltByEyes();
                }
                else
                {
                    DetectHandMovement();
                }
            }

            // 4. Feedback visuel (Dessin des carrés)
            DrawUIZones();
            UpdateActionTimer();

            // 5. ON SAUVEGARDE l'image actuelle pour la frame suivante (IMPORTANT)
            if (prevGrayMat.Empty() || prevGrayMat.Size() != grayMat.Size())
            {
                prevGrayMat = grayMat.Clone();
            }
            else
            {
                grayMat.CopyTo(prevGrayMat);
            }

            // 6. Affichage écran
            OpenCvSharp.Unity.MatToTexture(rgbaMat, texture);
        }
    }

    // --- LOGIQUE TÊTE (HANDICAP) ---
    void DetectTiltByEyes()
    {
        var faces = faceCascade.DetectMultiScale(grayMat, 1.1, 5);
        if (faces.Length == 0) return;

        OpenCvSharp.Rect f = faces[0];
        Cv2.Rectangle(rgbaMat, f, Scalar.Yellow, 2);

        // Zone des yeux (haut du visage)
        OpenCvSharp.Rect eyeRegion = new OpenCvSharp.Rect(f.X, f.Y + (f.Height / 5), f.Width, f.Height / 2);
        using (Mat faceROI = new Mat(grayMat, eyeRegion))
        {
            var eyes = eyeCascade.DetectMultiScale(faceROI, 1.1, 2, 0, new Size(30, 30));

            if (eyes.Length >= 2)
            {
                System.Array.Sort(eyes, (a, b) => a.X.CompareTo(b.X));
                Point p1 = new Point(eyeRegion.X + eyes[0].X + eyes[0].Width / 2, eyeRegion.Y + eyes[0].Y + eyes[0].Height / 2);
                Point p2 = new Point(eyeRegion.X + eyes[1].X + eyes[1].Width / 2, eyeRegion.Y + eyes[1].Y + eyes[1].Height / 2);

                Cv2.Line(rgbaMat, p1, p2, Scalar.Red, 2);

                float deltaY = p2.Y - p1.Y;
                float deltaX = p2.X - p1.X;
                float angle = Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg;

                if (angle > angleThreshold) TriggerHit("Droite");
                else if (angle < -angleThreshold) TriggerHit("Gauche");
            }
        }
    }

    // --- LOGIQUE MAINS (NORMAL) ---
    void DetectHandMovement()
    {
        if (prevGrayMat == null || prevGrayMat.Empty()) return;

        CheckMovement(UnityToOpenCVRect(handZoneLeft), minPixelCount, "Gauche");
        CheckMovement(UnityToOpenCVRect(handZoneRight), minPixelCount, "Droite");
    }

    void CheckMovement(OpenCvSharp.Rect area, int thresholdPixels, string sideTag)
    {
        if (prevGrayMat == null || prevGrayMat.Empty()) return;

        area = area.Intersect(new OpenCvSharp.Rect(0, 0, grayMat.Cols, grayMat.Rows));
        if (area.Width <= 0 || area.Height <= 0) return;

        using (Mat currROI = new Mat(grayMat, area))
        using (Mat prevROI = new Mat(prevGrayMat, area))
        using (Mat diffROI = new Mat())
        {
            Cv2.Absdiff(currROI, prevROI, diffROI);
            Cv2.Threshold(diffROI, diffROI, thresholdMovement, 255, ThresholdTypes.Binary);

            int movement = Cv2.CountNonZero(diffROI);

            // Ligne à ajouter pour débugger : elle affiche le flux de pixels même sans détection
            // Debug.Log(sideTag + " : " + movement + " pixels en mouvement"); 

            if (movement > thresholdPixels)
            {
                TriggerHit(sideTag);
                Debug.Log("<color=green>SUCCESS : </color>" + sideTag);
            }
        }
    }

    // --- UTILITAIRES ---
    void TriggerHit(string side)
    {
        lastAction = side;
        actionTimer = 0.2f;
        Debug.Log("<color=orange>ACTION : </color>" + side);
    }

    void UpdateActionTimer()
    {
        if (actionTimer > 0) actionTimer -= Time.deltaTime;
        else lastAction = "";
    }

    void DrawUIZones()
    {
        Scalar color = modeHandicap ? new Scalar(255, 255, 0, 50) : Scalar.Green;
        Cv2.Rectangle(rgbaMat, UnityToOpenCVRect(handZoneLeft), color, 2);
        Cv2.Rectangle(rgbaMat, UnityToOpenCVRect(handZoneRight), color, 2);
    }

    OpenCvSharp.Rect UnityToOpenCVRect(Rect r)
    {
        return new OpenCvSharp.Rect((int)r.x, (int)r.y, (int)r.width, (int)r.height);
    }

    private void OnDisable()
    {
        // Arrête la capture de la webcam pour libérer le hardware
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }


    private void OnDestroy()
    {
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
            webCamTexture = null;
        }

        if (rgbaMat != null) rgbaMat.Dispose();
        if (grayMat != null) grayMat.Dispose();
        if (prevGrayMat != null) prevGrayMat.Dispose();
        if (faceCascade != null) faceCascade.Dispose();
        if (eyeCascade != null) eyeCascade.Dispose();
    }

   
}
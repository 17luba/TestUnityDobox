using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp; // On garde uniquement celui-ci
using OpenCvSharp.Demo; // Utile pour certaines conversions si présentes

public class CameraManager : MonoBehaviour
{
    private WebCamTexture webCamTexture;
    private Mat rgbaMat;
    private Texture2D texture;
    public RawImage displayImage;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            // Initialisation de la caméra
            webCamTexture = new WebCamTexture(devices[0].name, 640, 480, 30);
            webCamTexture.Play();

            // Initialisation de la Mat (OpenCvSharp utilise des constructeurs simples)
            // CV_8UC4 = 8 bits, Unsigned, 4 canaux (RGBA)
            rgbaMat = new Mat(480, 640, MatType.CV_8UC4);
            texture = new Texture2D(640, 480, TextureFormat.RGBA32, false);

            displayImage.texture = texture;
        }
    }

    void Update()
    {
        if (webCamTexture != null && webCamTexture.didUpdateThisFrame)
        {
            // Conversion WebCamTexture vers Mat (Syntaxe OpenCVSharp)
            // Note : Unity.TextureToMat est souvent fourni dans les helpers de l'asset
            rgbaMat = OpenCvSharp.Unity.TextureToMat(webCamTexture);

            // --- C'est ici que tu ajouteras tes filtres (Gris, Canny, etc.) ---

            // Conversion Mat vers Texture2D pour l'affichage
            OpenCvSharp.Unity.MatToTexture(rgbaMat, texture);
        }
    }
}
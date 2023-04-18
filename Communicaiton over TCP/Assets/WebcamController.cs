using UnityEngine;
using UnityEngine.UI;

public class WebcamController : MonoBehaviour
{
    public RawImage rawImage;

    void Start()
    {
        WebCamTexture webcamTexture = new WebCamTexture();
        rawImage.texture = webcamTexture;
        rawImage.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }
}
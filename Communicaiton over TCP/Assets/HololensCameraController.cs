using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

public class HololensCameraController : MonoBehaviour
{
    public RawImage rawImage;
    public int captureIntervalSeconds = 1;

    private PhotoCapture photoCaptureObject;
    private Texture2D targetTexture;

    void Start()
    {
        // Create a target texture to render the camera image onto
        targetTexture = new Texture2D(1280, 720, TextureFormat.RGBA32, false);

        // Start taking pictures
        InvokeRepeating("TakePicture", 0, captureIntervalSeconds);
    }

    void OnDestroy()
    {
        // Stop taking pictures when this script is destroyed
        CancelInvoke();
    }

    void TakePicture()
    {
        // Create a new PhotoCapture object
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;

            // Set up the camera parameters
            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.cameraResolutionWidth = targetTexture.width;
            cameraParameters.cameraResolutionHeight = targetTexture.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            // Start the camera capture
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result)
            {
                // Take a picture
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            });
        });
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        // Copy the photo data into the target texture
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);

        // Display the target texture on the raw image
        rawImage.texture = targetTexture;

        // Stop the camera capture
        photoCaptureObject.StopPhotoModeAsync(delegate (PhotoCapture.PhotoCaptureResult stopResult)
        {
            photoCaptureObject.Dispose();
            photoCaptureObject = null;
        });
    }
}
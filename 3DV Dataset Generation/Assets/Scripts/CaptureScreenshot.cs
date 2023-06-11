using UnityEngine;

public class CaptureScreenshot : MonoBehaviour
{
    public bool capture;
    public string fileName;
    public int pictureIndex;

    void Update()
    {
        
    }

    public void TakeScreenshot(string path)
    {
        if (capture)
        {
            fileName = "Data Point " + (pictureIndex).ToString() + ".png";

            ScreenCapture.CaptureScreenshot(path + fileName);

            Debug.Log("Data Point Generated");
        }
    }
}
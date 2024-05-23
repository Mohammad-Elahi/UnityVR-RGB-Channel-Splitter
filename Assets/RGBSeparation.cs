using System.Collections;
using System.IO;
using UnityEngine;

public class RGBSeparation : MonoBehaviour
{
    public string folderPath = "U:/Project/UnityVR-RGB-Channel-Splitter/Recordings";
    public int frameRate = 30;

    private void Start()
    {
        // Set the playback framerate
        Time.captureFramerate = frameRate;

        // Create the folder if not exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }
    private void Update()
    {
        // Generate filename based on the current frame count (format is '0005 shot.png"')
        string name = string.Format("{0}/{1:D04} shot.png", folderPath, Time.frameCount);

        // Capture the screenshot to the file.
        ScreenCapture.CaptureScreenshot(name);

        // Wait for the screenshot to be saved, then split it before loading.
        StartCoroutine(LoadAndSplitScreenshot(name));
    }
        // load the screenshot from the file path, splits it to RGB channels and saves each channel as a image file.  
    private IEnumerator LoadAndSplitScreenshot(string filePath)
    {
        // Wait for the screenshot to be saved
        yield return new WaitForSeconds(0.0625f);

        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Load the screenshot as a Texture2D
            Texture2D screenshot = LoadPNG(filePath);

            // Split the screenshot into RGB channels
            Texture2D red, green, blue;
            SplitIntoChannels(screenshot, out red, out green, out blue);

            // Save the channels as separate images
            SaveAsPNG(red, string.Format("{0}/{1:D04} red.png", folderPath, Time.frameCount));
            SaveAsPNG(green, string.Format("{0}/{1:D04} green.png", folderPath, Time.frameCount));
            SaveAsPNG(blue, string.Format("{0}/{1:D04} blue.png", folderPath, Time.frameCount));
        }
        else
        {
            Debug.LogError("File does not exist: " + filePath);
        }
    }
//load an image file as Texture2D by reading its bytes,creating new Texture2D instance,and loading the image data into it.
    private Texture2D LoadPNG(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

// This method takes an original image as input
private void SplitIntoChannels(Texture2D original, out Texture2D red, out Texture2D green, out Texture2D blue)
{   
     //Creates separate Texture2D objects for red, green, and blue channels

    red = new Texture2D(original.width, original.height);
    green = new Texture2D(original.width, original.height);
    blue = new Texture2D(original.width, original.height);

//Processes each pixel to extract RGB values and assigns them to corresponding channels.

    Color[] originalPixels = original.GetPixels();
    Color[] redPixels = new Color[originalPixels.Length];
    Color[] greenPixels = new Color[originalPixels.Length];
    Color[] bluePixels = new Color[originalPixels.Length];

    for (int i = 0; i < originalPixels.Length; i++)
    {
        Color pixel = originalPixels[i];
        redPixels[i] = new Color(pixel.r, 0, 0); // Visualize the red channel
        greenPixels[i] = new Color(0, pixel.g, 0); // Visualize the green channel
        bluePixels[i] = new Color(0, 0, pixel.b); // Visualize the blue channel
    }

    red.SetPixels(redPixels);
    red.Apply();

    green.SetPixels(greenPixels);
    green.Apply();

    blue.SetPixels(bluePixels);
    blue.Apply();
}
    private void SaveAsPNG(Texture2D tex, string filePath)
    {
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
    }
}


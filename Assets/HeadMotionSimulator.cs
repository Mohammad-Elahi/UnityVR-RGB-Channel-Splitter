using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class HeadMotionSimulator : MonoBehaviour
{
    public string csvFilePath = "U:/Project/UnityVR-RGB-Channel-Splitter/head_motion.csv";
    private List<HeadMotionData> motionData;
    private int currentIndex = 0;
    private float timeBetweenRows = 0.001f; // 1 millisecond

    void Start()
    {
        // Read and parse the .csv file
        motionData = ReadCSVFile(csvFilePath);
        Time.timeScale = 1; // Set the time scale to 1 for one millisecond runtime between rows
    }

    void Update()
    {
        if (motionData != null && motionData.Count > 0)
        {
            // Update cube position and rotation based on the motion data
            transform.position = new Vector3(motionData[currentIndex].x, motionData[currentIndex].y, motionData[currentIndex].z);
            transform.rotation = new Quaternion(motionData[currentIndex].qx, motionData[currentIndex].qy, motionData[currentIndex].qz, motionData[currentIndex].qw);

            // Update currentIndex for the next frame
            currentIndex++;

            // Check if this is the last row
            if (currentIndex >= motionData.Count)
            {
                // Stop the program
                Application.Quit();
            }

            // Wait for 1 milisecond before processing the next row
            StartCoroutine(WaitForNextRow());
        }
    }

    private IEnumerator WaitForNextRow()
    {
        yield return new WaitForSeconds(timeBetweenRows);
    }

    private List<HeadMotionData> ReadCSVFile(string filePath)
    {
        List<HeadMotionData> data = new List<HeadMotionData>();
        if (File.Exists(filePath))
        {
            // Read the lines of the .csv file
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 1; i < lines.Length; i++) // Start from 1 to skip the header
            {
                string[] values = lines[i].Split(',');
                if (values.Length >= 8) // Ensure all columns are present
                {
                    HeadMotionData motion = new HeadMotionData();
                    // Parse and assign the values to the HeadMotionData object
                    motion.time = float.Parse(values[0]);
                    motion.x = float.Parse(values[1]);
                    motion.y = float.Parse(values[2]);
                    motion.z = float.Parse(values[3]);
                    motion.qx = float.Parse(values[4]);
                    motion.qy = float.Parse(values[5]);
                    motion.qz = float.Parse(values[6]);
                    motion.qw = float.Parse(values[7]);
                    data.Add(motion);
                }
                else
                {
                    Debug.LogError("Invalid data in the .csv file at line " + (i + 1));
                }
            }
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
        return data;
    }
}

public class HeadMotionData
{
    public float time;
    public float x;
    public float y;
    public float z;
    public float qx;
    public float qy;
    public float qz;
    public float qw;
}



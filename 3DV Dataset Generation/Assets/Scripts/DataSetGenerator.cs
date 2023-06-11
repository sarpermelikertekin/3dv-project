using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataSetGenerator : MonoBehaviour
{
    CaptureScreenshot captureScreenshot;
    RandomLighting randomLighting;
    RandomBackground randomBackground;
    RandomRotation randomRotation;
    RandomTranslator randomTranslator;
    RandomScalor randomScalor;


    public GameObject[] implants;
    public List<GameObject> active_implants;
    public GameObject activeImplant;
    public GameObject boundingBox;
    
    public string implantData;
    public string outputFileName;

    public int numberOfImages;
    public float dataPointWaitTime;
    public int counter;

    // Start is called before the first frame update
    void Start()
    {
        captureScreenshot = gameObject.GetComponent<CaptureScreenshot>();
        randomLighting = gameObject.GetComponent<RandomLighting>();
        randomBackground = gameObject.GetComponent<RandomBackground>();
        randomRotation = gameObject.GetComponent<RandomRotation>();
        randomTranslator = gameObject.GetComponent<RandomTranslator>();
        randomScalor = gameObject.GetComponent<RandomScalor>();

        implantData = "Index, Position, Quaternion, EulerAngles, Scale, Label" + "\n";
        outputFileName = "imagefile.txt";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(StartGeneratingDataSet(dataPointWaitTime));
        }
    }

    public void ToggleActiveImplants()
    {
        for (int i = active_implants.Count-1; i >= 0; i--)
        {
            active_implants[i].SetActive(false);
            active_implants.RemoveAt(i);
        }

        int rand_no_implants = Random.Range(0, 5);
        if(rand_no_implants < 1) //no objects with 0.2 probability
            return;


        for (int i = 0; i < 5; i++)
        {
            int rand_index = Random.Range(0, implants.Length+1);
            if (rand_index >= implants.Length)
            {
                continue;
            }
            activeImplant = implants[rand_index];
            if (activeImplant.activeSelf)
            {
                continue;
            }
            activeImplant.SetActive(true);
            active_implants.Add(activeImplant);
        }
    }

    public void PopulateOutputFile(float scale)
    {
        implantData += captureScreenshot.pictureIndex.ToString();
        implantData += (", " + activeImplant.transform.position.ToString());
        implantData += (", " + activeImplant.transform.rotation.ToString());
        implantData += (", " + activeImplant.transform.eulerAngles.ToString());
        implantData += (", " + scale.ToString());
        implantData += (", " + activeImplant.transform.name + "\n");
    }

    public void WriteToFile(string fileName, string textToWrite)
    {
        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllText(path, textToWrite);
    }

    public void GeneratePicture(bool isBoundingBoxActive, string path)
    {
        boundingBox.SetActive(isBoundingBoxActive);

        captureScreenshot.TakeScreenshot(path);
        
    }

    public IEnumerator StartGeneratingDataSet(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ToggleActiveImplants();
        for (int i = 0; i<active_implants.Count; i++)
        {
            activeImplant = active_implants[i];
            float greyTone = (float)Random.Range(0.2f, 0.9f);
            activeImplant.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = new Color(greyTone, greyTone, greyTone);
            randomRotation.RotateRandomly(activeImplant);
            randomLighting.PutTheLightInARandomPosition();
            randomBackground.ChangeBackgroundRandomly();
            randomTranslator.TranslateRandomly(activeImplant);
            float scaleFactor = (float)Random.Range(0.6f, 3f);
            randomScalor.ScaleRandomly(scaleFactor, activeImplant);
            PopulateOutputFile(scaleFactor);
        }

        Debug.Log(captureScreenshot.pictureIndex);
        Debug.Log(active_implants.Count);

        //GeneratePicture(true, "Assets/Data Points with Bounding Box/");
        //yield return new WaitForSeconds(seconds);
        GeneratePicture(false, "Assets/Data Points/");
        ++captureScreenshot.pictureIndex;
        

        if (captureScreenshot.pictureIndex < numberOfImages)
        {
            StartCoroutine(StartGeneratingDataSet(seconds));
        }
        else
        {
            Debug.Log(implantData);
            WriteToFile(outputFileName, implantData);
        }
    }
}

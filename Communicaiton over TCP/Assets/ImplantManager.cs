using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImplantManager : MonoBehaviour
{
    public GameObject med;
    public GameObject lat;

    public Material highlightedColor;
    public Material implantColor;

    public List<string> implantNames;
    public string[] buffer1;
    public string[] buffer2;
    public string input;

    // Start is called before the first frame update
    void Start()
    {
        implantNames = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        med.GetComponent<MeshRenderer>().material = implantColor;
        lat.GetComponent<MeshRenderer>().material = implantColor;

        buffer1 = input.Split(";");
        Debug.Log(buffer1);
        for (int i = 0; i < buffer1.Length - 1; i++)
        {
            buffer2 = buffer1[i].Split(",");
            implantNames.Add(buffer2[buffer2.Length - 1]);
            Debug.Log(implantNames);
        }

        if (implantNames.Contains("FracturePlate_1x_MedScrewCut1"))
        {
            med.GetComponent<MeshRenderer>().material = highlightedColor;
        }
        if (implantNames.Contains("FracturePlate_1x_LatScrewCut1"))
        {
            lat.GetComponent<MeshRenderer>().material = highlightedColor;
        }

        implantNames.RemoveRange(0, implantNames.Count);
    }
}

//0,79.53459930419922,293.3654479980469,206.3458709716797,417.59722900390625,0.5361877679824829,0,FracturePlate_1x_MedScrewCut1; 
//1,546.314453125,274.79656982421875,695.4112548828125,415.70684814453125,0.5298264026641846,1,FracturePlate_1x_LatScrewCut1;
//2,229.38258361816406,230.24192810058594,347.0044250488281,351.0977783203125,0.41528844833374023,3,LatInstrument;
//3,399.6430969238281,175.31700134277344,550.707275390625,325.06488037109375,0.3667522370815277,0,FracturePlate_1x_MedScrewCut1;
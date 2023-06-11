using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomScalor : MonoBehaviour
{

    public GameObject objectToBeScaled;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ScaleRandomly(float scaleFactor, GameObject activeImplant)
    {
        activeImplant.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        Debug.Log("Scaled");
    }
    
}

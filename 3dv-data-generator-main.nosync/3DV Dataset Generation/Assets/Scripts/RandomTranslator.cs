using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTranslator : MonoBehaviour
{
    public GameObject objectToBeTranslated;
    public float xBoundary;
    public float yBoundary;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TranslateRandomly(GameObject activeImplant)
    {
        //objectToBeTranslated.transform.position = new Vector3(Random.Range(-xBoundary, xBoundary), Random.Range(-yBoundary, yBoundary), objectToBeTranslated.transform.position.z);
        activeImplant.transform.position = new Vector3(Random.Range(-xBoundary, xBoundary), Random.Range(-yBoundary, yBoundary), objectToBeTranslated.transform.position.z);
        Debug.Log("Rotated");
    }
}

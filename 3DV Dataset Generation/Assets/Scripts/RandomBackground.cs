using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomBackground : MonoBehaviour
{

    public Texture[] textures;
    public GameObject background;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeBackgroundRandomly()
    {
        background.GetComponent<RawImage>().texture = textures[Random.Range(0, textures.Length)];
        Debug.Log("Background Changed");
    }
}

using System.Collections;
using UnityEngine;

public class RandomLighting : MonoBehaviour
{
    public GameObject centerObject; // The object to rotate around
    public GameObject rotatingObject;
    public float speed = 1.0f; // The speed of rotation

    private Vector3 direction; // The direction vector to rotate towards

    void Update()
    {

    }

    public void PutTheLightInARandomPosition()
    {
        direction = Random.onUnitSphere;

        // Rotate the object towards the random direction vector
        rotatingObject.transform.rotation = Quaternion.RotateTowards(rotatingObject.transform.rotation, Quaternion.LookRotation(direction, centerObject.transform.up), speed * Time.deltaTime);

        // Move the object to the position on the sphere
        rotatingObject.transform.position = centerObject.transform.position + (direction * 2.0f); // Multiply by radius or distance from center object

        Debug.Log("Light is in a Different Position");
    }
}
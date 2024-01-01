using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    public CarContoler theCar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            theCar.CheckpointHit(other.GetComponent<CheckPoints>().cpNumber);
        }
    }

}

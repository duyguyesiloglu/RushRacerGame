using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public int groundLayerNo = 8;
    public AudioSource soundtoPlay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.layer != groundLayerNo)
        {
            soundtoPlay.Stop();
            soundtoPlay.pitch = Random.Range(0.8f, 1.2f);

            soundtoPlay.Play();
        }

        
    }
}

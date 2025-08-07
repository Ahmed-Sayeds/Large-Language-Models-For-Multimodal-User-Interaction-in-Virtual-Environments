using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class music : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    void Start()
    {
        //audioSource.Play();
        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = true;
    }

    private void Update()
    {
       
    }

}


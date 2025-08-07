using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeController : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioGPT audioGPT;
    void Start()
    {

        

    }
    
    // Update is called once per frame
    void Update()
    {

    }

    private float scaleValue = 0.2f;
    public void ResizeObjectX(float value)
    {
        if (audioGPT.SelectedGameObject != null) { audioGPT.SelectedGameObject.transform.localScale = new Vector3(value * scaleValue, audioGPT.SelectedGameObject.transform.localScale.y, audioGPT.SelectedGameObject.transform.localScale.z); }
    }
    public void ResizeObjectY(float value)
    {
        if (audioGPT.SelectedGameObject != null) { audioGPT.SelectedGameObject.transform.localScale = new Vector3(audioGPT.SelectedGameObject.transform.localScale.x, value * scaleValue, audioGPT.SelectedGameObject.transform.localScale.z); }
    }
    public void ResizeObjectZ(float value)
    {
        if (audioGPT.SelectedGameObject != null) { audioGPT.SelectedGameObject.transform.localScale = new Vector3(audioGPT.SelectedGameObject.transform.localScale.x, audioGPT.SelectedGameObject.transform.localScale.y, value * scaleValue); }
    }

    public void ScaleValue(float value)
    {
        scaleValue = value;
    }
}

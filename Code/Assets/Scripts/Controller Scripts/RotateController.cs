using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RotateController : MonoBehaviour
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

    public void RotateObjectX(float value)
    {
        if (audioGPT.SelectedGameObject != null) { audioGPT.SelectedGameObject.transform.eulerAngles = new Vector3(value, audioGPT.SelectedGameObject.transform.eulerAngles.y, audioGPT.SelectedGameObject.transform.eulerAngles.z); }
    }
    public void RotateObjectY(float value)
    {
        if (audioGPT.SelectedGameObject != null) { audioGPT.SelectedGameObject.transform.eulerAngles = new Vector3(audioGPT.SelectedGameObject.transform.eulerAngles.x, value, audioGPT.SelectedGameObject.transform.eulerAngles.z); }
    }
    public void RotateObjectZ(float value)
    {
        if (audioGPT.SelectedGameObject != null) { audioGPT.SelectedGameObject.transform.eulerAngles = new Vector3(audioGPT.SelectedGameObject.transform.eulerAngles.x, audioGPT.SelectedGameObject.transform.eulerAngles.y, value); }
    }

    public void XRotationLock(bool value)
    {
        audioGPT.xRotationLock = value;
    }

    public void YRotationLock(bool value)
    {
        audioGPT.yRotationLock = value;
    }

    public void ZRotationLock(bool value)
    {
        audioGPT.zRotationLock = value;
    }


}

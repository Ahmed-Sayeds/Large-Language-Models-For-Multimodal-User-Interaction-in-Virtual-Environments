using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;
using TMPro;
using UnityEngine.UI;
public class MoveObjectUsingAxisController : MonoBehaviour
{
    public AudioGPT audioGPT;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private float scaleValue = 0.2f;
    public void ScaleValue(float value)
    {
        scaleValue = value;
    }
    public void OnButtonClicked(Button buttonClicked)
    {
        if (audioGPT.SelectedGameObject != null)
        {
            Vector3 currentPosition = Vector3.zero;
            switch (buttonClicked.gameObject.name)
            {
                case "- X":
                    currentPosition = audioGPT.SelectedGameObject.transform.position;
                    currentPosition.x -= scaleValue;
                    audioGPT.SelectedGameObject.transform.position = currentPosition;
                    break;

                case "+ X":
                    currentPosition = audioGPT.SelectedGameObject.transform.position;
                    currentPosition.x += scaleValue;
                    audioGPT.SelectedGameObject.transform.position = currentPosition;
                    break;

                case "- Y":
                    currentPosition = audioGPT.SelectedGameObject.transform.position;
                    currentPosition.y -= scaleValue;
                    audioGPT.SelectedGameObject.transform.position = currentPosition;
                    break;

                case "+ Y":
                    currentPosition = audioGPT.SelectedGameObject.transform.position;
                    currentPosition.y += scaleValue;
                    audioGPT.SelectedGameObject.transform.position = currentPosition;
                    break;

                case "- Z":
                    currentPosition = audioGPT.SelectedGameObject.transform.position;
                    currentPosition.z -= scaleValue;
                    audioGPT.SelectedGameObject.transform.position = currentPosition;
                    break;

                case "+ Z":
                    currentPosition = audioGPT.SelectedGameObject.transform.position;
                    currentPosition.z += scaleValue;
                    audioGPT.SelectedGameObject.transform.position = currentPosition;
                    break;

            }
        }
    }



}

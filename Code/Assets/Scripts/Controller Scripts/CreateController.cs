using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class CreateController : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioGPT audioGPT;

    private GameObject[] prefabs;
    public TMP_Dropdown dropDown;
    void Start()
    {


        //prefabsList.onValueChanged.AddListener(OnDropdownValueChanged);
        prefabs = Resources.LoadAll<GameObject>("Prefabs/");

        dropDown.ClearOptions();
        // Loop through the loaded prefabs
        TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData("");
        dropDown.options.Add(option);
        foreach (GameObject prefab in prefabs)
        {
            option = new TMP_Dropdown.OptionData(prefab.name);
            // Access the name of each prefab
            dropDown.options.Add(option);
        }
        dropDown.RefreshShownValue();

    }

    public void OnDropdownValueChanged(int index)
    {

        Debug.Log("value changed    "+ dropDown.options[dropDown.value].text);
        // Add more conditions for other options as needed
        try
        {
            audioGPT.SelectedGameObject = Instantiate(Resources.Load<GameObject>("Prefabs/" + dropDown.options[dropDown.value].text));
        }
        catch (Exception e) { Debug.Log("Creation Exception:" + e); }
    }

    // Update is called once per frame
    public void OnButtonClicked(Button buttonClicked)
    {
        switch (buttonClicked.gameObject.name)
        {
            case "Sphere":
                audioGPT.SelectedGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;

            case "Cube":
                audioGPT.SelectedGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;

            case "Capsule":
                audioGPT.SelectedGameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                break;

            case "Cylinder":
                audioGPT.SelectedGameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                break;

            case "Plane":
                audioGPT.SelectedGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                break;

            case "Quad":
                audioGPT.SelectedGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                break;

        }
    }
}

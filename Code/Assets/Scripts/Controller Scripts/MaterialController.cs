using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    public AudioGPT audioGPT;

    private Material[] materials;
    public TMP_Dropdown dropDown;
    void Start()
    {

        materials = Resources.LoadAll<Material>("Materials/");

        dropDown.ClearOptions();
        // Loop through the loaded prefabs
        TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData("");
        dropDown.options.Add(option);
        foreach (Material material in materials)
        {
            option = new TMP_Dropdown.OptionData(material.name);
            // Access the name of each prefab
            dropDown.options.Add(option);

        }
        dropDown.RefreshShownValue();


    }

    public void OnDropdownValueChanged(int index)
    {

        Debug.Log("value changed    " + index);
        // Add more conditions for other options as needed
        try
        {
            audioGPT.oneObjectAppendingToUseForUndo();
            if (audioGPT.SelectedGameObject == null)
            {
                Debug.Log("Selected game object is null    " + index);
            }
            //Material myMaterial = Resources.Load<Material>("Materials/" + dropDown.options[dropDown.value].text);
            audioGPT.SelectedGameObject.GetComponent<Renderer>().material = materials[index-1];
        }
        catch (Exception e) { Debug.Log("Creation Exception:" + e); }
    }
}

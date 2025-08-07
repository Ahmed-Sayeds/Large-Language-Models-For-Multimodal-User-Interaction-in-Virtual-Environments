using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ScriptController : MonoBehaviour
{
    public AudioGPT audioGPT;

    private MonoScript[] scripts;
    public TMP_Dropdown dropDown;
    void Start()
    {

        scripts = Resources.LoadAll<MonoScript>("Scripts/");

        dropDown.ClearOptions();
        // Loop through the loaded prefabs
        TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData("");
        dropDown.options.Add(option);
        foreach (MonoScript script in scripts)
        {
            option = new TMP_Dropdown.OptionData(script.name);
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
            audioGPT.SelectedGameObject.AddComponent(scripts[index - 1].GetClass());
        }
        catch (Exception e) { Debug.Log("Creation Exception:" + e); }
    }
}

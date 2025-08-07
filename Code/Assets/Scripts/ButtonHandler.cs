using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using System;
using System.IO;

public class ButtonHandler : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject rotateMenu;
    public GameObject resizeMenu;
    public GameObject createMenu;
    public GameObject materialMenu;
    public GameObject scriptMenu;
    public GameObject ComponentMenu;
    public GameObject MoveObjectUsingAxisMenu;

    public AudioGPT audioGPT;


    private string path = "C:\\Users\\student\\Desktop\\research\\8.txt";


    public string commandsLogString = "";


    private void Start()
    {
        MainMenuActivate();
    }
    private void Update()
    {
        //EyeGaze cameraEyeGazeScript = cameraObject.GetComponent<EyeGaze>();
    }

    private String returnDateNowString()
    {
        string currentTime = DateTime.Now.ToString("HH:mm:ss");
        String S = currentTime.ToString();
        S += ",";
        return S;
    }


    public void OnButtonClicked(Button buttonClicked)
    {
        commandsLogString = returnDateNowString();

        switch (buttonClicked.gameObject.name)
        {

            case "Select Object":
                commandsLogString += "Menu,Select Object," + Environment.NewLine;
                audioGPT.controlSelectionMode = 0;
                break;

            case "Rotate":
                commandsLogString += "Menu,Rotate," + Environment.NewLine;
                otherMenuActivate(rotateMenu);
                break;

            case "Resize":
                commandsLogString += "Menu,Resize," + Environment.NewLine;
                otherMenuActivate(resizeMenu);
                break;

            case "Create":
                commandsLogString += "Menu,Create," + Environment.NewLine;
                otherMenuActivate(createMenu);
                break;

            case "Delete":
                commandsLogString += "Menu,Delete," + Environment.NewLine;
                Destroy(audioGPT.SelectedGameObject);
                break;

            case "Material":
                commandsLogString += "Menu,Material," + Environment.NewLine;
                otherMenuActivate(materialMenu);
                break;

            case "Script":
                commandsLogString += "Menu,Select Object," + Environment.NewLine;
                otherMenuActivate(scriptMenu);
                break;

            case "Component":
                commandsLogString += "Menu,Component," + Environment.NewLine;
                otherMenuActivate(ComponentMenu);
                break;

            case "Moving Object":
                commandsLogString += "Menu,Moving Object," + Environment.NewLine;
                audioGPT.controlSelectionMode = 1;
                audioGPT.oneObjectAppendingToUseForUndo();
                break;

            case "Moving Player":
                commandsLogString += "Menu,Moving Player," + Environment.NewLine;
                audioGPT.controlSelectionMode = 2;

                break;

            case "Teleport Player":
                commandsLogString += "Menu,Teleport Player," + Environment.NewLine;
                audioGPT.controlSelectionMode = 3;
                break;

            case "Undo":
                commandsLogString += "Menu,Undo," + Environment.NewLine;
                audioGPT.UndoFunction();
                break;

            case "Move Object Using Axis":
                commandsLogString += "Menu,Move Object Using Axis," + Environment.NewLine;
                otherMenuActivate(MoveObjectUsingAxisMenu);
                break;

            case "Home":
                commandsLogString += "Menu,Home," + Environment.NewLine;
                MainMenuActivate();
                break;

            
        }

        File.AppendAllText(path, commandsLogString);

    }
    public void MainMenuActivate()
    {
        rotateMenu.SetActive(false);
        resizeMenu.SetActive(false);
        createMenu.SetActive(false);
        materialMenu.SetActive(false);
        scriptMenu.SetActive(false);
        ComponentMenu.SetActive(false);
        MoveObjectUsingAxisMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void otherMenuActivate(GameObject otherMenu)
    {
        mainMenu.SetActive(false);

        otherMenu.SetActive(true);
    }

}

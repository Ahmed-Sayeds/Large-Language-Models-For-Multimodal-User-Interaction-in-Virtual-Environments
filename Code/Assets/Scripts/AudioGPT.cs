using Assets.Scripts;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Debug = UnityEngine.Debug;

public class AudioGPT : MonoBehaviour
{
    private OpenAIApi openai = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();
    /*    private string initializationPrompt = @"
    You will provide me with a segment of natural language text describing actions and manipulations within a virtual environment. your task will be to analyze this text and perform the following:
    Class: 
    You will identify the objective of the text. Using the provided list, You will determine which item best matches the objective, and return the index of that item as a string. The list is as follows:
    {
    1 - Rotate an object.
    2 - Resize an object.
    3 - Create an object.
    4 - Delete an object.
    5 - Select an object.
    6 - Add material to an object.
    7 - Add a script to an object.
    8 - Add a component to an object, such as a rigidbody.
    9 - Move an object.
    10 - Move to a location.
    11 - Teleport to a location.
    }

    Object: You will extract any mentioned objects from the text. These objects can be referred to with single or multi-word names, or non-specific pronouns like 'that', within the virtual environment context.
    You will return a list of these object names.Values: You will extract any values related to an object. 
    If the text contains one or more vector3 values, You will return these as a sublist within the values list, if it is not vector3 return values as a sublist with a single value in values list. If any values in a vector3 coordinate are missing, You will fill in those values with an empty string ''. 
    Non-vector values or the word 'there' or any similar terms will be added directly to the values list. If the text contains words like 'greater', 'smaller', 'multiply', 'divide', or 'power', You will convert them to '+', '-', '*', '/', and '^' respectively and place the symbol before the value.
    You will return these as a list of strings or list of lists. You will compile the results into a JSON format and return it. The returned JSON should have the structure:jsonCopy code{'index': '<index>','objects': ['<object1>', '<object2>', ...],'values': [['<value1>', '<value2>', '<value3>'], ..., '[<valueN>]'],}
    If the text does not contain any objects or values, You will return empty lists in the respective places.";*/

    private string initializationPrompt = @"
Your role is to interpret the input text that describes actions in a virtual reality environment.

If your text contains multiple instructions, each one will be identified and processed separately. This leads to individual 'command' sections in the returned JSON output.

You need to:

a - Determine the objective of the text and assign the corresponding index from the list provided.
[
1 - Rotating an object. Which is changing the angle of rotation.
2 - Resizing an object. Which is changing how boy it is.
3 - Creating an object.
4 - Deleting, Destroying, Removing an object.
5 - Selecting an object.
6 - Adding material to an object.
7 - Adding a script to an object.
8 - Adding a component to an object, such as a rigidbody.
9 - The action of moving an object to a specific location.
10 - Moving myself which is the player to a location.
11 - Teleporting myself which is the player to a location.
12 - Undo, which reverses that last acion done by user.
]

b- Extract any objects mentioned in the text and return them in a list. If no objects are mentioned return an empty list.

c- Extract any values related to an object. For three-dimensional objects like cubes, if an operation affects its size, ensure the operation is applied to all three dimensions. If any values in a vector3 coordinate are missing, I will fill in those values with an empty string """". Non-vector values or the word ""there"" or any similar terms will be added directly to the values list. If the text contains words like ""greater"", ""smaller"", ""multiply"", ""divide"", or ""power"", I will convert them to ""+"", ""_"", ""*"", ""/"", and ""^"" respectively and place the symbol before the value. I will return these as a list of strings or list of lists.

d- Returned results in a JSON structure like this:


VERY IMPORTANT: Do not add any objects that were not mentioned in the prompt text I gave to you.

{
""command"": [{
""indexes"": ""<index>"",
""objects"": [""<object1>"", ""<object2>"", ...],
""values"": [""<value1>"", ""<value2>"", ""<value3>"", ...],
}]}

Feedback will be provided for unclear or ambiguous instructions.

Disregard English stopwords during the process. Handle conflicting commands by returning separate commands for each one. Treat ambiguous terms, which could be either an object or an action, as separate commands.

If the reply doesn't contain an index number ask the user for more clear instructions and guide him through it.

The following are examples:

Question:
select that and move it here

Answer:

{
""command"": [{
""indexes"": ""5"",
""objects"": [""that""],
""values"": []
},
{
""indexes"": ""9"",
""objects"": [""it""],
""values"": [""here""]
}]

Question:
Move to there

Answer:
{
""command"": [{
""indexes"": ""10"",
""objects"": [""""],
""values"": [""there""]
}]
}

Question:
create cube here

Answer:

{
""command"": [{
""indexes"": ""3"",
""objects"": [""cube""],
""values"": []
},
{
""indexes"": ""9"",
""objects"": [""""],
""values"": [""here""]
}]

Question:
Move to there

Answer:
{
""command"": [{
""indexes"": ""10"",
""objects"": [""""],
""values"": [""there""]
}]
}


Question:
Make the cube 5 times bigger

Answer:
{
""command"": [{
""indexes"": ""2"",
""objects"": [""it""],
""values"": [""*5"",""*5"",""*5""]
}]
}



Question:
Decrease height by 5 units

Answer:
{
  ""command"": [{
    ""indexes"": ""2"",
    ""objects"": [],
    ""values"": ["""",""_5"",""""]
  }]
}




Question:
Increase height by 5 units

Answer:
{
  ""command"": [{
    ""indexes"": ""2"",
    ""objects"": [],
    ""values"": ["""",""+5"",""""]
  }]
}

Reason:
The Y axis maps to the height of an object in vector3 and to the right and left rotation.


Question:
Move it forward by 10 units

Answer:
{
  ""command"": [{
    ""indexes"": ""9"",
    ""objects"": [""it""],
    ""values"": ["""","""",""+10""]
  }]
}

Reason:
The z axis maps to forward and backward in vector3 and to up and down in rotation.


Question:
Move it up by 5 units

Answer:
{
  ""command"": [{
    ""indexes"": ""9"",
    ""objects"": [""it""],
    ""values"": ["""",""+5"",""""]
  }]
}

Reason:
The Y axis maps to up and down in vector3.


Question:
move there

Answer:
{
  ""command"": [{
    ""indexes"": ""10"",
    ""objects"": [],
    ""values"": [""there""]
  }]
}


Question:
move it here

Answer:
{
  ""command"": [{
    ""indexes"": ""9"",
    ""objects"": [""it""],
    ""values"": [""here""]
  }]
}

Question:
apply material brick

Answer:
{
  ""command"": [{
    ""indexes"": ""6"",
    ""objects"": [""""],
    ""values"": [""brick""]
  }]
}



";

    /*"
    Your role is to interpret the input text that describes actions in a virtual reality environment. The actions can include:

    1 - Rotating an object
    2 - Resizing an object
    3 - Creating an object
    4 - Deleting an object
    5 - Selecting an object
    6 - Adding material to an object
    7 - Adding a script to an object
    8 - Adding a component to an object, such as a rigidbody
    9 - Moving an object
    10 - Moving to a location
    11 - Teleporting to a location

    If your text contains multiple instructions, each one will be identified and processed separately. This leads to individual 'command' sections in the returned JSON output.

    You need to:

    Determine the objective of the text and assign the corresponding index from the list.
    Extract any objects mentioned in the text and return them in a list.
    Extract any values related to an object. For three-dimensional objects like cubes, if an operation affects its size, ensure the operation is applied to all three dimensions. If any values in a vector3 coordinate are missing, I will fill in those values with an empty string """". Non-vector values or the word ""there"" or any similar terms will be added directly to the values list. If the text contains words like ""greater"", ""smaller"", ""multiply"", ""divide"", or ""power"", I will convert them to ""+"", ""-"", ""*"", ""/"", and ""^"" respectively and place the symbol before the value. I will return these as a list of strings or list of lists.
    The results should be compiled and returned in a JSON structure like this:

    {
    ""command"": [{
    ""indexes"": ""<index>"",
    ""objects"": [""<object1>"", ""<object2>"", ...],
    ""values"": [""<value1>"", ""<value2>"", ""<value3>"", ...],
    }]}

    Feedback will be provided for unclear or ambiguous instructions.

    Disregard English stopwords during the process. Handle conflicting commands by returning separate commands for each one. Treat ambiguous terms, which could be either an object or an action, as separate commands.



    The following are examples:

    Question:
    Make the cube 5 times bigger.

    Answer:
    {
    ""command"": [{
    ""indexes"": ""2"",
    ""objects"": [""it""],
    ""values"": [""*5"",""*5"",""*5""]
    }]
    }

    Question:
    Make the cube 5 times bigger.

    Answer:
    {
      ""command"": [{
        ""indexes"": ""10"",
        ""objects"": [""""],
        ""values"": [""there""]
      }]
    }


    Question:
    Increase height by 5 units.

    Answer:
    {
      ""command"": [{
        ""indexes"": ""10"",
        ""objects"": [""""],
        ""values"": ["""",""+5"",""""]
      }]
    }

    Reason:
    The Y axis maps to the height of an object in vector 3 and to the right and left rotation.
    ";*/
    /* @"
   You will provide me with a segment of natural language text describing actions and manipulations within a virtual environment. Your task will be to analyze this text and perform the following:

   Class: 
   You will identify the objective of the text. Using the provided list, you will determine which item best matches the objective and return the index of that item as a string. The list is as follows:
   {
   1 - Rotate an object.
   2 - Resize an object.
   3 - Create an object.
   4 - Delete an object.
   5 - Select an object.
   6 - Add material to an object.
   7 - Add a script to an object.
   8 - Add a component to an object, such as a rigidbody.
   9 - Move an object.
   10 - Move to a location.
   11 - Teleport to a location.
   }

   Object: 
   You will extract any mentioned objects from the text. These objects can be referred to with single or multi-word names or non-specific pronouns like 'that' or 'it', within the virtual environment context. You will return a list of these object names.

   Values: 
   You will extract any values related to an object. If the text contains one or more vector3 values, you will return these as a sublist within the values list. If a value is not a vector3 but is intended to modify a 3D object (such as 'make cube 5 times bigger'), it should be assumed that this value applies to all dimensions of the object and be listed three times in the sublist (e.g., ['*5', '*5', '*5']). If any values in a vector3 coordinate are missing, you will fill in those values with an empty string ''...

   Please provide me with a segment of natural language text describing an action or manipulation within a virtual environment, and I will generate the corresponding JSON output based on the prompt.

   Example:
   Text: 'Move it there.'
   Output:
   {
     'index': '10',
     'objects': ['it'],
     'values': [['there']]
   }
   ";*/
    public Material defaultMaterial= null;

    public string microphoneName;
    private AudioClip microphoneClip;
    private readonly string fileName = "output.wav";

    public string commandsLogString = "";
    private string GazeVoice = "";

    /****************************************************************************************************************************************************************************************************************/
    //public GameObject SelectedGameObject;
    public event Action<GameObject> VariableUpdated;
    public GameObject myObject;

    public TMP_InputField chatGPT_ReplyPanel;

    private List<List<List<UnityEngine.Component>>> objectsComponents = new List<List<List<UnityEngine.Component>>>();
    private List<List<GameObject>> objectsReference = new List<List<GameObject>>();
    private List<List<GameObject>> objectsCopyReference = new List<List<GameObject>>();

    public GameObject SelectedGameObject
    {
        get { return myObject; }
        set
        {
            if (value != theGround && value != theMenuBack)
            {
                if (SelectedGameObject != null && value != SelectedGameObject)
                {
                    Outline scriptComponent = SelectedGameObject.GetComponent<Outline>();

                    // Check if the script component exists
                    if (scriptComponent != null)
                    {
                        // Option 1: Destroy the script component
                        Destroy(scriptComponent);

                        // Option 2: Remove the script component
                        // RemoveComponent(scriptComponent);
                    }
                }
                if (SelectedGameObject != value)
                {

                    myObject = value;
                    VariableUpdated?.Invoke(myObject);
                    SelectedGameObjectChanged(myObject);
                }
            }
        
        }
    }

    private void SelectedGameObjectChanged(GameObject updatedGameObject)
    {
        // You can now use updatedGameObject here
        Debug.Log("This is from AudioGPT script   " + updatedGameObject.name);
        var outline = SelectedGameObject.AddComponent<Outline>();

        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.cyan;
        outline.OutlineWidth = 5f;
    }



    private Stopwatch stopwatch;



    public GameObject theCube;
    public GameObject theGround;
    public GameObject theMenuBack;

    public GameObject playerObject;
    public GameObject cameraObject;

    public int mode = -1;

    public GameObject rightHand_Controller;
    public GameObject LeftHand_Controller;

    public int controlSelectionMode= 0;

    public int controllerGesture = 0;

    private Vector3 differenceVector;
    private Vector3 initialVector;

    public InputAction rightHandControllerGripAction;
    public InputAction rightHandControllerTriggerAction;

    public InputAction leftHandControllerGripAction;
    public InputAction leftHandControllerTriggerAction;

    public InputAction rightHandControllerTrackpadClickedAction;

    public InputAction leftHandControllerTrackpadClickedAction;


    private bool doNotTriggerObjectMovementFlag = false;

    public bool xRotationLock = false;
    public bool yRotationLock = false;
    public bool zRotationLock = false;


    private string path = "C:\\Users\\student\\Desktop\\research\\8.txt";
    /****************************************************************************************************************************************************/
    void Start()
    {
        /*AudioGPT audioGPT = this;
        audioGPT.VariableUpdated += SelectedGameObjectChanged;*/

        microphoneName = Microphone.devices[0];
        stopwatch = new Stopwatch();

        SelectedGameObject = undoObjectPlaceHolder;
        
        //ExtractValues("move it back by 5 units");

        /*foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            Debug.Log("wwwwww"+obj.name);
        }*/

        rightHandControllerGripAction.started += OnRightHandControllerGripAction;
        rightHandControllerGripAction.performed += OnRightHandControllerGripAction;
        rightHandControllerGripAction.canceled += OnRightHandControllerGripAction;
        rightHandControllerGripAction.Enable();

        rightHandControllerTriggerAction.started += OnRightHandControllerTriggerAction;
        rightHandControllerTriggerAction.performed += OnRightHandControllerTriggerAction;
        rightHandControllerTriggerAction.canceled += OnRightHandControllerTriggerAction;
        rightHandControllerTriggerAction.Enable();

        rightHandControllerTrackpadClickedAction.started += OnRightHandTrackpadClickAction;
        rightHandControllerTrackpadClickedAction.performed += OnRightHandTrackpadClickAction;
        rightHandControllerTrackpadClickedAction.canceled += OnRightHandTrackpadClickAction;
        rightHandControllerTrackpadClickedAction.Enable();


        leftHandControllerGripAction.started += OnLeftHandControllerGripAction;
        leftHandControllerGripAction.performed += OnLeftHandControllerGripAction;
        leftHandControllerGripAction.canceled += OnLeftHandControllerGripAction;
        leftHandControllerGripAction.Enable();

        leftHandControllerTriggerAction.started += OnLeftHandControllerTriggerAction;
        leftHandControllerTriggerAction.performed += OnLeftHandControllerTriggerAction;
        leftHandControllerTriggerAction.canceled += OnLeftHandControllerTriggerAction;
        leftHandControllerTriggerAction.Enable();

        leftHandControllerTrackpadClickedAction.started += OnLeftHandTrackpadClickAction;
        leftHandControllerTrackpadClickedAction.performed += OnLeftHandTrackpadClickAction;
        leftHandControllerTrackpadClickedAction.canceled += OnLeftHandTrackpadClickAction;
        leftHandControllerTrackpadClickedAction.Enable();



        //ExtractValues("make it 3 times bigger");
        //rightHandControllerTriggerActionFlag = true;
        //controlSelectionMode = 0;
    }

    private String returnDateNowString()
    {
        string currentTime = DateTime.Now.ToString("HH:mm:ss");
        String S = currentTime.ToString();
        S += ",";
        return S;
    }
    private void OnRightHandTrackpadClickAction(InputAction.CallbackContext context)
    {
        // Your event handling logic here
        if (context.started)
        {
            
            GazeVoice = "GestureVoice,";
            // Grip action performed
            MicrophoneToAudioClip();
            mode = 1;
        }
        else if (context.performed)
        {
            // Grip action performed

        }
        else if (context.canceled)
        {
            // Grip action canceled
            SaveAudioClipAndGenerateText();
        }
    }

    //private bool leftHandControllerTrackpadClickedActionFlag = false;
    private void OnLeftHandTrackpadClickAction(InputAction.CallbackContext context)
    {
        // Your event handling logic here
        if (context.started)
        {
            // Grip action performed

            GazeVoice = "GazeVoice,";
            MicrophoneToAudioClip();
            mode = 0;
            //leftHandControllerTrackpadClickedActionFlag = true;
            playerObject.GetComponent<ActionBasedContinuousMoveProvider>().enabled = false;
        }
        else if (context.performed)
        {
            // Grip action performed

        }
        else if (context.canceled)
        {
            // Grip action canceled
            playerObject.GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;

            //leftHandControllerTrackpadClickedActionFlag = false;
            SaveAudioClipAndGenerateText();

        }
    }



    bool  rightHandControllerGripActionFlag = false;
    private GameObject miniGameObject = null;
    public GameObject rightHandModel;
    private void OnRightHandControllerGripAction(InputAction.CallbackContext context)
    {
        // Your event handling logic here
        if (context.started)
        {
            // Grip action performed
            commandsLogString = returnDateNowString();
            commandsLogString += "GestureRotation," + Environment.NewLine;
            File.AppendAllText(path, commandsLogString);
            rightHandControllerGripActionFlag = true;
            
            oneObjectAppendingToUseForUndo();
            controllerGesture = 1;
            miniGameObject = Instantiate(SelectedGameObject);
            float max = Math.Max(Math.Max(miniGameObject.GetComponent<Renderer>().bounds.size.x, miniGameObject.GetComponent<Renderer>().bounds.size.y), miniGameObject.GetComponent<Renderer>().bounds.size.z);
            miniGameObject.transform.localScale *= (0.2f / max);
            miniGameObject.transform.SetParent(rightHand_Controller.transform);
            miniGameObject.transform.position = rightHandModel.transform.position;
            rightHandModel.SetActive(false);


            //differenceVector = LeftHand_Controller.transform.InverseTransformPoint(rightHand_Controller.transform.position);
            differenceVector = LeftHand_Controller.transform.position - rightHand_Controller.transform.position;
            initialVector = SelectedGameObject.transform.eulerAngles;
            previousRotation = Vector3.zero;
            
        }
        else if (context.performed)
        {
            // Grip action performed
            Debug.Log("Right hand Grip Started");

        }
        else if (context.canceled)
        {
            // Grip action canceled
            rightHandControllerGripActionFlag = false;
            Destroy(miniGameObject);
            controllerGesture = 0;
            rightHandModel.SetActive(true);
        }
    }


    bool  leftHandControllerGripActionFlag = false;
    private void OnLeftHandControllerGripAction(InputAction.CallbackContext context)
    {
        // Your event handling logic here
        if (context.started)
        {
            commandsLogString = returnDateNowString();
            commandsLogString += "GestureResize," + Environment.NewLine;
            File.AppendAllText(path, commandsLogString);
            // Grip action performed
            leftHandControllerGripActionFlag = true;
            
            oneObjectAppendingToUseForUndo();
            controllerGesture = 2;
            differenceVector = LeftHand_Controller.transform.position - rightHand_Controller.transform.position;
            initialVector = SelectedGameObject.transform.localScale;
            
            /*else if (leftHandControllerGripActionFlag)
            {
                controllerGesture = 1;
                differenceVector = LeftHand_Controller.transform.position - rightHand_Controller.transform.position;
                initialVector = SelectedGameObject.transform.eulerAngles;
            }*/
        }
        else if (context.performed)
        {
            // Grip action performed
            Debug.Log("Right hand Grip Started");

        }
        else if (context.canceled)
        {
            // Grip action canceled
            leftHandControllerGripActionFlag = false;
            controllerGesture = 0;

        }
    }

    public bool rightHandControllerTriggerActionFlag = false;
    private void OnRightHandControllerTriggerAction(InputAction.CallbackContext context)
    {
        // Your event handling logic here
        if (context.started)
        {

            commandsLogString = returnDateNowString();
            rightHandControllerTriggerActionFlag = true;
            // Grip action performed
            if (controlSelectionMode== 3)
            {
                mode = 1;
                TeleportPlayer(new List<String>(), new List<GameObject>());
                commandsLogString += "Trigger Teleport," + Environment.NewLine;
            }
            else if ( controlSelectionMode == 0)
            {
                commandsLogString += "Trigger Select," + Environment.NewLine;
            }
            else if (controlSelectionMode == 1)
            {
                commandsLogString += "Trigger Move Object," + Environment.NewLine;
            }
            else if (controlSelectionMode == 2)
            {
                commandsLogString += "Trigger Move Player," + Environment.NewLine;
            }
            File.AppendAllText(path, commandsLogString);


        }
        else if (context.performed)
        {
            // Grip action performed
            //Debug.Log("Right hand Grip Started");

        }
        else if (context.canceled)
        {
            rightHandControllerTriggerActionFlag = false;
            controlSelectionMode = 0;

            // Grip action canceled
            //Debug.Log("Right hand Grip Ended -----------------------------------");

        }
    }
    bool showMeshFlag = true;
    public GameObject wireFrameObjects;
    private void OnLeftHandControllerTriggerAction(InputAction.CallbackContext context)
    {
        // Your event handling logic here
        if (context.started)
        {
            if (showMeshFlag)
            {
                showMeshFlag = false;
                wireFrameObjects.SetActive(false);

            }
            else
            {
                wireFrameObjects.SetActive(true);
                showMeshFlag = true;
            }
        }
        else if (context.performed)
        {
            // Grip action performed
            //Debug.Log("Right hand Grip Started");

        }
        else if (context.canceled)
        {
            rightHandControllerTriggerActionFlag = false;
            controlSelectionMode = 0;

            // Grip action canceled
            //Debug.Log("Right hand Grip Ended -----------------------------------");

        }
    }






    void FixedUpdate()
    {
        if (controllerGesture == 1)
        {
            GestureRotation();
        }
        if (controllerGesture == 2)
        {
            GestureResize();
        }

        if (rightHandControllerTriggerActionFlag)
        {
            if (controlSelectionMode== 0)
            {

                RightControllerSelectObject();

            }
            else if (controlSelectionMode== 1)
            {
                mode = 1;
                MoveObject(new List<string>(), new List<GameObject>());
            }
            else if (controlSelectionMode== 2)
            {
                mode = 1;
                MovePlayer(new List<String>(), new List<GameObject>());
            }
        }
        
    }

    Vector3 currentRotation = Vector3.zero;
    Vector3 previousRotation = Vector3.zero;
    private void GestureRotation()
    {




        /*Vector3 RightHandLocalPosition = LeftHand_Controller.transform.InverseTransformPoint(rightHand_Controller.transform.position);
        currentRotation = new Vector3(((RightHandLocalPosition.y - differenceVector.y)*100),
                                    (-(RightHandLocalPosition.z - differenceVector.z)*100),
                                    (-(RightHandLocalPosition.x - differenceVector.x)*100));*/

        //SelectedGameObject.transform.Rotate(currentRotation - previousRotation, Space.World);
        Vector3 currentRotation = new Vector3((LeftHand_Controller.transform.position.y - rightHand_Controller.transform.position.y - differenceVector.y)*100,
                                                (LeftHand_Controller.transform.position.z - rightHand_Controller.transform.position.z - differenceVector.z)*100,
                                                (LeftHand_Controller.transform.position.x - rightHand_Controller.transform.position.x - differenceVector.x)*100);

        


        if (!xRotationLock)
        {
            //SelectedGameObject.transform.Rotate(currentRotation - previousRotation, Space.World);
            //SelectedGameObject.transform.RotateAround(SelectedGameObject.transform.position, Vector3.up, (currentRotation.y - previousRotation.y));
            SelectedGameObject.transform.eulerAngles = new Vector3(miniGameObject.transform.eulerAngles.x, SelectedGameObject.transform.eulerAngles.y, SelectedGameObject.transform.eulerAngles.z);
        }
        if (!yRotationLock)
        {
            SelectedGameObject.transform.eulerAngles = new Vector3(SelectedGameObject.transform.eulerAngles.x, miniGameObject.transform.eulerAngles.y, SelectedGameObject.transform.eulerAngles.z);
            //SelectedGameObject.transform.RotateAround(SelectedGameObject.transform.position, Vector3.forward, (currentRotation.z - previousRotation.z));
        }
        if (!zRotationLock)
        {
            SelectedGameObject.transform.eulerAngles = new Vector3(SelectedGameObject.transform.eulerAngles.x, SelectedGameObject.transform.eulerAngles.y, miniGameObject.transform.eulerAngles.z);
            //SelectedGameObject.transform.RotateAround(SelectedGameObject.transform.position, Vector3.right, (currentRotation.x - previousRotation.x));
        }

        previousRotation = currentRotation;

                                                                
        
    }

    private void GestureResize()
    {
        //Debug.Log("The new angles are   " + ((LeftHand_Controller.transform.position.x - rightHand_Controller.transform.position.x) - differenceVector.x));
        SelectedGameObject.transform.localScale = new Vector3(Math.Abs(LeftHand_Controller.transform.position.x - rightHand_Controller.transform.position.x) - Math.Abs(differenceVector.x) + initialVector.x,
                                                              Math.Abs(LeftHand_Controller.transform.position.y - rightHand_Controller.transform.position.y) - Math.Abs(differenceVector.y) + initialVector.y,
                                                              Math.Abs(LeftHand_Controller.transform.position.z - rightHand_Controller.transform.position.z) - Math.Abs(differenceVector.z) + initialVector.z);
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.P))
        {
            MicrophoneToAudioClip();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            UndoFunction();
        }
        // Check if the space button is released
        if (Input.GetKeyUp(KeyCode.U) || Input.GetKeyUp(KeyCode.P))
        {
            SaveAudioClipAndGenerateText();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (controlSelectionMode== 0)
            {
                RightControllerSelectObject();

            }
            else if (controlSelectionMode== 1)
            {
                mode = 1;
                MoveObject(new List<string>(), new List<GameObject>());
            }
            else if (controlSelectionMode== 2)
            {
                mode = 1;
                MovePlayer(new List<String>(), new List<GameObject>());
            }
            else if (controlSelectionMode== 3)
            {
                mode = 1;
                TeleportPlayer(new List<String>(), new List<GameObject>());
            }
            controlSelectionMode = 0;
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            mode = 0;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            mode = 1;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (controllerGesture == 0)
            {
                controllerGesture = 2;
                differenceVector = LeftHand_Controller.transform.position - rightHand_Controller.transform.position;
                initialVector = SelectedGameObject.transform.localScale;
                //initialVector = SelectedGameObject.transform.eulerAngles;
                //Debug.Log("Difference Vector    "+differenceVector);
                Debug.Log("Gesture Rotation ON");
            }
            else
            {
                Debug.Log("Gesture OFF");
                controllerGesture = 0;
            }
        }

        

    }

    private void MicrophoneToAudioClip()
    {
        stopwatch.Reset();
        stopwatch.Start();
        Debug.Log("Talk");
        chatGPT_ReplyPanel.text = "Talk";
        microphoneClip = Microphone.Start(microphoneName, false, 20, AudioSettings.outputSampleRate);

    }

    private async void SaveAudioClipAndGenerateText()
    {
        Microphone.End(microphoneName);
        stopwatch.Stop();
        AudioClip cutClip = CutAudioClip(microphoneClip, 0, stopwatch.ElapsedMilliseconds / 1000f);
        //byte[] data = SavWav.Save(fileName, cutClip);
        //byte[] data = SavWav.Save("C:\\Users\\ahm_a\\Multimodal_Research\\Recording.wav", cutClip);
        byte[] data = SavWav.Save(fileName, CutAudioClip(microphoneClip, 0, stopwatch.ElapsedMilliseconds / 1000f));

        Debug.Log("Audio clip saved");
        chatGPT_ReplyPanel.text = "Audio clip saved";
        var req = new CreateAudioTranscriptionsRequest
        {
            FileData = new FileData() { Data = data, Name = "audio.wav" },
            // File = Application.persistentDataPath + "/" + fileName,
            Model = "whisper-1",
            Language = "en"
        };
        var res = await openai.CreateAudioTranscription(req);
        try { Debug.Log("This is the orginal voice from the microphone    " + res.Text.ToLower()); } catch (Exception e) { }
        try
        {
            ExtractValues(res.Text.ToLower());
        }
        catch(Exception e)
        {
            Debug.Log("Extract values exception:    "+e);
            chatGPT_ReplyPanel.text = "Extract values exception:    " + e;

        }
    }

    private AudioClip CutAudioClip(AudioClip clip, float startTime, float endTime)
    {
        int startSample = Mathf.FloorToInt(startTime * clip.frequency);
        int endSample = Mathf.FloorToInt(endTime * clip.frequency);
        int length = endSample - startSample;

        float[] data = new float[length * clip.channels];
        clip.GetData(data, startSample);

        AudioClip cutClip = AudioClip.Create("CutClip", length, clip.channels, clip.frequency, false);
        cutClip.SetData(data, 0);

        return cutClip;
    }

    public static CommandList CreateFromJSON(string jsonString)
    {
        Debug.Log("-------------" + jsonString+ "-------------");
        try
        {
            return JsonUtility.FromJson<CommandList>(jsonString);
        }
        catch (Exception e)
        {
            Debug.Log("Error is here" + e);
        }
        return JsonUtility.FromJson<CommandList>(jsonString);
    }

    /*--------------------------------------------------------------------------------------------------------------------------------------*/
    private async void ExtractValues(String prompt)
    {
        Debug.Log("Doing promt");
        prompt = prompt.Replace(".", "");
        chatGPT_ReplyPanel.text = "Doing Prompt:    "+prompt;
        commandsLogString = "-"+prompt + Environment.NewLine;
        File.AppendAllText(path, commandsLogString);
        var newMessage = new ChatMessage()
        {
            Role = "user",
            //Content = "increase cube height by 5"
            Content = prompt
        };

        

        if (messages.Count == 0) newMessage.Content = initializationPrompt + "\n" + newMessage.Content;
        messages.Add(newMessage);


        // Complete the instruction
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            Model = "gpt-3.5-turbo",
            //Model = "gpt-4",
            Messages = messages
        });

        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            var message = completionResponse.Choices[0].Message;
            message.Content = message.Content.Trim();
            messages.Add(message);
            
            
            try
            {
                String jsonmessage = message.Content.Replace("'", "\"");
                //jsonmessage = jsonmessage.Replace("\n", "");
                int jsonStart = jsonmessage.IndexOf('{');
                int jsonend = jsonmessage.LastIndexOf('}');
                jsonmessage = jsonmessage.Substring(jsonStart, jsonend - jsonStart + 1);
                CommandList whatever = CreateFromJSON(jsonmessage);

                InterpretJSON(whatever);

            }
            catch (Exception e)
            {
                Debug.Log("Original Reply: "+ message.Content);
                chatGPT_ReplyPanel.text = message.Content;
                Debug.Log("-------" + e);
            }
            

        }
        else
        {
            Debug.LogWarning("No text was generated from this prompt.");
        }

    }

    private void InterpretJSON(CommandList message)
    {
        List<GameObject> gameObjects = null;
        for (int i = 0; i < message.command.Count; i++) {
            commandsLogString = returnDateNowString();
            commandsLogString += GazeVoice;
            switch (message.command[i].indexes)
            {
                case "1":
                    Debug.Log("Rotating --------");
                    
                    commandsLogString += "Rotating," + Environment.NewLine;
                    
                    //objectsReference.Append(gameObjects);
                    gameObjects = IdentifyGameObjects(message.command[i]);
                    RotateObject(message.command[i].values, gameObjects);
                    
                    break;

                case "2":
                    Debug.Log("Resizing --------");
                    commandsLogString += "Resizing," + Environment.NewLine;
                    gameObjects = IdentifyGameObjects(message.command[i]);
                    objectsReference.Add(gameObjects);
                    AppendToListComponenets(gameObjects);
                    ResizeObject(message.command[i].values, gameObjects);
                    //objectsToDelete.Append(gameObjects);
                    break;

                case "3":
                    Debug.Log("Creating --------");
                    commandsLogString += "Creating," + Environment.NewLine;
                    CreateObject(message.command[i].objects);
                    break;

                case "4":
                    Debug.Log("Deleting --------");
                    commandsLogString += "Deleting," + Environment.NewLine;
                    gameObjects = IdentifyGameObjects(message.command[i]);
                    DeleteObject(gameObjects);
                    break;

                case "5":
                    Debug.Log("Selecting --------");
                    commandsLogString += "Selecting," + Environment.NewLine;
                    gameObjects = IdentifyGameObjects(message.command[i]);
                    SelectObject(gameObjects);
                    break;

                case "6":
                    Debug.Log("Material --------");
                    commandsLogString += "Material," + Environment.NewLine;
                    //objectsReference.Append(gameObjects);
                    gameObjects = IdentifyGameObjects(message.command[i]);
                    objectsReference.Add(gameObjects);
                    AppendToListComponenets(gameObjects);
                    MaterialObject(message.command[i].values, gameObjects);
                    //objectsToDelete.Append(gameObjects);
                    break;

                case "7":
                    Debug.Log("Script --------");
                    commandsLogString += "Script," + Environment.NewLine;
                    //objectsReference.Append(gameObjects);
                    gameObjects = IdentifyGameObjects(message.command[i]);
                    objectsReference.Add(gameObjects);
                    AppendToListComponenets(gameObjects);
                    ScriptObject(message.command[i].values, gameObjects);
                    //objectsToDelete.Append(gameObjects);
                    break;

                case "8":
                    Debug.Log("Component --------");
                    commandsLogString += "Component," + Environment.NewLine;
                    gameObjects = IdentifyGameObjects(message.command[i]);
                    objectsReference.Add(gameObjects);
                    AppendToListComponenets(gameObjects);
                    ComponentObject(message.command[i].values, gameObjects);
                    //objectsToDelete.Append(gameObjects);
                    break;

                case "9":
                    Debug.Log("Move Object --------");
                    commandsLogString += "Move Object," + Environment.NewLine;
                    //objectsReference.Append(gameObjects);
                    gameObjects = IdentifyGameObjects(message.command[i]);
                    objectsReference.Add(gameObjects);
                    AppendToListComponenets(gameObjects);
                    MoveObject(message.command[i].values, gameObjects);
                    break;

                case "10":
                    Debug.Log("Move Player --------");
                    commandsLogString += "Move Player," + Environment.NewLine;
                    gameObjects = IdentifyGameObjects(message.command[i]);
                    MovePlayer(message.command[i].values, gameObjects);
                    break;

                case "11":
                    Debug.Log("Teleport Player --------");
                    commandsLogString += "Teleport Player," + Environment.NewLine;
                    gameObjects = IdentifyGameObjects(message.command[i]);
                    TeleportPlayer(message.command[i].values, gameObjects);
                    //objectsToDelete.Append(gameObjects);
                    break;

                case "12":
                    Debug.Log("Undo --------");
                    commandsLogString += "Undo," + Environment.NewLine;
                    UndoFunction();
                    break;

            }
            File.AppendAllText(path, commandsLogString);

        }
    }

    public void oneObjectAppendingToUseForUndo()
    {
        List<GameObject> gameObjects = new List<GameObject>();
        gameObjects.Add(SelectedGameObject);
        objectsReference.Add(gameObjects);
        AppendToListComponenets(gameObjects);
        try
        {
            if (objectsReference.Count > 10)
            {
                for (int i = 0; i < objectsReference.Count - 10; i++)
                {
                    objectsReference.RemoveAt(0);
                    List<GameObject> objectsToDestroy = objectsCopyReference[0];
                    objectsCopyReference.RemoveAt(0);
                    //Destroy(objectToDestroy);
                    foreach (GameObject objectToDestroy in objectsToDestroy)
                    {
                        Destroy(objectToDestroy);
                    }

                }
            }
        }
        catch (Exception)
        {

        }
    }

    private void AppendToListComponenets(List<GameObject> gameObjects)
    {
        /*List<List<Component>> currectObjectsListComponents = new List<List<Component>>();
        foreach (GameObject gameObject in gameObjects)
        {
            Component[] existingComponents = gameObject.GetComponents<Component>();
            List<Component> currentObjectComponents = new List<Component>(existingComponents);
            currectObjectsListComponents.Add(currentObjectComponents);
        }
        objectsComponents.Add(currectObjectsListComponents);*/
        List<GameObject> gameObjectlist = new List<GameObject>();
        foreach (GameObject gameObject in gameObjects)
        {
            GameObject copy = Instantiate(gameObject);



            Outline scriptComponent = copy.GetComponent<Outline>();
            // Check if the script component exists
            if (scriptComponent != null)
            {
                // Option 1: Destroy the script component
                Destroy(scriptComponent);
                // Option 2: Remove the script component
                // RemoveComponent(scriptComponent);
            }



            Material[] currentMaterials = copy.GetComponent<Renderer>().materials;
            // Create a new list for the materials you want to keep
            List<Material> newMaterials = new List<Material>();
            foreach (Material material in currentMaterials)
            {
                // Check for the materials you want to keep
                if (!material.name.Contains("OutlineFill") && !material.name.Contains("OutlineMask"))
                {
                    newMaterials.Add(material);
                }
            }
            // Reassign the materials property with the new array of materials you want to keep
            copy.GetComponent<Renderer>().materials = newMaterials.ToArray();



            copy.SetActive(false);
            gameObjectlist.Add(copy);
            
        }
        objectsCopyReference.Add(gameObjectlist);
    }

    private List<GameObject> IdentifyGameObjects(CommandClass message)
    {
        List<GameObject> gameObjects = new List<GameObject>();
      
       
        if (message.objects.Count == 0)
        {
            gameObjects.Add(SelectedGameObject);
        }
        else
        {
            foreach (String gameObject in message.objects)
            {
                
                if (gameObject.Contains("that"))
                {
                    //Debug.Log("The gameobject =" + gameObject + "=");
                    EyeGaze cameraEyeGazeScript = cameraObject.GetComponent<EyeGaze>();
                    gameObjects.Add(cameraEyeGazeScript.lastHit);
                    
                }
                else if (gameObject.Equals("it"))
                {
                    gameObjects.Add(SelectedGameObject);
                }
                else
                {
                    GameObject foundObject = GameObject.Find(gameObject);
                    if (foundObject != null)
                    {
                        gameObjects.Add(foundObject);
                        
                    }
                }
            }
        }
        return gameObjects;

    }

    private (List<String>, bool) EyeGazePointLocation(List<String> values)
    {
        bool flag = false;
        if (values.Contains("there") || values.Contains("here"))
        {
            values = new List<string>();
            EyeGaze cameraEyeGazeScript = cameraObject.GetComponent<EyeGaze>();
            Vector3 collision = cameraEyeGazeScript.collision;
            //collision.y += 0.5f;
           
            values.Add(collision.x.ToString());
            values.Add(collision.y.ToString());
            values.Add(collision.z.ToString());
            flag= true;
        }
        return (values,flag);
    }

    private void RightControllerSelectObject()
    {
    
        var ray = new Ray(rightHand_Controller.transform.position, rightHand_Controller.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance: 200))
        {
            SelectedGameObject = hit.transform.gameObject;
            
        }
        
    }

    private List<String> RightControllerPointLocation(bool gameObjectIsSelectedForInteraction)
    {
        Collider collider = null;
        if (gameObjectIsSelectedForInteraction)
        {
            try
            {
                collider = SelectedGameObject.GetComponent<Collider>();
                collider.enabled = false;
            }
            catch (Exception) { }

            
        }
        var ray = new Ray(rightHand_Controller.transform.position, rightHand_Controller.transform.forward);
        RaycastHit hit;
        Vector3 collision = new Vector3();
        if (Physics.Raycast(ray, out hit, maxDistance: 200))
        {
            collision = hit.point;
            if (hit.collider.gameObject == SelectedGameObject)
            {
                doNotTriggerObjectMovementFlag = true;
            }
        }
        List<String> result = new List<String>();
        result.Add(collision.x.ToString());
        result.Add(collision.y.ToString());
        result.Add(collision.z.ToString());
        if (collider != null) { collider.enabled = true; }
        return result;
    }

    private float RelativeValues(float objectVal, String promptVal)
    {
        float val = 0;

        switch (promptVal.Substring(0, 1))
        {
            case "+":
                val = float.Parse(promptVal.Substring(1, promptVal.Length - 1));
                return objectVal + val;

            case "_":
                val = float.Parse(promptVal.Substring(1, promptVal.Length - 1));
                return objectVal - val;

            case "*":
                val = float.Parse(promptVal.Substring(1, promptVal.Length - 1));
                return objectVal * val;

            case "/":
                val = float.Parse(promptVal.Substring(1, promptVal.Length - 1));
                return objectVal / val;

            case "^":
                val = float.Parse(promptVal.Substring(1, promptVal.Length - 1));
                return (float)Math.Pow(objectVal, val);

            default:
                val = float.Parse(promptVal);
                return val;
        }
    }

    private Vector3 Vector3Values(Vector3 vector, List<string> values)
    {
        
        if (values[0].Length > 0)
        {
            vector.x = RelativeValues(vector.x, values[0]);
        }
        if (values[1].Length > 0)
        {
            vector.y = RelativeValues(vector.y, values[1]);
        }
        if (values[2].Length > 0)
        {
            vector.z = RelativeValues(vector.z, values[2]);
        }
        return vector;

    }

    private void RotateObject(List<String> values, List<GameObject> gameObjects)
    {
        if (gameObjects.Count == 0)
        {
            if (values.Count > 1)
            {
                SelectedGameObject.transform.eulerAngles = Vector3Values(SelectedGameObject.transform.localScale, values);
            }
        }
        else
        {
            foreach (GameObject gameObject in gameObjects)
            {

                if (values.Count > 1)
                {
                    gameObject.transform.eulerAngles = Vector3Values(gameObject.transform.localScale, values);
                }
            }
        }

    }

    private void ResizeObject(List<String> values, List<GameObject> gameObjects)
    {
            
        if (gameObjects.Count == 0)
        {
            if (values.Count > 1)
            {
                SelectedGameObject.transform.localScale = Vector3Values(SelectedGameObject.transform.localScale, values);
            }
        }
        else
        {
            foreach (GameObject gameObject in gameObjects)
            {

                if (values.Count > 1)
                {
                    gameObject.transform.localScale = Vector3Values(gameObject.transform.localScale, values);
                }
            }
        }
    }

    private void CreateObject(List<String> objectStrings)
    {
        foreach (String objectString in objectStrings)
        {
            switch (objectString)
            {
                case "sphere":
                    SelectedGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    break;

                case "cube":
                    SelectedGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    break;

                case "capsule":
                    SelectedGameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    break;

                case "cylinder":
                    SelectedGameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    break;

                case "plane":
                    SelectedGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    break;

                case "quad":
                    SelectedGameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    break;

                default:
                    try
                    {
                        SelectedGameObject = Instantiate(Resources.Load<GameObject>("Prefabs/" + objectString));
                    }
                    catch (Exception e) { Debug.Log("Creation Exception:" + e); }
                    break;

            }
        }
    }

    private void DeleteObject(List<GameObject> gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
        }
    }

    private void SelectObject(List<GameObject> gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            Debug.Log("--------------------"+gameObject.name);
            SelectedGameObject = gameObject;

        }
    }

    private void MaterialObject(List<String> values, List<GameObject> gameObjects)
    {
        if (gameObjects.Count == 0)
        {

            try
            {
                //Debug.Log("_"+values[0]+"_");
                Material myMaterial = Resources.Load<Material>("Materials/" + values[0]);
                Renderer objectRenderer = SelectedGameObject.GetComponent<Renderer>();
                Material currentMaterial = objectRenderer.material;
                if (myMaterial != null)
                {
                    objectRenderer.material = myMaterial;
                }
            }
            catch (Exception e) { Debug.Log("Material Apply Exception:" + e); }
        }
        else
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                try
                {
                    Material myMaterial = Resources.Load<Material>("Materials/" + values[i]);
                    Renderer objectRenderer = gameObjects[i].GetComponent<Renderer>();
                    if (myMaterial != null)
                    {
                        objectRenderer.material = myMaterial;
                    }
                }
                catch (Exception e) { Debug.Log("Material Apply Exception:" + e); }

            }
        }
    }

    private void ScriptObject(List<String> values, List<GameObject> gameObjects)
    {
        if (gameObjects.Count == 0)
        {
            try
            {
                UnityEngine.TextAsset scriptAsset = Resources.Load<UnityEngine.TextAsset>("Scripts/" + values[0]);
                System.Type scriptType = System.Type.GetType(scriptAsset.name);
                UnityEngine.Component scriptInstance = SelectedGameObject.AddComponent(scriptType);
            }
            catch (Exception e) { Debug.Log("Merial Apply Exception:" + e); }
        }
        else
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                try
                {
                    UnityEngine.TextAsset scriptAsset = Resources.Load<UnityEngine.TextAsset>("Scripts/" + values[i]);
                    System.Type scriptType = System.Type.GetType(scriptAsset.name);
                    UnityEngine.Component scriptInstance = gameObjects[i].AddComponent(scriptType);

                }
                catch (Exception e) { Debug.Log("Material Apply Exception:" + e); }

            }
        }
    }

    private void ComponentObject(List<String> values, List<GameObject> gameObjects)
    {
        if (gameObjects.Count == 0)
        {
            try
            {
                Rigidbody rigidbody = SelectedGameObject.AddComponent<Rigidbody>();
            }
            catch (Exception e) { Debug.Log("Component Apply Exception:" + e); }
        }
        else
        {
            for (int i = 0; i < gameObjects.Count; i++)
            {
                try
                {

                    Rigidbody rigidbody = gameObjects[i].AddComponent<Rigidbody>();

                }
                catch (Exception e) { Debug.Log("Component Apply Exception:" + e); }

            }
        }
    }

    private void MoveObject(List<String> values, List<GameObject> gameObjects)
    {
        (List<string>, bool) result = (null, false);
        if (mode == 0)
        {
            result = EyeGazePointLocation(values);
        }
        if (mode == 1)
        {
            result = (RightControllerPointLocation(true),true);
        }
        values = result.Item1;
        bool flag = result.Item2;
        if (!doNotTriggerObjectMovementFlag)
        {
            if (gameObjects.Count == 0)
            {
                if (values.Count > 1)
                {
                    SelectedGameObject.transform.position = Vector3Values(SelectedGameObject.transform.position, values);
                    try
                    {
                        Vector3 pos = SelectedGameObject.transform.position;
                        pos.y = pos.y + pos.y - SelectedGameObject.GetComponent<Renderer>().bounds.min.y;
                        SelectedGameObject.transform.position = pos;
                    }
                    catch(Exception)
                    {

                    }
                }
            }
            else
            {
                foreach (GameObject gameObject in gameObjects)
                {

                    if (values.Count > 1)
                    {
                        gameObject.transform.position = Vector3Values(gameObject.transform.position,values);
                        try
                        {
                            Vector3 pos = gameObject.transform.position;
                            //pos.y += gameObject.GetComponent<Renderer>().bounds.size.y/2;
                            pos.y = pos.y + pos.y - gameObject.GetComponent<Renderer>().bounds.min.y;
                            gameObject.transform.position = pos;
                        }
                        catch(Exception)
                        {

                        }
                    }
                    
                }
            }
        }
        else
        {
            doNotTriggerObjectMovementFlag = false;
        }

    }

    private void MovePlayer(List<String> values, List<GameObject> gameObjects)
    {
        (List<string>, bool) result = (null, false);
        if (mode == 0)
        {
            result = EyeGazePointLocation(values);
        }
        if (mode == 1)
        {
            result = (RightControllerPointLocation(false), true);
        }
        values = result.Item1;
        bool flag = result.Item2;
        if (flag)
        {
            PlayerInteraction script = playerObject.GetComponent<PlayerInteraction>();
            script.moveToPosition = Vector3Values(playerObject.transform.position, values);
            script.moveToFlag = true;
        }
        else if (gameObjects.Count == 0)
        {
            try
            {
                PlayerInteraction script = playerObject.GetComponent<PlayerInteraction>();
                script.moveToPosition = SelectedGameObject.transform.position;
                script.moveToFlag = true;
            }
            catch (Exception e) { Debug.Log("Move playerObject Exception:" + e); }
        }
        else
        {
            foreach (GameObject gameObject in gameObjects)
            {
                try
                {

                    PlayerInteraction script = playerObject.GetComponent<PlayerInteraction>();
                    script.moveToPosition = gameObject.transform.position;
                    script.moveToFlag = true;

                }
                catch (Exception e) { Debug.Log("Move playerObject Exception:" + e); }

            }
        }

    }

    private void TeleportPlayer(List<String> values, List<GameObject> gameObjects)
    {
        (List<string>, bool) result = (null, false);
        if (mode == 0)
        {
            result = EyeGazePointLocation(values);
        }
        if (mode == 1)
        {
            result = (RightControllerPointLocation(false), true);
        }
        values = result.Item1;
        bool flag = result.Item2;
        if (flag)
        {
            //playerObject.transform.position = new Vector3(SelectedGameObject.transform.position.x, 0, SelectedGameObject.transform.position.z - 1);
            Vector3 startPos = playerObject.transform.position; // Save starting position
            Vector3 targetPos = Vector3Values(playerObject.transform.position, values);
            Vector3 directionToStart = (startPos - targetPos).normalized; // Direction from targetPos to startPos
            directionToStart.y = 0; // Ignore y axis
            float offset = 1.0f; // Total offset amount
            Vector3 offsetPos = targetPos + directionToStart * offset; // Calculate the offset position
            playerObject.transform.position = offsetPos; // Teleport to the offset position
        }
        else if (gameObjects.Count == 0)
        {
            try
            {
                //playerObject.transform.position = new Vector3(SelectedGameObject.transform.position.x, 0, SelectedGameObject.transform.position.z - 1);
                Vector3  startPos = playerObject.transform.position; // Save starting position
                Vector3 targetPos = SelectedGameObject.transform.position;
                Vector3 directionToStart = (startPos - targetPos).normalized; // Direction from targetPos to startPos
                directionToStart.y = 0; // Ignore y axis
                float offset = 1.0f; // Total offset amount
                Vector3 offsetPos = targetPos + directionToStart * offset; // Calculate the offset position
                playerObject.transform.position = offsetPos; // Teleport to the offset position
            }
            catch (Exception e) { Debug.Log("Teleport playerObject Exception:" + e); }
        }
        else
        {
            foreach (GameObject gameObject in gameObjects)
            {

                try
                {

                    //playerObject.transform.position = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z - 1);
                    Vector3 startPos = playerObject.transform.position; // Save starting position
                    Vector3 targetPos = gameObject.transform.position;
                    Vector3 directionToStart = (startPos - targetPos).normalized; // Direction from targetPos to startPos
                    directionToStart.y = 0; // Ignore y axis
                    float offset = 1.0f; // Total offset amount
                    Vector3 offsetPos = targetPos + directionToStart * offset; // Calculate the offset position
                    playerObject.transform.position = offsetPos; // Teleport to the offset position

                }
                catch (Exception e) { Debug.Log("Teleport playerObject Exception:" + e); }
            }
        }
    }



    public GameObject undoObjectPlaceHolder;
    public void UndoFunction()
    {
        if (objectsReference.Count > 0)
        {
            SelectedGameObject = undoObjectPlaceHolder;

            GameObject copiedObject = null;
            for (int i = 0; i < objectsReference[objectsReference.Count -1].Count;i++)
            {
                GameObject currentObject = objectsReference[objectsReference.Count - 1][i];
                copiedObject = objectsCopyReference[objectsCopyReference.Count - 1][i];

                foreach (var component in currentObject.GetComponents<UnityEngine.Component>())
                {
                    if (!(component is Transform))
                    {
                        DestroyImmediate(component);

                    }
                    
                }
                foreach (var component in copiedObject.GetComponents<UnityEngine.Component>())
                {
                    if (!(component is Transform))
                    {
                        currentObject.AddComponent(component.GetType());
                    }
                    switch (component)
                    {
                        case Transform:
                            currentObject.transform.position = component.transform.position;
                            currentObject.transform.rotation = component.transform.rotation;
                            currentObject.transform.localScale = component.transform.localScale;
                            break;

                        case MeshFilter:
                            currentObject.GetComponent<MeshFilter>().mesh = copiedObject.GetComponent<MeshFilter>().mesh;
                            break;

                        case Renderer:
                            //Debug.Log("Current object materials size    " + currentObject.GetComponent<Renderer>().materials.ToList()[0].name);
                            List<Material> copyMaterials = copiedObject.GetComponent<Renderer>().materials.ToList();
                            currentObject.GetComponent<Renderer>().materials = copyMaterials.ToArray();

                            break;

                    }


                    /*Renderer renderer = currentObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        if (renderer.material == null)
                        {
                            // No Material is set, apply a default one
                            renderer.material = defaultMaterial; // your default Material
                        }
                    }*/
                   
                }

                /*Vector3 ffs = objectsCopyReference[0][0].transform.localScale;
                gameObject.transform.localScale = ffs;
                Debug.Log("Gameobject    " + gameObject.transform.localScale + "     ,Component  " + ffs);
                componentListIndex++;*/
            }

            objectsReference.RemoveAt(objectsReference.Count - 1);
            objectsCopyReference.RemoveAt(objectsCopyReference.Count - 1);
            Destroy(copiedObject);
        }
    }


}

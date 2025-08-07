using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComponentController : MonoBehaviour
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
    public void OnButtonClicked(Button buttonClicked)
    {
        audioGPT.oneObjectAppendingToUseForUndo();
        switch (buttonClicked.gameObject.name)
        {
            case "RigidBody":
                audioGPT.SelectedGameObject.AddComponent<Rigidbody>();

                break;
        }
    }
}

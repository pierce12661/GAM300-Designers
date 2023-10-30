using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AnchorTrigger : MonoBehaviour
{
    public GameObject kartObject;
    public Grapple grappleScript;
    public CanvasGroup popupText;

    public Transform controlPrompt;
    

    

    [HideInInspector] public bool isAnchorTrigger;

    private void Start()
    {
        kartObject = GameObject.FindGameObjectWithTag("Player");
        grappleScript = kartObject.GetComponent<Grapple>();

        
    }

    private void Update()
    {
        PopUpControls();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Sphere") 
        {
            isAnchorTrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Sphere") 
        {
            isAnchorTrigger = false;
        }
    }

    public void PopUpControls()
    {
        Vector3 originalScale = new Vector3(3f, 3f, 3f);

        if (isAnchorTrigger)
        {
            popupText.alpha = Mathf.Lerp(popupText.alpha, 1, 15.0f * Time.deltaTime);

            controlPrompt.localScale = Vector3.Lerp(controlPrompt.localScale, Vector3.one, 10.0f * Time.deltaTime);

            
        }
        else
        {
            popupText.alpha = Mathf.Lerp(popupText.alpha, 0, 15.0f * Time.deltaTime);

            controlPrompt.localScale = Vector3.Lerp(controlPrompt.localScale, originalScale, 10.0f * Time.deltaTime);
        }
    }

}
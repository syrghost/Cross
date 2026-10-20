using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class interagir : MonoBehaviour
{

    public pnjData pnjProche;
    public PlayerController playerController;

    public dialogueManager dialogueManager ;

    public InputAction interagirAction ;

    public GameObject visuelInteraction;

    public GameObject visuelPad ;

    

    // Start is called before the first frame update
    void Start()
    {
        var controls = playerController.controls ;
        interagirAction = controls.Player.parler ;
        visuelInteraction.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(pnjProche != null && interagirAction.WasPressedThisFrame() && ! dialogueManager.enCours && Time.frameCount >= dialogueManager.frameFin )
        {

            
            pnjProche.interagir();
        }

        if(pnjProche != null && dialogueManager.enCours == false)
        {
            visuelInteraction.SetActive(true);

        }
        else
        {
            visuelInteraction.SetActive(false);
        }

        if(dialogueManager.enCours ) visuelPad.SetActive(false);
        else 
        {
            visuelPad.SetActive(true);
        }
    }
}

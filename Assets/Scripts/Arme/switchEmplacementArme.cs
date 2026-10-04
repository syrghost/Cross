using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class switchEmplacementArme : MonoBehaviour
{
    public gestionnaireArmeJoueur gestionnaireArmeJoueur;
    private GameObject slot;
    public GameObject slotMainAutre;
    public GameObject slotMainArc;
    private GameObject armeActu;

    public bool armeEnMain = false ; 
    public Animator animator;
    public armeData armeData;
    private bool enCourDeChangement = false;

    public bool mainSurCorde = false;

    public PlayerController playerController;
    private InputAction degainerAction;
    
    void Start()
    {
        var controls = playerController.controls;
        degainerAction = controls.Player.Degainer;
        
    }

    
    void Update()
    {
        if (degainerAction.WasPressedThisFrame()  && ! enCourDeChangement && armeActu != null)
        {
            enCourDeChangement = true;
            
            if (armeEnMain)
            {
                animator.SetTrigger("triggerRanger");
            }
            else
            {
                animator.SetTrigger("triggerDegainer");
            }

        
            
            
        }
        animator.SetBool("armeEnMain", armeEnMain);
        
    }

    public void sortiLance()
    {
        armeEnMain = true;
        positionnerArme();
       

    }
    public void rangerLance()
    {
        armeEnMain= false;
       positionnerArme();
      
    }

    public void finChangementArme()
    {
        enCourDeChangement = false;
    }

    public void definirArmeActu(GameObject nouvelleArme)
    {
        armeActu = nouvelleArme;
        armeData =nouvelleArme != null ? nouvelleArme.GetComponent<armeData>() : null;
        positionnerArme();
    }

    void positionnerArme()
    {
        if(armeActu == null || armeData == null)
        {
            return;
        }
        if (armeEnMain)
        {
            if(armeData.arme.monTypeArme == Arme.typeArme.Arc)
            {
                slot = slotMainArc;
            }
            else
            {
                slot = slotMainAutre;
            }
            armeActu.transform.SetParent(slot.transform);
            //armeActu.transform.localPosition = new Vector3(-0.381f,0.22f,-0.002f);
            //armeActu.transform.localRotation = Quaternion.Euler(-15.507f ,-189.906f,-74f);
            armeActu.transform.localPosition = armeData.arme.positionMain;
            armeActu.transform.localRotation = Quaternion.Euler(armeData.arme.rotationMain);
            Debug.Log("changement effectuer");
        }
        else
        {
            armeActu.transform.SetParent(gestionnaireArmeJoueur.slotRanger);
            //armeActu.transform.localPosition = new Vector3(-0.032f,-0.227f,-0.156f);
            //armeActu.transform.localRotation = Quaternion.Euler(0f ,-172.4f,-22.9f);
            armeActu.transform.localPosition = armeData.arme.positionDos;
            armeActu.transform.localRotation = Quaternion.Euler(armeData.arme.rotationDos);
        }
    }

    public Arme.typeArme obtenirTypeArmeEquipee()
    {
        if(armeData == null || armeData.arme == null)
        {
            return Arme.typeArme.mainNues;
        }
        return armeData.arme.monTypeArme;
    } 


    public void activerSuiviCorde()
    {
        mainSurCorde = true ;
        Debug.Log("je suis ");
    }

}

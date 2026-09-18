using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor.UI;
using UnityEngine;

public class switchEmplacementArme : MonoBehaviour
{
    public gestionnaireArmeJoueur gestionnaireArmeJoueur;
    private GameObject slot;
    public GameObject slotMainAutre;
    public GameObject slotMainArc;
    private GameObject armeActu;

    public bool armeEnMain = false ; 
    public Animator animator;
    private armeData armeData;
    private bool enCourDeChangement = false;
    
    void Start()
    {
   
        
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)  && ! enCourDeChangement && armeActu != null)
        {
            enCourDeChangement = true;
            armeEnMain = !armeEnMain;
            
            if ( !armeEnMain)
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
        positionnerArme();
        armeEnMain = true;

    }
    public void rangerLance()
    {
       positionnerArme();
       armeEnMain= false;
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

}

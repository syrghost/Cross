using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI ;

public class sprint : MonoBehaviour
{

    public PlayerController playerController ;

    public modeFocus modeFocus ;
    public PlayerCombat playerCombat;
    public switchEmplacementArme switchEmplacementArme ;
    private float endurance ;
    public float maxEndurance = 100 ;

    public float tempsEndurance  = 20f;
    public float tempsEnduranceRecharge = 40f ;

    public Image visuelVert ;
    public Image visuelRouge;

    public Image arrierePlan ;
    
    private bool enduranceFinis ;

    public bool peutSprint = true ;
    void Start()
    {
        endurance = maxEndurance ;
        arrierePlan.gameObject.SetActive(false);
    }

    
    void Update()
    {
        if (playerController.courirAction.IsPressed()&& ! enduranceFinis && playerController.inputDirection != Vector2.zero && !modeFocus.focusActiver && !playerCombat.enTrainDattaquer && !playerController.enSaut && !switchEmplacementArme.enCourDeChangement )
        {
            arrierePlan.gameObject.SetActive(true);
            if(endurance > 0)
            {
                endurance -= tempsEndurance * Time.deltaTime ;
            }
            else
            {
                enduranceFinis = true ;
                peutSprint = false ;
                
            }

            visuelRouge.fillAmount = endurance / maxEndurance + 0.07f;

        }
        else
        {
            
            if(endurance< maxEndurance)
            {
                endurance += tempsEnduranceRecharge * Time.deltaTime ;
            }
            else
            {
                arrierePlan.gameObject.SetActive(false) ;
                enduranceFinis = false ;
                peutSprint = true ;
            }

            visuelRouge.fillAmount = endurance / maxEndurance;
        }

        visuelVert.fillAmount = (endurance / maxEndurance) ;
        
        
    }
}

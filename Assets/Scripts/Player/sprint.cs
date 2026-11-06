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

    public float tempsEnduranceGrimpe =  20f ;

    public float tempsEndurance  = 20f;
    public float tempsEnduranceRecharge = 40f ;

    public Image visuelVert ;
    public Image visuelRouge;

    public Image arrierePlan ;
    
    private bool enduranceFinis ;

    public bool peutSprint = true ;

    public grimper grimper ;

    private bool estEnTrainDeGrimper ;
    void Start()
    {
        endurance = maxEndurance ;
        arrierePlan.gameObject.SetActive(false);
        
    }

    
    void Update()
    {
        estEnTrainDeGrimper = grimper.estEnTrainDeGrimper ;
        if (playerController.courirAction.IsPressed()&& ! enduranceFinis && playerController.inputDirection != Vector2.zero && !modeFocus.focusActiver && !playerCombat.enTrainDattaquer && !playerController.enSaut && !switchEmplacementArme.enCourDeChangement && !estEnTrainDeGrimper || estEnTrainDeGrimper)
        {
            arrierePlan.gameObject.SetActive(true);
            if(endurance > 0)
            {
                if (estEnTrainDeGrimper)
                {
                    endurance -= tempsEnduranceGrimpe   * Time.deltaTime ;
                }
                else
                {
                    endurance -= tempsEndurance * Time.deltaTime ;
                }
                
            }
            else
            {
                if (estEnTrainDeGrimper)
                {
                    //grimper.arreterEscalade() ;
                }
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

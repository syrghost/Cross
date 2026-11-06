using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class lancerSort : MonoBehaviour
{

    public PlayerController playerController ;
    public gestionnaireGrimoireJoueur grimoires ;

    public Transform pointLancer ;

    public manaJoueur manaJoueur ;

    public Animator animator ;
    public inventaireManager inventaireManager ;
    private InputAction lancerLeSort, viserManaZone , anullerSort , lancerSortZone ;

    public switchEmplacementArme switchEmplacementArme ;

    public modeFocus modeFocus ;

    private grimoire grimoireEnCour ;

    public bool enZoneVisee = false ;
    public CinemachineInputProvider cameraExploration ;

    public GameObject zoneViseeAffichage ;

    private GameObject zone ;

    public float vitesseDeplacementZone = 5f ;

    private Vector3 positionZoneCible ;

    public float vitesseRotationVersZone =10f ;

    private bool estEntrainDetournerVersZone = false ;

    public instancePotion instancePotion ;



    

    // Start is called before the first frame update
    void Start()
    {
        var controls = playerController.controls ;
        lancerLeSort = controls.Player.manaZone ;
        viserManaZone = controls.viseeManaZone.controleVisee ;
        anullerSort = controls.viseeManaZone.annulerManaZone ;
        lancerSortZone = controls.viseeManaZone.manaZone ;

    }

    // Update is called once per frame
    void Update()
    {
        if(instancePotion.peutBoire == false)
        {
            return ;
        }

        if(enZoneVisee && modeFocus.focusActiver ) terminerViser();

        if (estEntrainDetournerVersZone && grimoireEnCour.typeSort == typeSortGrimoire.zone)
        {
            Vector3 directionVersZone = positionZoneCible- transform.position ;
            directionVersZone.y = 0f ;
            if(directionVersZone != Vector3.zero)
            {
                Quaternion rotationCible = Quaternion.LookRotation(directionVersZone);

                transform.rotation = Quaternion.Slerp(transform.rotation , rotationCible , vitesseRotationVersZone * Time.deltaTime) ;
            }
            
        }
        if (lancerLeSort.WasPressedThisFrame() && inventaireManager.armeEquipeeActu.monTypeArme== Arme.typeArme.baton && switchEmplacementArme.armeEnMain == true)
        {
 
            grimoire g = grimoires.grimoireActuelle ;

            if(g== null ) return ;

            if(manaJoueur.manaActuelle < g.coutMana) return ;

            grimoireEnCour = g ;

            if(grimoireEnCour.typeSort == typeSortGrimoire.projectile)
            {
                 animator.SetTrigger("lancerSort") ;
            }

            else if (grimoireEnCour.typeSort == typeSortGrimoire.zone)
            {

                if (!enZoneVisee && !modeFocus.focusActiver)
                {
                    commencerViser();
                }
                else if (!enZoneVisee && modeFocus.focusActiver)
                {
                    animator.SetTrigger("lancerSortZone");
                }
            }
        }

        if (enZoneVisee && !modeFocus.focusActiver)
        {

            Vector2 stick = viserManaZone.ReadValue<Vector2>();

            intancierEtDirigerVisuelle(stick) ;


            if (anullerSort.WasPressedThisFrame())
            {
                terminerViser() ;
            }                   

            if (lancerSortZone.WasPressedThisFrame())
            {
                if( zone != null)
                {
                    positionZoneCible = zone.transform.position ;
                }
                estEntrainDetournerVersZone = true ;
                animator.SetTrigger("lancerSortZone");

                terminerViser();
                        
                        
            }
            
                    


                    
        }

            
               
    

        animator.SetBool("enViserMagieZone", enZoneVisee) ;
    }


// appeler depuis une animation de l'animator
   public void lancer()
    {

        Vector3 direction = Vector3.zero ;
        grimoire g = grimoireEnCour;

        if( g==null ) return ;
        if( !manaJoueur.consommer(g.coutMana)) return ;
        estEntrainDetournerVersZone = false ;

        if(modeFocus.focusActiver && modeFocus.ennemiCible != null)
        {
            if(g.typeSort == typeSortGrimoire.projectile)
            {
                Transform pointVise = modeFocus.obtenirPoinVise(modeFocus.ennemiCible);
                direction = pointVise.position- pointLancer.position;
                
            }
            else if (g.typeSort == typeSortGrimoire.zone)
            {
                direction = modeFocus.ennemiCible.transform.position ;
            }
            
        }
        else if( !modeFocus.focusActiver)
        {
            if(g.typeSort == typeSortGrimoire.projectile)
            {
                direction = transform.forward;
            }

            else if (g.typeSort == typeSortGrimoire.zone)
            {
                direction = positionZoneCible ;
            }
            
        }
        

        



        if(g.typeSort == typeSortGrimoire.projectile)
        {
            GameObject obj =  Instantiate(g.sort , pointLancer.position , pointLancer.rotation);
            obj.GetComponent<projectileSort>().initialiser(g,direction) ;
        }
        else if (g.typeSort == typeSortGrimoire.zone)
        {
            Debug.Log("je me lance");
            GameObject obj = Instantiate(g.sort , direction , Quaternion.identity );
            obj.GetComponent<zoneAttaqueMagique>().infligerDegatZone(g.degatSort , g.rayonZoneProjectile);
            Debug.Log("je me lance pas ");
            Destroy(obj , g.dureeVieZone) ;
        }
    }

    public void commencerViser()
    {
        enZoneVisee = true ;
        cameraExploration.enabled =  false ;
        playerController.controls.Player.Disable();
        playerController.controls.viseeManaZone.Enable();


    }

    public void terminerViser()
    {
        
        enZoneVisee = false ;
        cameraExploration.enabled =  true  ;
        playerController.controls.Player.Enable();
        playerController.controls.viseeManaZone.Disable();
        Destroy(zone );
        

 

    }

    void intancierEtDirigerVisuelle(Vector2 direction)
    {
       

        if(zone == null)
        {
            zone = Instantiate(zoneViseeAffichage , transform.position  , transform.rotation) ;
        }

        Vector3 camForward = playerController.camTransform.forward ;
        Vector3 camRight = playerController.camTransform.right ;

        camForward.y = 0f ;
        camRight.y = 0f ; 

        camForward.Normalize();
        camRight.Normalize();  
        
        Vector3 inputDir = (camForward * direction.y) + (camRight* direction.x) ;

        zone.transform.Translate(inputDir * vitesseDeplacementZone * Time.deltaTime , Space.World) ;

       
    }

    void OnDisable()
    {
        terminerViser();
    }
}

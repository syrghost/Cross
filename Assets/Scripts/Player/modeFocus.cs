using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class modeFocus : MonoBehaviour
{
    private SphereCollider detectionMob;
    private int armeEquiper ;
    public bool focusActiver = false;
    private bool ennemisAProximiter = false;
    public Animator animator;
    public List<Transform> ennemisDetecter = new List<Transform>();
    public IReadOnlyList<Transform> EnnemisDetectes => ennemisDetecter; 
    public Transform ennemiCible;
    private PlayerController playerController;
    public bool enTrainDeTirer = false;
    public PlayerCombat playerCombat;
    public inventaireManager inv;
    


    private float vitesseRotation = 10f;
    public float distanceMin= 10f;

    public GameObject player;
    public float vitesseStrafe = 5f;

    [Header("Gestion de l'arc")]
    public bool enVisee = false ;
    public float correctionAngle = -90f;
    public Transform camTransform;
    Quaternion rotationSouhaiter  ;


    [Header("Gestion de la camera en mode focus")]
    public CinemachineVirtualCamera camCombat;
    public CinemachineTargetGroup targetGroup;

    public switchEmplacementArme switchEmplacementArme;

    [Header("Gestion tension Arc")]
    public float vitesseTensionArc = 1.5f;


    private InputAction focusAction , viseeAction , tirerArcAction , moveAction , changerCibleAction;

    private float tensionArc = 0f;


    [Header("changement cible " )]
    public float SeuilchangementCible = 0.5f;
    public bool pretAchangerCible = true;


    void Start()
    {

        detectionMob = GetComponent<SphereCollider>();
        playerController = player.GetComponent<PlayerController>();
        if(detectionMob == null) Debug.Log("pas de detection");
        else Debug.Log("detectionMob trouver");

        if(Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }

        var controls = playerController.controls;
        focusAction = controls.Player.Focus;
        viseeAction = controls.Player.Visee;
        tirerArcAction = controls.Player.TirerArc;
        moveAction = controls.Player.Move;

        changerCibleAction = controls.Player.CameraControle;



    }

    
    void Update()
    {
        armeEquiper = (int)switchEmplacementArme.obtenirTypeArmeEquipee();

        
        if (focusActiver && ennemiCible == null)
        {
            Debug.Log("L'ennemi ciblé a été éliminé/détruit !");
            
           
            ennemisDetecter.RemoveAll(e => e == null);

            if (ennemisDetecter.Count > 0)
            {
                
                targetGroup.RemoveMember(ennemiCible); 
                ennemiCible = trouverEnnemiPlusProche();
                if (ennemiCible != null)
                {
                    targetGroup.AddMember(ennemiCible, 1f, 1.5f);
                    Debug.Log("Basculement automatique sur un nouvel ennemi.");
                }
            }
            else
            {
                
                focusActiver = false;
                camCombat.Priority = 0;
                ennemisAProximiter = false;
                animator.SetBool("focusMode", false);
                Debug.Log("Plus aucun ennemi, désactivation du mode focus.");
            }
        }
        

        if (ennemisAProximiter == true && focusAction.WasPressedThisFrame() && ennemisDetecter.Count > 0)
        {
            focusActiver = !focusActiver;
            if (focusActiver)
            {
                
                ennemisDetecter.RemoveAll(e => e == null);
                
                ennemiCible = trouverEnnemiPlusProche();
                if (ennemiCible != null)
                {
                    Debug.Log("je focus l'ennemi");
                    targetGroup.AddMember(ennemiCible, 1f, 1.5f);
                    camCombat.Priority = 20;
                }
                else
                {
                    focusActiver = false; 
                }
            }
            else
            {
                SortirDuFocus();
            }
        }

        enVisee = viseeAction.IsPressed() && (armeEquiper == (int)Arme.typeArme.Arc);
        if (enVisee && tirerArcAction.WasPressedThisFrame() && !enTrainDeTirer)
        {
            animator.SetTrigger("DecocherFleche");
            enTrainDeTirer = true;
            tensionArc -= 0f;
        }

        
        if (armeEquiper == (int)Arme.typeArme.Arc && switchEmplacementArme.armeEnMain && !enTrainDeTirer)
        {
            if (enVisee)
            {
                tensionArc += vitesseTensionArc * Time.deltaTime;
                int layerArc = animator.GetLayerIndex("Arc Layer");
                animator.Play("PreparationTir", layerArc, tensionArc);
            }
            else
            {
                tensionArc = 0f;
                switchEmplacementArme.mainSurCorde = false;
            }
            tensionArc = Mathf.Clamp01(tensionArc);
        }

        if (focusActiver)
        {
            Vector2 stick = changerCibleAction.ReadValue<Vector2>();
            Debug.Log("stick.x = " + stick.x + " | pretAchangerCible = " + pretAchangerCible);
            if(Mathf.Abs(stick.x) < 0.2f)
            {
                pretAchangerCible = true;
            }
            if(pretAchangerCible && Mathf.Abs(stick.x) >= SeuilchangementCible)
            {
                Debug.Log("CONDITION VRAIE, appel changerCible");
                pretAchangerCible = false;
                changerCible(stick.x > 0 ? 1:-1 );

            }
            else
            {
                Debug.Log("Condition fausse : pret=" + pretAchangerCible + " abs=" + Mathf.Abs(stick.x));
            }
        }

        animator.SetBool("focusMode", focusActiver);
        animator.SetBool("enVisee", enVisee);
        animator.SetInteger("ArmeEquiper", armeEquiper);
        deplacementModeFocus();
    }

   
    void SortirDuFocus()
    {
        if (ennemiCible != null)
        {
            targetGroup.RemoveMember(ennemiCible);
            ennemiCible = null;
        }
        camCombat.Priority = 0;
        focusActiver = false;
        animator.SetBool("focusMode", false);
    }


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ennemis"))
        {
            return;

        }
        ennemisAProximiter = true;
        ennemisDetecter.Add(other.transform);
        Debug.Log("ennemis dectecter");
    }

    void OnTriggerExit(Collider other)
    {
        if ( !other.CompareTag("Ennemis"))
        {
            return;
        }
        ennemisAProximiter = ennemisDetecter.Count > 0;
        Debug.Log("ennemis sortie de la zone");
        ennemisDetecter.Remove(other.transform);
        ennemisDetecter.RemoveAll(e => e == null);

        if ( other.transform == ennemiCible)
        {
            ennemiCible = null;
            focusActiver = false;
            targetGroup.RemoveMember(other.transform);
            camCombat.Priority = 0;
            animator.SetBool("focusMode",false);
        }

        Debug.Log("Ennemi sorti. Total restant : " + ennemisDetecter.Count);
    }

    Transform trouverEnnemiPlusProche()
    {
        Transform plusProche = null;
        float distMin = Mathf.Infinity;
        foreach( var e in ennemisDetecter)
        {
            if ( e == null)
            {
                continue;
            }
            float d = Vector3.Distance(transform.position,centreReel(e));
            if (d < distMin)
            {
                distMin = d ;
                plusProche = e;
            }
        }
        return plusProche;
    }

    void deplacementModeFocus()
    {
        if(playerCombat != null && playerCombat.enTrainDattaquer)
        {
            return;
        }
        Vector2 move = moveAction.ReadValue<Vector2>();
        float inputX = move.x;
        float inputZ =move.y;
        bool estEnAction = !animator.GetCurrentAnimatorStateInfo(1).IsName("Rien");

        if (estEnAction)
        {
            inputX = 0f;
            inputZ = 0f;
        }

         
        Vector3 directionCible = Vector3.zero;

        if(focusActiver && ennemiCible != null)
        {
            directionCible = centreReel(ennemiCible)- player.transform.position;

            float distanceActuelle = Vector3.Distance(player.transform.position, centreReel(ennemiCible));

            if (inputZ > 0 && distanceActuelle<= distanceMin)
            {
                inputZ = 0f;
            }
        }
            
        else if (enVisee && camTransform != null && !focusActiver )
        {
            if ( !switchEmplacementArme.armeEnMain)
            {
                directionCible = Vector3.zero;
            }
            else
            {
                directionCible = camTransform.forward;
            }

            
        }



        
        if (directionCible != Vector3.zero)
        {
            
            directionCible.y = Mathf.Clamp(0f,-1f,15f);
            if (enVisee && !focusActiver )
            {
                //if (armeMain.armeEnMain)
                //{
                    //rotationSouhaiter = Quaternion.Euler(0f,0f,0f);
                //}
                Quaternion rotationDeBase = Quaternion.LookRotation(directionCible);
                rotationSouhaiter = rotationDeBase *  Quaternion.Euler(0f,90f,0f); 
            }
            else if (focusActiver || (focusActiver && enVisee))
            {
                rotationSouhaiter = Quaternion.LookRotation(directionCible );
            }
            
             

          
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation,rotationSouhaiter,Time.deltaTime * vitesseRotation );
            Vector3 direction = (player.transform.right * inputX + player.transform.forward * inputZ).normalized;
            playerController.DeplacementStrafe(direction, vitesseStrafe);
        }
            

        animator.SetFloat("directionX",inputX, 0.1f, Time.deltaTime);
        animator.SetFloat("directionZ",inputZ, 0.1f, Time.deltaTime);
    }  

    // pour trouver le centre de l'ennemi 
    public Vector3 centreReel(Transform cible)
    {
        if(cible == null ) return Vector3.zero;
        Collider col = cible.GetComponent<Collider>();
        return col != null  ? col.bounds.center : cible.position ; 
    }

    void changerCible(int sens)
    {

        
        ennemisDetecter.RemoveAll(e => e==null);
        Debug.Log("changerCible appelée, sens=" + sens + ", nb ennemis détectés=" + ennemisDetecter.Count);
        if(ennemisDetecter.Count <=1) return ;

        Transform meilleurCible = null;
        float meilleurAngle = Mathf.Infinity;

        foreach(Transform e in ennemisDetecter)
        {
            if(e == ennemiCible)
            {
                continue;
            }
            Vector3 versEnnemi = centreReel(e) -player.transform.position;
            versEnnemi.y = 0f;

            //angle 
            float angle = Vector3.SignedAngle(camTransform.forward,versEnnemi, Vector3.up);
            Debug.Log("Ennemi " + e.name + " -> angle = " + angle);

            if(sens> 0 && angle <=0) continue;
            if(sens <0 && angle >= 0) continue;

            float distanceAngle = Mathf.Abs(angle);
            if(distanceAngle < meilleurAngle)
            {
                meilleurAngle = distanceAngle;
                meilleurCible = e ;
            }


        }

        Debug.Log("Meilleure cible trouvée : " + (meilleurCible != null ? meilleurCible.name : "AUCUNE"));
        if(meilleurCible != null)
        {
            targetGroup.RemoveMember(ennemiCible);
            ennemiCible = meilleurCible;
            targetGroup.AddMember(ennemiCible,1f,1.5f);
        }

    }

    public Transform obtenirPoinVise(Transform ennemi)
    {
        if(ennemi == null) return null;

        foreach(Transform enfant in ennemi.GetComponentInChildren<Transform>())
        {
            if (enfant.CompareTag("pointFlecheEnnemi"))
            {
                return enfant ;
            }
        }
        return ennemi;


    }





}

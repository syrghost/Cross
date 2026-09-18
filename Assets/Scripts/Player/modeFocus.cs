using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;


public class modeFocus : MonoBehaviour
{
    private SphereCollider detectionMob;
    private int armeEquiper ;
    public bool focusActiver = false;
    private bool ennemisAProximiter = false;
    public Animator animator;
    private List<Transform> ennemisDetecter = new List<Transform>();
    public Transform ennemiCible;
    private PlayerController playerController;
    


    private float vitesseRotation = 10f;
    public float distanceMin= 10f;

    public GameObject player;
    public float vitesseStrafe = 5f;

    [Header("Gestion de l'arc")]
    public bool enVisee = false ;
    public float correctionAngle = -90f;
    private Transform camTransform;
    Quaternion rotationSouhaiter  ;


    [Header("Gestion de la camera en mode focus")]
    public CinemachineVirtualCamera camCombat;
    public CinemachineTargetGroup targetGroup;

    public switchEmplacementArme switchEmplacementArme;
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



    }

    
    void Update()
    {
        armeEquiper = (int)switchEmplacementArme.obtenirTypeArmeEquipee();
        if (ennemisAProximiter == true && Input.GetKeyDown(KeyCode.F) && ennemisDetecter.Count> 0)
        {

            focusActiver = !focusActiver;
            if (focusActiver)
            {
                ennemiCible = trouverEnnemiPlusProche();
                Debug.Log("je focus l'ennemi");

                targetGroup.AddMember(ennemiCible,1f,1.5f);
                camCombat.Priority = 20;
            }
            else
            {
                if(ennemiCible != null)
                {
                    targetGroup.RemoveMember(ennemiCible);
                }
                camCombat.Priority = 0;
            }
             
            
        }

        enVisee = Input.GetMouseButton(1) && (armeEquiper == (int)Arme.typeArme.Arc)  ;
        if(enVisee && Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("DecocherFleche");
        }
        animator.SetBool("focusMode",focusActiver);
        animator.SetBool("enVisee",enVisee);
        animator.SetInteger("ArmeEquiper",armeEquiper);
        deplacementModeFocus();

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
            float d = Vector3.Distance(transform.position, e.position);
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
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        bool estEnAction = !animator.GetCurrentAnimatorStateInfo(1).IsName("Rien");

        if (estEnAction)
        {
            inputX = 0f;
            inputZ = 0f;
        }

         
        Vector3 directionCible = Vector3.zero;

        if(focusActiver && ennemiCible != null)
        {
            directionCible = ennemiCible.position - player.transform.position;

            float distanceActuelle = Vector3.Distance(player.transform.position, ennemiCible.position);

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
            

        animator.SetFloat("directionX",inputX);
        animator.SetFloat("directionZ",inputZ);
    }    
}

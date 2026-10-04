using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Paramètres Mouvement (Style Template)")]
    [Tooltip("Vitesse de rotation douce du personnage")]
    public float rotationSmoothing = 0.12f;
    [Tooltip("Seuil d'angle pour déclencher un demi-tour (si géré par votre Animator)")]
    public float seuilAngle180 = 160f; 

    [Header("Physique & Gravité")]
    public float gravite = -15.0f; // Valeur du template plus réaliste, modifiable

    [Header("Références & Modes")]
    public modeFocus modeFocus;

    
    private CharacterController controller;
    private Animator animator;
    private Transform camTransform;
    private Vector3 velocityY;
    
    // Variables de rotation internes du template
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    public float multiplicateurVitesse = 2f;
    public playerControls controls;
    private InputAction moveAction,sautAction;

    public PlayerCombat playerCombat;
    public float vitesseRotationAttaque = 15f;

    [Header("saut")]
    public float hauteurSaut  = 1.2f;
    private Vector3 vitesseAir;
    private bool  enSaut;
    public float distanceSaut = 10f ;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        if (Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
    }

    void Awake()
    {
        controls= new playerControls();
        moveAction = controls.Player.Move;
        sautAction = controls.Player.saut;
    }
    void OnEnable()
    {
        controls.Player.Enable();
    }
    void OnDisable()
    {
        controls.Player.Disable();
    }

    void Update()
    {
        CalculerMouvementEtEnvoyerAAnimator();
        AppliquerGravite();
        gererSaut();

        if (enSaut)
        {
            controller.Move(vitesseAir * distanceSaut * Time.deltaTime);
        }
    }

    void CalculerMouvementEtEnvoyerAAnimator()
    {
        
        if (modeFocus != null && modeFocus.focusActiver)
        {
            
            animator.SetFloat("vitesse", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("vitesseAngulaire", 0f, 0.1f, Time.deltaTime);
            return;
        }


        
        Vector2 rawInput = moveAction.ReadValue<Vector2>();
        Vector2 inputDirection = rawInput.normalized;

        float intensiteInput = Mathf.Clamp01(inputDirection.magnitude);
        float angleDelta = 0f;

        
        if (inputDirection != Vector2.zero)
        {
            
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            
            
            float angleActuel = transform.eulerAngles.y;
            angleDelta = Mathf.DeltaAngle(angleActuel, _targetRotation);

          
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothing);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        
        if (intensiteInput > 0.85f) intensiteInput = 1f;
        if (intensiteInput > 0.1f) animator.SetFloat("derniereVitesse", intensiteInput);

       
        float vitesseAngulaireCible = Mathf.Clamp(angleDelta / 90f, -1f, 1f);

        
        if (intensiteInput <= 0.1f && Mathf.Abs(angleDelta) < 20f) 
        {
            vitesseAngulaireCible = 0f;
        }

        
        animator.SetFloat("vitesse", intensiteInput);
        animator.SetFloat("vitesseAngulaire", vitesseAngulaireCible, 0.1f, Time.deltaTime);
    }

    void AppliquerGravite()
    {
        if (controller.isGrounded)
        {
            if (velocityY.y < 0)
            {
                velocityY.y = -2f; // Force plaquant le joueur au sol de manière stable
            }
        }
        else
        {
            // Applique la gravité frame par frame si on est dans le vide
            velocityY.y += gravite * Time.deltaTime;
        }

        // Déplacement vertical uniquement lié à la physique
        controller.Move(new Vector3(0, velocityY.y, 0) * Time.deltaTime);
    }

    void gererSaut()
    {
        Vector2 rawInput = moveAction.ReadValue<Vector2>();
        Vector2 inputDirection = Vector2.ClampMagnitude(rawInput,1f);
        bool auSol = controller.isGrounded;
        animator.SetBool("estAuSol", auSol);
       
        bool occuper = (playerCombat != null && playerCombat.enTrainDattaquer ) || (modeFocus != null && (modeFocus.focusActiver || modeFocus.enTrainDeTirer));
        if(auSol && !occuper && sautAction.WasPressedThisFrame())
        {
            velocityY.y = Mathf.Sqrt(hauteurSaut * -2f * gravite);
            animator.SetTrigger("saut");

           Vector3 dirInput = new Vector3(inputDirection.x, 0f , inputDirection.y);
           vitesseAir = Quaternion.Euler(0f,camTransform.eulerAngles.y , 0f) * dirInput ;
            enSaut = true;
        }

        if(enSaut && auSol && velocityY.y <= 0)
        {
            enSaut = false;
            vitesseAir = Vector3.zero;
        }
    }

    void OnAnimatorMove()
    {
        if (animator == null) return;

        bool estEnAction = !animator.GetCurrentAnimatorStateInfo(1).IsName("Rien");
        
        if (modeFocus != null && modeFocus.focusActiver && !estEnAction)
        {
            return;
        }

        // Récupération du déplacement XZ dicté par l'animation (Root Motion)
        Vector3 rootMotionXZ = new Vector3(animator.deltaPosition.x, 0f, animator.deltaPosition.z);
        rootMotionXZ *= multiplicateurVitesse ;
        
        // Application du Root Motion au CharacterController
        controller.Move(rootMotionXZ);
        if (playerCombat != null && playerCombat.enTrainDattaquer)
        {
            if(playerCombat.CibleVeroiller != null)
            {
                Vector3 direction = modeFocus.centreReel(playerCombat.CibleVeroiller) - transform.position ;
                direction.y = 0f;
                if(direction.sqrMagnitude > 0.01)
                {
                    Quaternion rotationCible = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation , rotationCible , Time.deltaTime * vitesseRotationAttaque);

                }
                
            }

            return;
        }
        
        transform.rotation *= animator.deltaRotation;
    }

    public void DeplacementStrafe(Vector3 direction, float vitesse)
    {
        controller.Move(direction * vitesse * Time.deltaTime);
    }

    public void DeplacementLunge(Vector3 direction , float vitesse )
    {
        controller.Move(direction * vitesse * Time.deltaTime);
    }

    // Pour l'animation de tirer 
    public void finDecocherFleche()
    {
        if (modeFocus != null)
        {
            modeFocus.enTrainDeTirer = false;
        }
    }  
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header ( " Parametre Mouvement")]
    public float gravite = -9.6f ;
    public float seuilAngle180 = 160f; // Angle demi tour
    private CharacterController controller;
    private Animator animator;
    private Transform camTransform;
    private Vector3 velocityY;
    public modeFocus modeFocus;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        if(Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
    }
    void Update()
    {
        CalculerMouvementEtEnvoyerAAnimator();
        AppliquerGravite();
        

    }

    void CalculerMouvementEtEnvoyerAAnimator()
    {
        if (modeFocus.focusActiver == true)
        {
            return;
        }
        // recuperer input player
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(inputHorizontal, 0f , inputVertical);

        float intensiteInput = Mathf.Clamp01(inputDirection.magnitude);
        float angleDelta = 0f;

        if(intensiteInput > 0.1)
        {
            Vector3 cameraForward = camTransform.forward;
            Vector3 cameraRight = camTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();   

            Vector3 directionCible = (cameraForward * inputVertical  + cameraRight * inputHorizontal).normalized;
            // ecart d'angle
            angleDelta = Vector3.SignedAngle(transform.forward, directionCible, Vector3.up); 

            // tourner sur place



            if( Mathf.Abs(angleDelta) >=  seuilAngle180 && animator.GetFloat("vitesse") > 0.5f)
            { 
                animator.SetTrigger("Action180");   
            }   

            

        }
        if ( intensiteInput > 0.85)
        {
            intensiteInput = 1f;
        }
        if (intensiteInput > 0.1)
        {
            animator.SetFloat("derniereVitesse", intensiteInput);   
        }
        // convertir l'angle pour le blend 2D
        float vitesseAnguaireCible = 0f;

        if(animator.GetFloat("vitesse") < 0.15f && Mathf.Abs(angleDelta) > 45f)
        {
            
        }

        if (intensiteInput > 0.1f)
        {
            vitesseAnguaireCible = Mathf.Clamp(angleDelta/90f , -1f , 1f);
        }

        animator.SetFloat("vitesse",intensiteInput, 0.1f , Time.deltaTime );
        animator.SetFloat("vitesseAngulaire", vitesseAnguaireCible , 0.1f , Time.deltaTime);


    }

    void AppliquerGravite()
    {
        if (controller.isGrounded)
        {
            if(velocityY.y < 0)
            {
                velocityY.y = -2f;
            }

        }
        velocityY.y += gravite * Time.deltaTime;


        controller.Move(new Vector3(0,velocityY.y,0) * Time.deltaTime);
    }

    void OnAnimatorMove()
    {
        bool estEnAction = !animator.GetCurrentAnimatorStateInfo(1).IsName("Rien");
        if ( animator == null)
        {
            return;
        }
        if (modeFocus !=null && modeFocus.focusActiver && !estEnAction)
        {
            return;
        }

        Vector3 rootMotionXZ = new Vector3(animator.deltaPosition.x, 0f , animator.deltaPosition.z);
        controller.Move(rootMotionXZ);
        transform.rotation *= animator.deltaRotation;



        
    }

    public void DeplacementStrafe(Vector3 direction, float vitesse)
    {
        controller.Move(direction * vitesse * Time.deltaTime);
    }
}
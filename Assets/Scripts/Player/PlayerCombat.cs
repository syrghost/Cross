using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{

    private Animator animator;
    public PlayerController playerController;
    private InputAction attaqueAction;
    public modeFocus modeFocus;

    [Header("Aimentation")]
    public LayerMask calqueEnnemis;
    private int layerAction;
    private Transform cibleVeroiller;
    public Transform CibleVeroiller => cibleVeroiller;
    public bool enTrainDattaquer => layerAction >= 0 && !animator.GetCurrentAnimatorStateInfo(layerAction).IsName("Rien");
    
    private bool peutEnchainer = true ;
    public inventaireManager arme ;


    public switchEmplacementArme switchEmplacementArme ;

    void Start()
    {
        animator = GetComponent<Animator>();
        layerAction = animator.GetLayerIndex("Action Layer");
        var controls= playerController.controls;
        attaqueAction = controls.Player.Attaque;

    }


    void Update()
    {
        if (attaqueAction.WasPressedThisFrame() &&  peutEnchainer )
        {
             cibleVeroiller = obtenirCible();


            if(cibleVeroiller != null && Vector3.Distance(transform.position, modeFocus.centreReel(cibleVeroiller)) <= arme.armeEquipeeActu.distanceLunge)
            {
                if (arme.armeEquipeeActu.monTypeArme== Arme.typeArme.Arc)
                {
                    return;
                }

                StartCoroutine(sequenceLunge(cibleVeroiller));
            }
            peutEnchainer = false ;
            animator.SetTrigger(arme.armeEquipeeActu.nomTriggerAnimator);
        }
    }

    Transform obtenirCible()
    {
        if( !arme.armeEquipeeActu.utiliserAimentation) return null ;

        // ennemis deja verroiller par mode focus 
        if(modeFocus != null && modeFocus.focusActiver && modeFocus.ennemiCible != null)
        {
            return modeFocus.ennemiCible;
        }
        return trouverCibleParCone();
    }
    Transform trouverCibleParCone()
    {
        Transform meilleurCible = null;
        float meilleurAngle = arme.armeEquipeeActu.angleDetectionAimentation;

        foreach(Transform ennemi in modeFocus.EnnemisDetectes)
        {
            if (ennemi == null)
            {
                continue;
            }
            Vector3 direction = modeFocus.centreReel(ennemi) - transform.position;
            direction.y = 0f;
            float distance = direction.magnitude;
            if(distance > arme.armeEquipeeActu.distanceLunge)
            {
                continue;
            }
            
            float angle = Vector3.Angle(transform.forward,direction);
            if (angle < meilleurAngle)
            {
                meilleurAngle = angle;
                meilleurCible = ennemi;
            }

        }  
        return meilleurCible;
    }

    IEnumerator sequenceLunge(Transform cible)
    {
       
        
        // deplacement vers la cible avec arret 
        Vector3 directionFinal =modeFocus.centreReel(cible) - transform.position;
        directionFinal.y = 0f;
        float distanceRestante = directionFinal.magnitude - arme.armeEquipeeActu.distanceArret;
        if (distanceRestante > 0f)
        {
            directionFinal.Normalize();
            float tempsLunge = 0f;
            while (tempsLunge < arme.armeEquipeeActu.dureeLunge)
            {
                float vitesse = distanceRestante / arme.armeEquipeeActu.dureeLunge;
                playerController.DeplacementLunge(directionFinal , vitesse);
                tempsLunge += Time.deltaTime ;
                yield return null ;
            }
        }
        

    }
    public void ActiverHitbox()
    {
        Hitbox hitboxActuelle = obtenirHitboxActu();
        if (hitboxActuelle == null) return;
        hitboxActuelle.degat = arme.armeEquipeeActu.degat;

        hitboxActuelle.Activer();

        Debug.Log("toucher avec " + arme.armeEquipeeActu.nomArme);
        Invoke(nameof(DesactiverHitbox),arme.armeEquipeeActu.dureeActivationHitbox);
    }
    void DesactiverHitbox()
    {
        Hitbox hitboxActuelle = obtenirHitboxActu();
        if (hitboxActuelle != null)
        {
            hitboxActuelle.Desactiver();
        }
        
    }

    public void autoriserEnchainement()
    {
        peutEnchainer = true ; 
    }

    Hitbox obtenirHitboxActu()
    {
        return switchEmplacementArme != null && switchEmplacementArme.armeData != null ? switchEmplacementArme.armeData.hitbox : null;
    }
}

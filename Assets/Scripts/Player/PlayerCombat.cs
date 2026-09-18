using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    private Animator animator;
    public Hitbox HitboxAttaque;
    public Arme armeEquipee;

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger(armeEquipee.nomTriggerAnimator);
        }
    }
    public void ActiverHitbox()
    {
        HitboxAttaque.degat = armeEquipee.degat;
        HitboxAttaque.Activer();
        Invoke(nameof(DesactiverHitbox),armeEquipee.dureeActivationHitbox);
    }
    void DesactiverHitbox()
    {
        HitboxAttaque.Desactiver();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    public inventaireManager inv;
    public switchEmplacementArme switchEmplacementArme;
    public PlayerController playerController;
    private InputAction equiperAction , utiliserMana;
    private Arme derniereArmeEquipee; 

    public manaJoueur  manaJoueur ;
    void Start()
    {
        var controls = playerController.controls;
        equiperAction = controls.Player.changerArme;
        utiliserMana = controls.Player.manaZone ;

    }

    void OnEnable()  { GestionnaireQuete.OnQueteChange += q => Debug.Log("Changée : " + q.titre); }

    void Update()
    {

        if (!equiperAction.WasPressedThisFrame()) return;

        if (inv.armeEquipeeActu != null)
        {
            derniereArmeEquipee = inv.armeEquipeeActu; // on retient avant de désequiper
            inv.desequiperArme();
        }
        else if (derniereArmeEquipee != inv.mainNue)
        {
            inv.equiperArme(derniereArmeEquipee);
        }


        if(utiliserMana.WasPressedThisFrame())
        {

            Debug.Log("jappuie sur X") ;
            manaJoueur.consommer(10f) ;
        }
    }
}
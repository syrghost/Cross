using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    public inventaireManager inv;
    public switchEmplacementArme switchEmplacementArme;
    public PlayerController playerController;
    private InputAction equiperAction;
    private Arme derniereArmeEquipee; 
    void Start()
    {
        var controls = playerController.controls;
        equiperAction = controls.Player.changerArme;
    }

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
    }
}
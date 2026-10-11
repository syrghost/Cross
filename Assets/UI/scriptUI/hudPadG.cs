using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class hudPadG : MonoBehaviour
{
    [Header("reference")]
    public inventaireManager inv;
    public PlayerController playerController;

    [Header("slot equipee")]
    public Image iconeArmeEquipee;

    [Header("liste")]
    public GameObject panelListe;
    public Transform conteneur;
    public slotConsommableUi prefabSlot;

    private InputAction ouvrirAction, precedentAction, suivantAction, validerAction, annulerAction;
    private bool listeOuverte;
    private int indexSelection;
    private Arme derniereArme;
    private List<slotConsommableUi> slotsUI = new List<slotConsommableUi>();

    void Start()
    {
        var controls = playerController.controls;
        ouvrirAction = controls.Player.ouvrirArme;
        precedentAction = controls.equipementArme.gauche;
        suivantAction = controls.equipementArme.droite;
        validerAction = controls.equipementArme.entrer;
        annulerAction = controls.equipementArme.retour;

        panelListe.SetActive(false);
        rafraichirIcone();
    }

    void OnDisable()
    {
        
        if (listeOuverte && playerController != null)
        {
            playerController.controls.equipementArme.Disable();
            playerController.controls.Player.Enable();
            listeOuverte = false;
        }
    }

    void Update()
    {
        if (!listeOuverte)
        {
           
            if (!playerController.controls.Player.enabled) return;

            bool occupe = playerController.inputBloquer
                || (playerController.playerCombat != null && playerController.playerCombat.enTrainDattaquer);

            if (ouvrirAction.WasPressedThisFrame() && !occupe && inv.armesPossedee.Count > 0)
                ouvrir();
        }
        else
        {
            gererNavigation();

            if (validerAction.WasPressedThisFrame()) valider();
            else if (annulerAction.WasPressedThisFrame()) fermer();
        }

        if (inv.armeEquipeeActu != derniereArme) rafraichirIcone();
    }

    void rafraichirIcone()
    {
        derniereArme = inv.armeEquipeeActu;
        bool afficher = derniereArme != null && derniereArme != inv.mainNue && derniereArme.icone != null;

        iconeArmeEquipee.enabled = afficher;
        if (afficher) iconeArmeEquipee.sprite = derniereArme.icone;
    }

    void ouvrir()
    {
        listeOuverte = true;
        playerController.controls.Player.Disable();
        playerController.controls.equipementArme.Enable();

        foreach (var s in slotsUI) Destroy(s.gameObject);
        slotsUI.Clear();

        indexSelection = Mathf.Max(0, inv.armesPossedee.IndexOf(inv.armeEquipeeActu));

        foreach (Arme arme in inv.armesPossedee)
        {
            slotConsommableUi ui = Instantiate(prefabSlot, conteneur);
            ui.Init(arme.icone, 0, false);
            slotsUI.Add(ui);
        }

        panelListe.SetActive(true);
        mettreAJourSurbrillance();
    }

    void gererNavigation()
    {
        int direction = 0;
        if (suivantAction.WasPressedThisFrame()) direction = 1;
        else if (precedentAction.WasPressedThisFrame()) direction = -1;
        if (direction == 0) return;

        indexSelection = Mathf.Clamp(indexSelection + direction, 0, slotsUI.Count - 1);
        mettreAJourSurbrillance();
    }

    void mettreAJourSurbrillance()
    {
        for (int i = 0; i < slotsUI.Count; i++)
            slotsUI[i].surbrillance(i == indexSelection);
    }

    void valider()
    {
        if (indexSelection >= 0 && indexSelection < inv.armesPossedee.Count)
            inv.GererClicArme(inv.armesPossedee[indexSelection]);

        fermer();
    }

    void fermer()
    {
        listeOuverte = false;
        panelListe.SetActive(false);
        playerController.controls.equipementArme.Disable();
        playerController.controls.Player.Enable();
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class hudPadB : MonoBehaviour
{
    [Header("reference")]
    public gestionnaireGrimoireJoueur grimoires;
    public PlayerController playerController;

    [Header("slot equipe")]
    public Image iconeGrimoireActuel;

    [Header("liste")]
    public GameObject panelListe;
    public Transform conteneur;
    public slotConsommableUi prefabSlot;

    private InputAction ouvrirAction, precedentAction, suivantAction, validerAction, annulerAction;
    private bool listeOuverte;
    private int indexSelection;
    private grimoire dernierGrimoire;
    private List<slotConsommableUi> slotsUI = new List<slotConsommableUi>();

    void Start()
    {
        var controls = playerController.controls;
        ouvrirAction = controls.Player.ouvrirGrimoire;   // nouvelle action
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

            if (ouvrirAction.WasPressedThisFrame() && !occupe && grimoires.grimoireConnus.Count > 0)
                ouvrir();
        }
        else
        {
            gererNavigation();

            if (validerAction.WasPressedThisFrame()) valider();
            else if (annulerAction.WasPressedThisFrame()) fermer();
        }

        if (grimoires.grimoireActuelle != dernierGrimoire) rafraichirIcone();
    }

    void rafraichirIcone()
    {
        dernierGrimoire = grimoires.grimoireActuelle;
        bool afficher = dernierGrimoire != null && dernierGrimoire.iconGrimoire != null;

        iconeGrimoireActuel.enabled = afficher;
        if (afficher) iconeGrimoireActuel.sprite = dernierGrimoire.iconGrimoire;
    }

    void ouvrir()
    {
        listeOuverte = true;
        playerController.controls.Player.Disable();
        playerController.controls.equipementArme.Enable();

        foreach (var s in slotsUI) Destroy(s.gameObject);
        slotsUI.Clear();

        indexSelection = grimoires.indexActuelPublic;

        foreach (grimoire g in grimoires.grimoireConnus)
        {
            slotConsommableUi ui = Instantiate(prefabSlot, conteneur);
            ui.Init(g.iconGrimoire, 0, false);
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
        grimoires.choisir(indexSelection);
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
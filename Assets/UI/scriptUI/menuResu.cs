using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class menuResu : MonoBehaviour
{
    public inventaireManager inv;
    public PlayerController playerController;

    [Header("etape 1 : question")]
    public GameObject panelQuestion;
    public GameObject surbrillanceOui;
    public GameObject surbrillanceNon;

    [Header("etape 2 : liste")]
    public GameObject panelListe;
    public Transform conteneur;
    public slotConsommableUi prefabSlot;

    private enum Etape { Question, Liste }
    private Etape etape;
    private bool ouvert;
    private int index;
    private VieJoueur vie;
    private List<slotConsommable> potions;
    private List<slotConsommableUi> slotsUI = new List<slotConsommableUi>();

    public Color couleurSelection = Color.blue ;
    public Color couleurNormale = Color.white ;
    public TMP_Text oui ;
    public TMP_Text non ;


    void Start()
    {
        panelQuestion.SetActive(false);
        panelListe.SetActive(false);
        surbrillanceOui.SetActive(false);
        surbrillanceNon.SetActive(false) ;
    }

    public void ouvrir(VieJoueur v)
    {
        vie = v;
        ouvert = true;
        Time.timeScale = 0f;
        playerController.controls.Player.Disable();
        playerController.controls.equipementArme.Enable();
        afficherQuestion();
    }

    void Update()
    {
        if (!ouvert) return;
        var c = playerController.controls.equipementArme;

        int dir = 0;
        if (c.droite.WasPressedThisFrame()) dir = 1;
        else if (c.gauche.WasPressedThisFrame()) dir = -1;

        if (dir != 0)
        {
            int max = (etape == Etape.Question) ? 2 : slotsUI.Count;
            index = Mathf.Clamp(index + dir, 0, max - 1);
            surbrillance();
        }

        if (c.entrer.WasPressedThisFrame()) valider();
        else if (c.retour.WasPressedThisFrame()) retour();
    }

    void afficherQuestion()
    {
        etape = Etape.Question;
        index = 0; // 0 = Oui, 1 = Non
        panelListe.SetActive(false);
        panelQuestion.SetActive(true);
        surbrillanceOui.SetActive(true);
        surbrillanceNon.SetActive(true) ;
        surbrillance();
    }

    void afficherListe()
    {
        etape = Etape.Liste;
        index = 0;
        panelQuestion.SetActive(false);
        panelListe.SetActive(true);

        foreach (var s in slotsUI) Destroy(s.gameObject);
        slotsUI.Clear();

        potions = inv.potionResuDisponible();
        foreach (slotConsommable s in potions)
        {
            slotConsommableUi ui = Instantiate(prefabSlot, conteneur);
            ui.Init(s.objet.imageConsommable, s.quantiter, true);
            slotsUI.Add(ui);
        }
        surbrillance();
    }

    void surbrillance()
    {
        if (etape == Etape.Question)
        {
            oui.color = index == 0 ? couleurSelection :   couleurNormale ;
            non.color = index ==1 ? couleurSelection :   couleurNormale ;
            oui.text = (index == 0 ? "> " : "  ") + "Oui";
            non.text = (index == 1 ? "> " : "  ") + "Non";
        }
        else if (etape == Etape.Liste)
        {
            surbrillanceOui.SetActive(false);
            surbrillanceNon.SetActive(false);
        }
    }

    void valider()
    {
        if (etape == Etape.Question)
        {
            if (index == 0) afficherListe();
            else refuser();
        }
        else
        {
            Consommable choisie = potions[index].objet;
            fermer();                          // remet timeScale à 1 AVANT de ressusciter
            inv.utiliserResu(choisie, vie);
        }
    }

    void retour()
    {
        if (etape == Etape.Liste) afficherQuestion();
        else refuser();
    }

    void refuser()
    {
        fermer();
        vie.gameOver();
    }

    void fermer()
    {
        ouvert = false;
        Time.timeScale = 1f;
        panelQuestion.SetActive(false);
        panelListe.SetActive(false);
        surbrillanceOui.SetActive(false);
        surbrillanceNon.SetActive(false) ;
        playerController.controls.equipementArme.Disable();
        playerController.controls.Player.Enable();
    }

    void OnDisable()
    {
        if (ouvert) { Time.timeScale = 1f; ouvert = false; }
    }
}
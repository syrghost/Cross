using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UIInventaireToolkit : MonoBehaviour
{
    [Header("References")]
    public inventaireManager inv;
    public VisualTreeAsset boutonArmeTemplate; 
    private VisualElement contenuListe; 
    
    private VisualElement root; 
    private bool inventaireActiver = false;
    public PlayerController playerController;
    
    private InputAction inventaireAction;
    private InputAction precedentsAction; 
    private InputAction suivantsAction;   
    private playerControls controls; 

    // Gestion des onglets / catégories
    private enum Categorie { Armes, Armures }
    private Categorie categorieActuelle = Categorie.Armes;

    void OnEnable()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        contenuListe = root.Q<VisualElement>("contenuListe");
    }
 
    void Start()
    {
        root.style.display = DisplayStyle.None;
        inventaireActiver = false;
        controls = playerController.controls;
        
        inventaireAction = controls.Player.Inventaire;
        precedentsAction = controls.Ui.precedents;
        suivantsAction = controls.Ui.suivants;

        controls.Ui.Disable(); 
    }

    void Update()
    {
        if (inventaireAction.WasPressedThisFrame() && !inventaireActiver)
        {
            inventaireActiver = true;
            root.style.display = DisplayStyle.Flex;
            
            categorieActuelle = Categorie.Armes; 
            ActualiserListe();
            
            controls.Player.Disable();
            controls.Ui.Enable();
            DonnerFocusInitial();
        }
        else if (inventaireActiver)
        {
            if (controls.Ui.retour.WasPressedThisFrame())
            {
                inventaireActiver = false;
                root.style.display = DisplayStyle.None;
                controls.Ui.Disable();
                controls.Player.Enable();
                return;
            }

            if (precedentsAction.WasPressedThisFrame())
            {
                categorieActuelle = Categorie.Armes;
                ActualiserListe();
                DonnerFocusInitial();
            }
            else if (suivantsAction.WasPressedThisFrame())
            {
                categorieActuelle = Categorie.Armures;
                ActualiserListe();
                DonnerFocusInitial();
            }
        }
    }

    void DonnerFocusInitial()
    {
        Button premierBouton = contenuListe.Q<Button>("boutonArme");
        premierBouton?.Focus();
    }

    public void ActualiserListe()
    {
        if (contenuListe == null) return;

        contenuListe.Clear();
 
        if (categorieActuelle == Categorie.Armes)
        {
            // --- AFFICHAGE DES ARMES ---
            foreach (Arme arme in inv.armesPossedee)
            {
                TemplateContainer nouveauBouton = boutonArmeTemplate.Instantiate();
     
                Label texte = nouveauBouton.Q<Label>("texteNomArme");
                if (texte != null) texte.text = arme.nomArme;
     
                Arme armeCapturee = arme;
                Button bouton = nouveauBouton.Q<Button>("boutonArme");
                if (bouton != null)
                {
                    bouton.focusable = true; 
                    // Utilisation de la méthode de bascule
                    bouton.clicked += () => inv.GererClicArme(armeCapturee);
                }

                contenuListe.Add(nouveauBouton);
            }
        }
        else if (categorieActuelle == Categorie.Armures)
        {
            // --- AFFICHAGE DES ARMURES ---
            foreach (Armure armure in inv.armurePossedee) // Correction du 's' ici
            {
                TemplateContainer nouveauBouton = boutonArmeTemplate.Instantiate();
     
                Label texte = nouveauBouton.Q<Label>("texteNomArme");
                if (texte != null) texte.text = armure.nomArmure;
     
                Armure armureCapturee = armure;
                Button bouton = nouveauBouton.Q<Button>("boutonArme");
                if (bouton != null)
                {
                    bouton.focusable = true; 
                    // Utilisation de la méthode de bascule
                    bouton.clicked += () => inv.GererClicArmure(armureCapturee);
                }

                contenuListe.Add(nouveauBouton);
            }
        }
    }
}
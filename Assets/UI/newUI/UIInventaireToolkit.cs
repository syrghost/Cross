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
    private playerControls controls; 

    void OnEnable()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        contenuListe = root.Q<VisualElement>("contenuListe");

        ActualiserListe();
    }
 
    void Start()
    {
        root.style.display = DisplayStyle.None;
        inventaireActiver = false;
        controls = playerController.controls;
        inventaireAction = controls.Player.Inventaire;

        controls.Ui.Disable(); // au repos, la map Ui n'a pas besoin d'être active
    }


    void Update()
    {
        if (inventaireAction.WasPressedThisFrame() && !inventaireActiver)
        {
            // Ouverture
            inventaireActiver = true;
            root.style.display = DisplayStyle.Flex;
            ActualiserListe();
            controls.Player.Disable();
            controls.Ui.Enable();
            DonnerFocusInitial();
    }
        else if (inventaireActiver && controls.Ui.retour.WasPressedThisFrame())
        {
            // Fermeture via Cancel/Retour dans la map Ui
            inventaireActiver = false;
            root.style.display = DisplayStyle.None;
            controls.Ui.Disable();
            controls.Player.Enable();
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
 
        foreach (Arme arme in inv.armesPossedee)
        {
            TemplateContainer nouveauBouton = boutonArmeTemplate.Instantiate();
 
            Label texte = nouveauBouton.Q<Label>("texteNomArme");
            if (texte != null)
            {
                texte.text = arme.nomArme;
            }
 
            Arme armeCapturee = arme;
 
            Button bouton = nouveauBouton.Q<Button>("boutonArme");
            if (bouton != null)
            {
                bouton.focusable = true; 
                bouton.clicked += () => inv.equiperArme(armeCapturee);
            }

            contenuListe.Add(nouveauBouton);
        }
    }
}
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class uiBoutique : MonoBehaviour
{
    public static uiBoutique instance;

    public GameObject panneau ;

    public Image interagir , argent ;
    public TMP_Text texteTitre, texteListe, texteArgent, texteMessage;
    public PlayerController playerController;
    public inventaireManager inv;

    InputAction haut, bas, acheter, retour;
    boutique actuelle;
    bool ouverte;
    int index, frameOuverture;

    void Awake()     { instance = this; }
    void OnEnable()  { portefeuille.OnArgentChange += MajArgent; }
    void OnDisable() { portefeuille.OnArgentChange -= MajArgent; }

    void Start()
    {
        panneau.SetActive(false);
        var controls = playerController.controls;
        haut = controls.Dialogue.oui;
        bas = controls.Dialogue.non;
        acheter = controls.Dialogue.suivant;
        retour = controls.Dialogue.retour;
    }

    public void Ouvrir(boutique b)
    {
        if (b == null || b.articles.Length == 0) return;

        actuelle = b;
        index = 0;
        texteTitre.text = b.nomBoutique;
        texteMessage.text = "";
        MajArgent(portefeuille.instance.Argent);
        Rafraichir();

        panneau.SetActive(true);
        argent.gameObject.SetActive(false);
        interagir.gameObject.SetActive(false);
        ouverte = true;
        frameOuverture = Time.frameCount;
        dialogueManager.instance.garderLeControl = true;
    }

    void Update()
    {
        if (!ouverte || Time.frameCount <= frameOuverture) return;

        int n = actuelle.articles.Length;

        if (haut.WasPressedThisFrame())         { index = (index - 1 + n) % n; Rafraichir(); }
        else if (bas.WasPressedThisFrame())     { index = (index + 1) % n; Rafraichir(); }
        else if (acheter.WasPressedThisFrame()) Acheter();
        else if (retour.WasPressedThisFrame())  Fermer();
    }

    void Acheter()
    {
        resultatAchat r = gestionnaireBoutique.instance.acheter(actuelle.articles[index]);

        texteMessage.text = r switch
        {
            resultatAchat.ok             => "Achat effectué !",
            resultatAchat.pasAssezArgent => "Pas assez d'argent.",
            resultatAchat.plein          => "Tu en as déjà le maximum.",
            _                            => "Tu possèdes déjà cet objet."
        };
        Rafraichir();
    }

    void Rafraichir()
    {
        var sb = new StringBuilder();

        for (int i = 0; i < actuelle.articles.Length; i++)
        {
            articleBoutique a = actuelle.articles[i];

            string ligne = NomObjet(a.objet);
            if (!inv.peutRecevoir(a.objet))
                ligne += a.objet is Consommable ? " (max)" : " (possédé)";
            ligne += "<pos=75%>" + a.prix;

            sb.AppendLine(i == index ? "<color=yellow>> " + ligne + "</color>" : "  " + ligne);
        }
        texteListe.text = sb.ToString();
    }

    void MajArgent(int montant)
    {
        texteArgent.text = "Argent : " + montant;
    }

    void Fermer()
    {
        ouverte = false;
        panneau.SetActive(false);
        dialogueManager.instance.rendreControl();
        
        argent.gameObject.SetActive(true);
        interagir.gameObject.SetActive(true);
    }

    string NomObjet(ScriptableObject o)
    {
        if (o is Arme a)    return a.nomArme;
        if (o is Armure ar) return ar.nomArmure;
        return o.name;   // consommable : remplace par son champ de nom si tu en as un
    }
}
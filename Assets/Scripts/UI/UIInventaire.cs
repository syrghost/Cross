using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventaire : MonoBehaviour
{
    [Header("References")]
    public inventaireManager inv;
    public GameObject boutonArmePrefab;
    public Transform contenuListe;
 
    void Start()
    {
        ActualiserListe();
    }
 
    public void ActualiserListe()
    {
        // 1. Vider la liste actuelle avant de la reconstruire
        foreach (Transform enfant in contenuListe)
        {
            Destroy(enfant.gameObject);
        }
 
        // 2. Creer un bouton pour chaque arme possedee
        foreach (Arme arme in inv.armesPossedee)
        {
            GameObject nouveauBouton = Instantiate(boutonArmePrefab, contenuListe);
 
            // Changer le texte affiche sur le bouton
            TMP_Text texte = nouveauBouton.GetComponentInChildren<TMP_Text>();
            if (texte != null)
            {
                texte.text = arme.nomArme;
            }
 
            // Copie locale obligatoire : sans ca, tous les boutons
            // equiperaient la derniere arme de la liste
            Arme armeCapturee = arme;
 
            Button bouton = nouveauBouton.GetComponent<Button>();
            bouton.onClick.AddListener(() => inv.equiperArme(armeCapturee));
        }
    }
}

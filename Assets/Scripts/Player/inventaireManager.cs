using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventaireManager : MonoBehaviour

{
    [Header("reference")]
    public gestionnaireArmeJoueur gestionnaireArme;
    public switchEmplacementArme switchEmplacementArme;

     

    [Header("Donnee")]
    public List<Arme> armesPossedee = new List<Arme>();
    public Arme armeEquipeeActu;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ajouterArme(Arme arme)
    {
        if (armesPossedee.Contains(arme))
        {
            Debug.LogWarning("Arme deja poseder" + arme.nomArme);
            return;
        }
        armesPossedee.Add(arme);
    }

    public void equiperArme(Arme arme)
    {
        if(!armesPossedee.Contains(arme))
        {
            Debug.LogWarning("impossible d'equiper une arme non possedee " + arme.nomArme);
            return;
        }
        gestionnaireArme.equiperVisuel(arme);

        GameObject nouveauGameObject = gestionnaireArme.obtenirGameObjectArmeEquipee();

        switchEmplacementArme.definirArmeActu(nouveauGameObject);
        armeEquipeeActu = arme;
    }
    public void desequiperArme()
    {
        Debug.Log("desequiper");
        if(armeEquipeeActu == null)
        {
            Debug.Log("armeEquipeeActu est null, on sort");
            return;
        }
        Debug.Log("on continue, on masque l'arme");
        gestionnaireArme.masquerArmeEquipee();
        switchEmplacementArme.definirArmeActu(null);

        armeEquipeeActu= null;
        switchEmplacementArme.armeEnMain = false;
        Debug.Log("fin de la fonction ");
    }
}

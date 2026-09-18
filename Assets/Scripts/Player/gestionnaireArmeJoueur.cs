using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class gestionnaireArmeJoueur : MonoBehaviour
{
    [Header("reference")]
    public Transform armesJoueurParents ;
    public Transform slotRanger;
    private Dictionary<Arme,armeData> registresArmes = new Dictionary<Arme, armeData>();
    private armeData armeEquipeeActu;
    private GameObject AttachementDos;
    private GameObject AttachementCeinture;
    private Transform transformAttachementDos;
    private Transform transformAttachementCeinture;
    
    
    void Start()
    {
       AttachementDos = GameObject.FindWithTag("AttachementDos");
       AttachementCeinture = GameObject.FindWithTag("AttachementCeinture");
        if(AttachementDos != null)
        {
            transformAttachementDos = AttachementDos.transform;

        }
        if (AttachementCeinture != null)
        {
            transformAttachementCeinture = AttachementCeinture.transform;
        }

        construireRegistre();
    }

    
    void Update()
    {
        
    }

    Transform obtenirSlotPourType(Arme.typeArme type)
    {
        switch (type)
        {
            case Arme.typeArme.Arc:
            case Arme.typeArme.epeeUneMain:
                return transformAttachementCeinture;
            default:
                return transformAttachementDos;
        }
    }

    void construireRegistre()
    {
        armeData[] toutesLesArmes = armesJoueurParents.GetComponentsInChildren<armeData>(true);
        foreach(armeData donnee in toutesLesArmes)
        {
            if(donnee.arme == null)
            {
                Debug.LogWarning("arme sans reference : " + donnee.gameObject.name);
                continue;
            }
            registresArmes[donnee.arme] = donnee;
        }
    }

    public void equiperVisuel(Arme nouvelleArme)
    {
         if(!registresArmes.ContainsKey(nouvelleArme))
        {
            Debug.LogWarning("Arme non trouver dans le registre" + nouvelleArme.nomArme);
            return;
        }
        // cacher l'arme si une equipee 
        if(armeEquipeeActu != null)
        {
            definirVisibilite(armeEquipeeActu , false);
        }

        // Position et affichage ( pas en main hein )
        armeData nouvelleDonne = registresArmes[nouvelleArme];
        Arme.typeArme typeArme = nouvelleDonne.arme.monTypeArme;
        slotRanger = obtenirSlotPourType(typeArme);

        nouvelleDonne.transform.SetParent(slotRanger);
        


        definirVisibilite(nouvelleDonne, true);

        armeEquipeeActu = nouvelleDonne;
    }

    void definirVisibilite(armeData donnee , bool visible)
    {
        foreach(Renderer r in donnee.renderers)
        {
            r.enabled = visible;
        }
    }

    public GameObject obtenirGameObjectArmeEquipee()
    {
        return armeEquipeeActu != null ? armeEquipeeActu.gameObject : null ;
    }

    public void masquerArmeEquipee()
    {
        if (armeEquipeeActu != null)
        {
            definirVisibilite(armeEquipeeActu,false);
            armeEquipeeActu = null;
        } 
    }
}



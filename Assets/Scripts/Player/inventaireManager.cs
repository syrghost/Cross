using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class slotConsommable
{
    public Consommable objet;
    public int quantiter;
}
public class inventaireManager : MonoBehaviour

{
    [Header("reference")]
    public gestionnaireArmeJoueur gestionnaireArme;
    public gestionaireArmureJoueur gestionaireArmure;
    public switchEmplacementArme switchEmplacementArme;

     

    [Header("Donnee")]
    public List<Arme> armesPossedee = new List<Arme>();
    public List<Armure> armurePossedee = new List<Armure>();
    public List<slotConsommable>slots = new List<slotConsommable>();

    public Arme armeEquipeeActu;
    public Arme mainNue;

    [Header("consommable")]
    public Consommable consommableEquiper;

    public GameObject player;
    void Start()
    {
        
    }

    void Update()
    {
       
        
        if ( armeEquipeeActu == null )
        {
            equiperArme(mainNue);
        }




        
    }
//-------------------------------------------POUR LES ARMES -----------------------------------------
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
            if (arme != mainNue)
            {
                Debug.LogWarning("impossible d'equiper une arme non possedee " + arme.nomArme);
                return;
            }
            
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
        if (armeEquipeeActu.monTypeArme == Arme.typeArme.mainNues) return;
        
        Debug.Log("on continue, on masque l'arme");
        gestionnaireArme.masquerArmeEquipee();
        switchEmplacementArme.definirArmeActu(null);

        armeEquipeeActu= mainNue;
        switchEmplacementArme.armeEnMain = false;
        Debug.Log("fin de la fonction ");
    }

// --------------------------------------- POUR LES ARMURES ----------------------------

    public void ajouterArmure ( Armure armure)
    {
        if (armurePossedee.Contains(armure))
        {
            Debug.Log("armure deja possede " + armure.nomArmure);
            return;
        }
        armurePossedee.Add(armure);
    }

    public void equiperArmure(Armure armure)
    {
        if (!armurePossedee.Contains(armure))
        {
            Debug.LogWarning("impossible d'equiper une armure non posseder"  + armure.nomArmure);
            return;
        }

        gestionaireArmure.equiper(armure);
    }


    public void desequiperArmure(Armure.typeArmure type)
    {
        gestionaireArmure.desequiper(type);
    }


    // ----------------------------------POUR LES CONSOMMABLE-----------------------------------------
    public int quantiterConsommable(Consommable objet )
    {
        slotConsommable slot = slots.Find (s => s.objet == objet);
        return slot != null ? slot.quantiter : 0 ;
    } 

    public int ajouterConsommable(Consommable objet , int nombre = 1)
    {
        slotConsommable slot = slots.Find(s=> s.objet == objet);

        if (slot == null)
        {
            slot = new slotConsommable{ objet = objet , quantiter = 0};
            slots.Add(slot);
        }

        int place = objet.quantiterMax - slot.quantiter ;
        int ajoute = Mathf.Min(nombre,place);
        slot.quantiter += ajoute ;
        return ajoute ;

    }
    public bool utiliserConsommable( Consommable objet)
    {
        slotConsommable slot = slots.Find(s=> s.objet == objet);

        if (slot.quantiter <= 0 || slot == null) return false ;

        slot.quantiter -- ;

        Debug.Log("quantiter de potion restante " + slot.quantiter);

        if(slot.quantiter == 0) slots.Remove(slot);

        // pour les effets 
        VieJoueur vieJoueur = player.GetComponent<VieJoueur>();
        Animator animator = player.GetComponent<Animator>();

        switch (objet.tuConsomme)
        {
            case Consommable.typeConsommable.potionSoin :

                if (vieJoueur == null)
                {
                    return false ;
                }
                if(animator == null) return false;

                
                animator.SetTrigger("soigner");
                vieJoueur.Soigner(objet.quantiterSoinRecu);
                break;
            case Consommable.typeConsommable.potionMana :
                break;

            case Consommable.typeConsommable.potionResu :
                break;
            
        }


        return true;
    }

    public void equiperConsommable(Consommable objet)
    {
        if(quantiterConsommable(objet) <= 0) return ;
        consommableEquiper = objet ;
    }

    public bool utiliserConsommableEquipee()
    {
        if (consommableEquiper == null ) return false ;

        bool reussi = utiliserConsommable(consommableEquiper);

        if (quantiterConsommable(consommableEquiper)<= 0)
        {
            consommableEquiper = null;
        }
        return reussi ;
    }

    public Consommable obtenirConsommable(int index)
    {
        if(index < 0 || index >= slots.Count) return null ;
        return slots[index].objet ;

    }
   


// TESTE UI
    // Permet de basculer entre équiper et déséquiper une arme au clic
    public void GererClicArme(Arme arme)
    {
        if (armeEquipeeActu == arme && arme != mainNue)
        {
            desequiperArme();
        }
        else
        {
            equiperArme(arme);
        }
    }

    // Permet de basculer entre équiper et déséquiper une armure au clic
    public void GererClicArmure(Armure armure)
    {
        // On vérifie si cette armure exacte est déjà équipée sur son slot
        if (gestionaireArmure.EstArmureEquipee(armure))
        {
            gestionaireArmure.desequiper(armure.emplacementArmure);
        }
        else
        {
            gestionaireArmure.equiper(armure);
        }
    }



}

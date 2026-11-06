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

    public gestionnaireGrimoireJoueur gestionnaireGrimoire ;




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
        if(objet.utilisableManuellement == false) return false ;
        slotConsommable slot = slots.Find(s=> s.objet == objet);

        if ( slot == null || slot.quantiter <= 0 ) return false ;

        slot.quantiter -- ;

        Debug.Log("quantiter de potion restante " + slot.quantiter);

        if(slot.quantiter == 0) slots.Remove(slot);

        // pour les effets 
        VieJoueur vieJoueur = player.GetComponent<VieJoueur>();
        manaJoueur manaJoueur = player.GetComponent<manaJoueur>();
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
                if(manaJoueur == null)
                {
                    return false ;
                }
                if(animator == null) return false ;
                animator.SetTrigger("soigner") ;
                manaJoueur.recuperer(objet.quantiterManaRecu);
                break;

            case Consommable.typeConsommable.potionResu :
                break;
            
        }


        return true;
    }

    public void equiperConsommable(Consommable objet)
    {
        if(objet.utilisableManuellement == false) return ;
        if(quantiterConsommable(objet) <= 0) return ;
        
        consommableEquiper = objet ;
    }

    public bool utiliserConsommableEquipee()
    {
        

        if (consommableEquiper == null ) return false ;
        if(consommableEquiper.utilisableManuellement == false) return false;

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

//------------------------------ SI LE CONSOMMABLE EST UNE POTION RESU -----------------------

    public List<slotConsommable> potionResuDisponible()
    {
        return slots.FindAll( s => s.objet.tuConsomme == Consommable.typeConsommable.potionResu && s.quantiter >0) ; 
    }

    public bool utiliserResu(Consommable potion , VieJoueur vie)
    {
        slotConsommable slot = slots.Find(s => s.objet == potion && s.quantiter>0);
        if(slot == null ) return false ;

        slot.quantiter -- ;
        if(slot.quantiter <= 0)
        {
            slots.Remove(slot);
        }
        vie.ressuciter(potion.pourcentageVieResu);
        return true ;
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


// ------------------POUR LES RECOMPENSE DE QUETE ------------------------------------
    void recevoirRecompense(recompense r)
    {

        switch (r.type)
        {
            case typeRecompense.objet :
                if(r.objet is Consommable c)
                {
                    int ajoute = ajouterConsommable(c , r.quantiter);
                    if(ajoute < r.quantiter) Debug.LogWarning("inventaire plein") ;

                    
                }
                else if(r.objet is Arme a)
                {
                    ajouterArme(a) ;
                } 

                else if (r.objet is Armure A)
                {
                    ajouterArmure(A);
                }
                else if (r.objet is grimoire g)
                {
                    ajouterGrimoire( g) ;
                }
                break ;
            // -----------------POUR D'AUTRE RECOMPENSE -----------------
            case typeRecompense.argent :
                break ;
        } 
            
        
    }

    void OnEnable()
    {
        GestionnaireQuete.OnRecompense += recevoirRecompense;
    }

    void OnDisable()
    {
        GestionnaireQuete.OnRecompense -= recevoirRecompense ;
    }

    //pour les marchand 
    public bool peutRecevoir(ScriptableObject objet)
    {
        if(objet is Consommable c) return quantiterConsommable(c) < c.quantiterMax ;

        if(objet is Arme a) return !armesPossedee.Contains(a);
        if(objet is Armure A) return !armurePossedee.Contains(A);
        if(objet is grimoire g ) return !gestionnaireGrimoire.grimoireConnus.Contains(g);

        return false ;


    }
    public void recevoir(ScriptableObject objet)
    {
        if(objet is Consommable c) ajouterConsommable(c,1);
        else if (objet is Arme a) ajouterArme(a);
        else if(objet is Armure A) ajouterArmure(A);
        else if (objet is grimoire g) ajouterGrimoire(g);

    }


//-----------------------------------------------POUR LES GRIMOIRE ----------------------------------
    public void ajouterGrimoire( grimoire g)
    {
        gestionnaireGrimoire.ajouterGrimoire(g);
    }

}

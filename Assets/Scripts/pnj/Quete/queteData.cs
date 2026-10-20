using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class queteData : MonoBehaviour
{

    public quete  quete;
    public etatQuete etat = etatQuete.nonCommencer ;

    List<string> dejaCompter = new List<string>();


    int[] progressions ; // une progression par objectif 

    void OnEnable()
    {
        
        evenementsJeu.onEvenement += surEvenement ;


    }


    void OnDisable()
    {
        
        evenementsJeu.onEvenement -= surEvenement ;

    }


    void surEvenement( typeObjectif type , string id)
    {
        if(etat != etatQuete.enCours) return ;

        for(int i = 0  ; i < quete.objectifs.Length ; i++)
        {
            Objectif o = quete.objectifs[i];

            if(type != o.type) continue ;

            if( ! string.IsNullOrEmpty(o.idCible) && o.idCible != id) continue ;
            if(progressions[i] >= o.nombreRequis ) continue ;

            progressions[i] ++ ;

        }

        if (tousTerminer() ) etat = etatQuete.terminer ; 


    }

    public void CommencerQuete()
    {
        etat = etatQuete.enCours ;
        progressions = new int[ quete.objectifs.Length] ;

        dejaCompter.Clear() ;
    }

    bool tousTerminer()
    {
        for(int i =0 ; i < quete.objectifs.Length; i++)
        {
            if(progressions[i] < quete.objectifs[i].nombreRequis) return false ;

        }
        return true ;
    }




    public string[] dialogueActuelle()
    {
        switch (etat)
        {
            case etatQuete.nonCommencer :
                return quete.dialogueAvant ;
            
            case etatQuete.enCours :
                return quete.dialoguePendant ;

            default :  return quete.dialogueApres ; 
        }
        
    }
}

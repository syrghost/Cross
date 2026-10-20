using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class GestionnaireQuete : MonoBehaviour
{

    public static GestionnaireQuete instance ;

    public Dictionary< quete , SuiviQuete> suivis = new Dictionary<quete, SuiviQuete>();

    public static event Action<quete> OnQueteChange ; 
    public static event Action <recompense> OnRecompense ; 



    void Awake()
    {
        instance = this ;
    }
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
        foreach ( var pair in suivis)
        {
            quete q = pair.Key;
            SuiviQuete s = pair.Value ;


            if(s.etat != etatQuete.enCours ) continue ;

            bool change = false ;

            for(int i=0; i< q.objectifs.Length ;i++)
            {
                Objectif o = q.objectifs[i] ;
                if(type != o.type) continue ;

                if( !string.IsNullOrEmpty(o.idCible) && o.idCible != id ) continue ;

                if (s.progression[i] >= o.nombreRequis)
                {
                    continue ;
                }

                s.progression[i] ++ ;
                change = true ;

            }
            if(tousTerminer(q,s)) s.etat = etatQuete.terminer ; change = true ;
            if(change)  OnQueteChange?.Invoke(q) ;
        }
    }

    SuiviQuete Obtenir(quete q)
    {
        if( ! suivis.TryGetValue(q, out SuiviQuete s))
        {
            s = new SuiviQuete ();
            suivis[q] = s ;

        }
        return s ;
    }

    public etatQuete Etat(quete q)
    {
        return Obtenir(q).etat ;
    }

    public void Demarrer(quete q)
    {
        SuiviQuete s = Obtenir(q) ;
        s.etat = etatQuete.enCours ;
        s.progression = new int [q.objectifs.Length] ;

        OnQueteChange?.Invoke(q) ;
    }

    bool tousTerminer(quete q , SuiviQuete s)
    {
        for(int i = 0 ; i< q.objectifs.Length; i++)
        {
            if(s.progression[i] < q.objectifs[i].nombreRequis) return false ;
        }
        return true ;
    }

    public void rendre(quete q)
    {
        SuiviQuete s= Obtenir(q);
        if(s.etat != etatQuete.terminer )  return ;
        
        s.etat = etatQuete.rendu ;

        foreach(recompense r in q.recompense)
        {
            OnRecompense?.Invoke(r) ;
        }
        OnQueteChange?.Invoke(q) ;
    }

    public bool prerequisOk( quete q)
    {
        if(q.prerequis == null ) return true ;

        foreach(quete p in q.prerequis)
        {
            if(p == null) continue ;
            if(Etat(p) != etatQuete.rendu) return false ;
        }
        return true ;
    }

    public int Progression(quete q , int i )
    {

        SuiviQuete s = Obtenir(q) ;
        return s.progression == null ? 0 : s.progression[i];
    }

    public List<quete> quetesActives()
    {
        var liste = new List<quete>();

        foreach(var paire in suivis)
        {
            etatQuete e = paire.Value.etat ;

            if(e == etatQuete.enCours  || e == etatQuete.terminer)
            {
                liste.Add(paire.Key);
            }
        }
        return liste ;
    }



}

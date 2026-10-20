using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portefeuille : MonoBehaviour
{
    public static portefeuille instance ;
    public static event System.Action<int> OnArgentChange ;

    [SerializeField] int argent ;
    public int Argent => argent ;

    void Awake()
    {
        instance = this ;
    }
    void Start()
    {
        OnArgentChange?.Invoke(argent) ;
    }
    void OnEnable()
    {
        GestionnaireQuete.OnRecompense += argentRecompenseQuete;
    }
    void OnDisable()
    {
        GestionnaireQuete.OnRecompense -= argentRecompenseQuete;
    }

    void argentRecompenseQuete(recompense r )
    {
        if(r.type == typeRecompense.argent) ajouter(r.quantiter) ;
    }

    public void ajouter(int montant)
    {
        if(montant <=0) return ;
        argent += montant ;
        OnArgentChange?.Invoke(argent);
    }

    public bool depenser(int montant )
    {
        if(montant <= 0 || argent < montant) return false ;
        argent -= montant ;
        OnArgentChange?.Invoke(argent);
        return true ;


    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manaJoueur : MonoBehaviour
{

    public float manaMax = 100f ;

    public float manaActuelle { get ; private set ;}
    
    [Header("regeneration")]

    public float regenParSeconde = 10f ;
    public float delaiRegen = 5f ;
    private float timerRegen ;

    void Start()
    {
        manaActuelle = manaMax ;

    }

    void Update()
    {
        if(timerRegen > 0)
        {
            timerRegen -= Time.deltaTime ;
            return ;

        }

        manaActuelle = Math.Min(manaActuelle + regenParSeconde * Time.deltaTime , manaMax ) ;

    }

    public bool consommer(float quantite)
    {
        if(manaActuelle < quantite) return false  ;
        
        manaActuelle -= quantite ;
        timerRegen = delaiRegen  ;
        return true ;

    }

    public void recuperer(float quantiter)
    {
        manaActuelle = Mathf.Min(manaActuelle+ quantiter , manaMax) ;
        
    }
}

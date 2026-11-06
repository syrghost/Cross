using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UI ;

public class BareVieJoueur : MonoBehaviour
{
    public Slider barreVie ;
    public VieJoueur vieJoueur ; 
    private float  maxValueVie;


    void Update()
    {
        miseAJourBarre() ;
    }

    void miseAJourBarre()
    {
        barreVie.maxValue = vieJoueur.vieMax ;
        barreVie.value = vieJoueur.vieActuelle ;

    }
}

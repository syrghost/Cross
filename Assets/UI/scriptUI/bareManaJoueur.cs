using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bareManaJoueur : MonoBehaviour
{

    public Slider barreMana ;
    public manaJoueur manaJoueur ;
   
    void Start()
    {
        
    }

 
    void Update()
    {
        barreMana.maxValue = manaJoueur.manaMax;
        barreMana.value = manaJoueur.manaActuelle ;
        
    }
}

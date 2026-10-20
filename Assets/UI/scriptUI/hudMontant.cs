using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class hudMontant : MonoBehaviour
{

    public TMP_Text texteArgent;

    void OnEnable()
    {
        portefeuille.OnArgentChange += afficher ;
    }
    void OnDisable()
    {
        portefeuille.OnArgentChange -= afficher ;
    }



    void afficher( int montant)
    {
        texteArgent.text = montant.ToString();
    }


}

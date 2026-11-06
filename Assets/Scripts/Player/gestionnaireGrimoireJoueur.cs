using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gestionnaireGrimoireJoueur : MonoBehaviour
{
    public List<grimoire> grimoireConnus = new List<grimoire>();
    private int indexActuelle = 0 ;
    public int indexActuelPublic => indexActuelle;

    public grimoire grimoireActuelle
    {
        get
        {
            if(grimoireConnus.Count == 0) return null ;
            return grimoireConnus[indexActuelle] ;
        }
    }

    public void ajouterGrimoire(grimoire g)
    {
        if(grimoireConnus.Contains(g)) return ;
        grimoireConnus.Add(g) ;

    }

    public void grimoireSuivant()
    {
        if(grimoireConnus.Count == 0) return ;
        indexActuelle = (indexActuelle+1 ) % grimoireConnus.Count ;
    }

    public void choisir(int index)
{
    if (index < 0 || index >= grimoireConnus.Count) return;
    indexActuelle = index;
}

}

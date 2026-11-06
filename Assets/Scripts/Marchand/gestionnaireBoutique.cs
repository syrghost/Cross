using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum resultatAchat
{
    ok , pasAssezArgent , plein , dejaPossedee
}
public class gestionnaireBoutique : MonoBehaviour
{

    public static gestionnaireBoutique instance ;
    public inventaireManager inv ;


    void Awake()
    {
        instance = this ;
    }

    public resultatAchat acheter(articleBoutique article)
    {
         // cas pas assez d'argent 
         if(portefeuille.instance.Argent < article.prix)
        {
            return resultatAchat.pasAssezArgent ;
        }

        // cas inventaire plein ou deja posseder
        if (!inv.peutRecevoir(article.objet))
        {
            return article.objet is Consommable ? resultatAchat.plein : resultatAchat.dejaPossedee ;
        }

        // quand tous est bon 
        portefeuille.instance.depenser(article.prix);
        inv.recevoir(article.objet);
        return resultatAchat.ok ;
    }


}

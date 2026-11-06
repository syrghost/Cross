using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VieJoueur : MonoBehaviour
{

    public  int  vieMax  = 100;
    public int vieActuelle  {get ; private set;}

    [Header("invincibiliter apres coup")]
    public float dureeInvinciblite;
    private bool estInvincible = false;
    public inventaireManager inv ;
    private bool mort ;

    public menuResu menuResu ;
    void Start()
    {
        vieActuelle = vieMax;
    }
    public void SubirDegats(int degats)
    {
        if (estInvincible ||vieActuelle < 0) return ;

        vieActuelle -= degats;
        Debug.Log("le joueur a subits :" + degats);
        Debug.Log("vie restante  " + vieActuelle);

        if (vieActuelle <0)
        {
            Mourir();
        }
        else
        {
            StartCoroutine(ActiverInvinciblilite());
        }
        
    }

    void Mourir() 
    {
        mort = true ;
        if(inv.potionResuDisponible().Count >0 ) menuResu.ouvrir(this);
        else gameOver();
        Debug.Log("Vous etes morts Game over");

    }

    IEnumerator ActiverInvinciblilite()
    {
        estInvincible = true;
        yield return new WaitForSeconds(dureeInvinciblite);
        estInvincible = false;

    }

    public void Soigner(int quantite)
    {
        vieActuelle = Mathf.Min(vieActuelle + quantite , vieMax);
        Debug.Log("vous avez recu" + quantite  +" de soin");
    }

    public void ressuciter(float pourcentage)
    {
        vieActuelle = Mathf.Max(1 , Mathf.RoundToInt(vieMax * pourcentage)) ;
        mort = false ;
        StartCoroutine(ActiverInvinciblilite());
    }

   public void gameOver()
    {
        Debug.Log("t'as pas utiliser la potion de resu donc t'es mort");
    }



}

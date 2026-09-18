using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VieJoueur : MonoBehaviour
{

    public  int  vieMax  = 100;
    private int vieActuelle;

    [Header("invincibiliter apres coup")]
    public float dureeInvinciblite;
    private bool estInvincible = false;
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

        if (vieActuelle <= 0)
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


    void Update()
    {
        
    }
}

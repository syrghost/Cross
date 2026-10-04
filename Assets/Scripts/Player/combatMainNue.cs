using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class combatMainNue : MonoBehaviour
{
    public Hitbox hitboxMainGauche;
    public Hitbox hitboxMainDroite;
    public Hitbox hitboxPiedGauche;
    public Hitbox hitboxPiedDroite;
    public Arme arme;

    public void activerMainGauche() => activer(hitboxMainGauche);
    public void activerMainDroite() => activer(hitboxMainDroite);
    public void activerPiedGauche() => activer(hitboxPiedGauche);
    public void activerPiedDroite() => activer(hitboxPiedDroite);
    

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void activer(Hitbox hit)
    {
        if(hit == null)
        {
            return;
        }
        hit.degat = arme.degat;
        hit.Activer();
        Debug.Log ("hit box bien activer " + hit);
        Invoke(nameof(desactiverTous), arme.dureeActivationHitbox);
    }

    void desactiverTous()
    {
        if (hitboxMainDroite != null) hitboxMainDroite.Desactiver();
        if (hitboxMainGauche != null) hitboxMainGauche.Desactiver();
        if (hitboxPiedDroite != null) hitboxPiedDroite.Desactiver();
        if (hitboxPiedGauche != null) hitboxPiedGauche.Desactiver();

    }
}

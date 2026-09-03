using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemiIA: MonoBehaviour
{
    public float vitesseDeplacement = 3f ;
    public float distanceDetection = 8f;
    public float distanceAttaque = 5f;
    public int degatAttaque = 10 ;
    public float cooldownAttaque = 1.5f;

    private Transform joueur;
    private float timerAttaque  = 0f;
   
    void Start()
    {
        joueur = GameObject.FindGameObjectWithTag("Player").transform;
    }


    void Update()
    {
        float distance  = Vector3.Distance(transform.position , joueur.position);
        timerAttaque += Time.deltaTime;

    if (distance <= distanceDetection)
        {
            if (distance > distanceAttaque)
            {
                // poursuit 
                Vector3 direction = (joueur.position - transform.position).normalized;
                transform.position  += direction * vitesseDeplacement * Time.deltaTime ;
                transform.LookAt(new Vector3 (joueur.position.x , transform.position.y , joueur.position.z));
            }
            else
            {
                // attaque
                if (timerAttaque >= cooldownAttaque)
                {
                    Attaquer();
                    timerAttaque = 0f;
                }
            }
        }
    }
    void Attaquer()
    {
        VieJoueur vieJoueur = joueur.GetComponent<VieJoueur>();
        if ( vieJoueur != null)
        {
            vieJoueur.SubirDegats(degatAttaque);
        }
    }
}

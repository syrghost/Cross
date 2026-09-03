using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetConsommableAuSol : MonoBehaviour
{
    public Consommable consommable;


    void OnTriggerEnter(Collider other)
    {
        if (! other.CompareTag("Player")) return;

        VieJoueur vie = other.GetComponent<VieJoueur>();
        if ( vie != null && consommable.quantiteSoin > 0)
        {
            vie.Soigner(consommable.quantiteSoin);
        }

        Destroy(gameObject);
    }
    void Start()
    {
        
    }

   
    void Update()
    {
        
    }
}

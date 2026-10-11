using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dataConsommable : MonoBehaviour
{
    public Consommable consommable;
    public int quantiter ;

    void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player"))
        {
            return;
        }
        inventaireManager inv = other.GetComponent<inventaireManager>();
        inv.ajouterConsommable(consommable, quantiter);
        Destroy(gameObject);
    }
}

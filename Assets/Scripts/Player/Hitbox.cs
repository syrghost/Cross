using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
   public int degat ;
   private bool active = false;

   public void Activer()
    {
        active = true;
    }
    public void Desactiver()
    {
        active = false;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(!active) return;
        if ( !other.CompareTag("Ennemis")) return;

        Vie vieEnnemi = other.GetComponent<Vie>();

        if (vieEnnemi != null)
        {
            vieEnnemi.SubirDegat(degat);
        }


        Debug.Log("toucher" + "" + other.name);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int degat;
    private bool active = false;
    private Collider col;
    private readonly HashSet<Vie> dejaTouches = new HashSet<Vie>();

    void Awake()
    {
        col = GetComponent<Collider>();
        col.enabled = false;
    }

    public void Activer()
    {
        dejaTouches.Clear();  
        active = true;
        col.enabled = true;   
    }

    public void Desactiver()
    {
        active = false;
        col.enabled = false;
    }

    void OnTriggerEnter(Collider other) { Toucher(other); }
    void OnTriggerStay(Collider other)  { Toucher(other); } 

    void Toucher(Collider other)
    {
        if (!active) return;

        Vie vie = other.GetComponentInParent<Vie>();
        if (vie == null || !vie.CompareTag("Ennemis")) return;
        if (!dejaTouches.Add(vie)) return;   

        vie.SubirDegat(degat);

        Animator anim = vie.GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("subirDegat");

        Debug.Log("Touché : " + vie.name);
    }
}
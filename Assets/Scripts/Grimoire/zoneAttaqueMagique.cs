using System.Collections.Generic;
using UnityEngine;

public class zoneAttaqueMagique : MonoBehaviour
{
    public void infligerDegatZone(int degat, float rayon)
    {
        Collider[] touches = Physics.OverlapSphere(transform.position, rayon);
        HashSet<Vie> dejaTouches = new HashSet<Vie>();

        foreach (Collider c in touches)
        {
            Vie vie = c.GetComponentInParent<Vie>();
            if (vie == null || !vie.CompareTag("Ennemis")) continue;
            if (!dejaTouches.Add(vie)) continue;  

            vie.SubirDegat(degat);

            Animator anim = vie.GetComponent<Animator>();
            if (anim != null) anim.SetTrigger("subirDegat");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu (fileName = "NouvelleArme" , menuName = " Combat / Arme")]
public class Arme : ScriptableObject
{
    [Header("Infos generales")]
    public string nomArme ;
    public int degat = 10 ;
    public float porteeAttaque = 1 ;

    [Header("Animation")]
    public string nomTriggerAnimator;
    public string nomStateAnimator;

    [Header("Timing hitbox")]
    public float dureeActivationHitbox = 0.2f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

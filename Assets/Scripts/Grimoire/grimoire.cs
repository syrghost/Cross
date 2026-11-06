using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum typeSortGrimoire {
    projectile , zone
}

[CreateAssetMenu( fileName = " nouveauGrimoire" , menuName = " grimoire/sort")]
public class grimoire : ScriptableObject
{
    [Header("info")]

    public string nomGrimoire ;
    public Sprite iconGrimoire ;
    [TextArea] public string descriptionGrimoire ;

    [Header("general")]
    public typeSortGrimoire typeSort ;
    public float coutMana = 10f ;
    public int degatSort = 10 ;


    [Header("projectile")]

    public float vitesseProjectile = 15f ;
    public float dureeVieProjectile = 3f ;

    [Header("zone ")]

    public float rayonZoneProjectile = 5f ;

    public float dureeVieZone = 5f ;

    [Header("effet")]
    public GameObject sort ;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "NouveauConsommable" , menuName = "Combat/Consommable")]
public class Consommable : ScriptableObject
{
    [Header("info general")]
    public string nomObjet;

    public Sprite imageConsommable;
    [TextArea] public string description;
    public int quantiterMax =99;

    public enum typeConsommable
    {
        potionSoin,
        potionMana,
        potionResu
    }
    public typeConsommable tuConsomme;
    public int quantiterSoinRecu;
    public int quantiterManaRecu;



    [Range(0f , 1f)] public float pourcentageVieResu = 0.25f ;

    public bool utilisableManuellement => tuConsomme != typeConsommable.potionResu ;

    

}   

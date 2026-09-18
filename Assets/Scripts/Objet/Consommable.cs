using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "NouveauConsommable" , menuName = "Combat/Consommable")]
public class Consommable : ScriptableObject
{
    [Header("info general")]
    public string nomObjet;
    public string description;

    [Header("Effet")]
    public int quantiteSoin;
    public int quantiteMana;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}

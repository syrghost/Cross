using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "nouvelle Armure" , menuName = "Combat/Armure")]
public class Armure : ScriptableObject
{
    public enum typeArmure
    {
        casque,
        torse,
        botte,
        pantalon
    }

    public string nomArmure;
    public int defense = 1;
    public typeArmure emplacementArmure;


}

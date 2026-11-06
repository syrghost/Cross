using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class articleBoutique 
{

    public ScriptableObject objet ;
    public int prix ;
    public string description ;

    
}

[CreateAssetMenu (fileName = "nouvelle boutique" , menuName = "Data/boutique")]

public class boutique : ScriptableObject
{
    public string nomBoutique ;

    public articleBoutique[] articles ;

}

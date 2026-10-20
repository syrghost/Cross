using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum typeRecompense
{
    argent ,
    objet
}

[System.Serializable]
public class recompense 
{
    public typeRecompense type ;
    public int quantiter ;
    public ScriptableObject objet ;    

}

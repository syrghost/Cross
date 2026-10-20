using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum typeObjectif
{
    tuer,
    ramasser, 
    parler
}


[System.Serializable] 
public class Objectif 
{ 
    public typeObjectif type ;
    public string idCible ;
    public int nombreRequis ;
    
    public string description ;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  static class evenementsJeu
{
    public static event System.Action<typeObjectif , string> onEvenement ;

    public static void Declencher( typeObjectif type, string id)
    {
        onEvenement?.Invoke(type , id) ;
        Debug.Log("Evenement : " + type + " / " + id);
    }
}

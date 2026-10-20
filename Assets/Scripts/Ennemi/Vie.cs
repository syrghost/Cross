using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vie : MonoBehaviour
{
    public int vieMax = 30 ;
    private int vieActuelle;

    public string idEnnemie ;

    public static event Action<Vie> onMort;
    // Start is called before the first frame update
    void Start()
    {
        vieActuelle = vieMax;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SubirDegat(int degat)
    {
        vieActuelle -= degat ;
        Debug.Log( gameObject.name + "as subits" + degat  +"degats");
        Debug.Log("vie restante" + vieActuelle);

        if (vieActuelle <= 0)
        {
            Mourir();
        } 
    }
    void Mourir()
    {
        Debug.Log(gameObject.name + " est morts");
        onMort?.Invoke(this);

        evenementsJeu.Declencher(typeObjectif.tuer, idEnnemie) ;

        Destroy(gameObject);
    }
}

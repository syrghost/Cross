using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public inventaireManager inv;
    public Arme arme;
    private bool equipee = false ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inv.equiperArme(arme);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            inv.desequiperArme();
        }
    }
}

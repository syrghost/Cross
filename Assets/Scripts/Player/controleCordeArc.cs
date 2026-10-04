using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controleCordeArc : MonoBehaviour
{
    [Header("reference")]
    public Transform osCorde ; // l'os ou se trouve la corde dee l'arc 
    public Transform mainDroite; // mais dorite du player 

    public modeFocus modeFocus;

    public switchEmplacementArme switchEmplacementArme;

    public float vitesseRetour = 15f;

    private Vector3 velocityActuelle = Vector3.zero;



    private Vector3 positionReposLocal;

    void Start()
    {
        if (osCorde != null)
        {
            positionReposLocal = osCorde.localPosition;
        }
        
    }

    void LateUpdate()
    {
        if(osCorde == null)
        {
            return;
        }

        bool doitSuivreMain = modeFocus.enVisee && !modeFocus.enTrainDeTirer && switchEmplacementArme.mainSurCorde;
        if (doitSuivreMain && mainDroite != null )
        {
            osCorde.position = mainDroite.position;
            velocityActuelle = Vector3.zero;
        }
        else
        {
            Vector3 positionReposMonde = osCorde.parent.TransformPoint(positionReposLocal);
            osCorde.position = Vector3.SmoothDamp(osCorde.position, positionReposMonde, ref velocityActuelle , 1f / vitesseRetour);
        }
    }


    void Update()
    {
        
    }
}

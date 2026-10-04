using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class tirerFleche : MonoBehaviour
{
    [Header("Gestion fleche arc")]
    public GameObject prefabFleche;
    public Transform pointInstanciationFleche;
    public modeFocus modeFocus ;
    public switchEmplacementArme switchEmplacementArme;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        //pour le tire de la fleche 

    public void tirer()
    {
        
        Vector3 direction ;
        
        
        if(modeFocus.focusActiver && modeFocus.ennemiCible != null)
        {
            Transform pointVise = modeFocus.obtenirPoinVise(modeFocus.ennemiCible);
            direction = pointVise.position- pointInstanciationFleche.position;


        }
        else if (modeFocus.camTransform != null)
        {
            direction = modeFocus.camTransform.forward;

        }
        else
        {
            direction = pointInstanciationFleche.forward;
        }
        GameObject flecheInstance = Instantiate(prefabFleche,pointInstanciationFleche.position,Quaternion.LookRotation(direction));
        fleche fleche = flecheInstance.GetComponent<fleche>();
        int degatArc = switchEmplacementArme.armeData != null ? switchEmplacementArme.armeData.arme.degat : 10;
        fleche.initialiserFleche(direction,degatArc);

    }
}

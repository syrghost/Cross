using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "NouveauPNJ" ,menuName ="Data/pnj")]
public class pnj : ScriptableObject
{

    public string nomPnj;
    [TextArea(2,5)]
    public string[] ligneDialogue ;

    public quete[] queteProposer ;

    public quete queteActuelle(GestionnaireQuete gq)
    {

        // quete a rendre d'abord 
        foreach(quete q in queteProposer)
        {

            etatQuete e = gq.Etat(q) ;
            if(e== etatQuete.enCours ||  e == etatQuete.terminer) return q ;

        }
        // sinon la premiere quete dispo 
        foreach (quete q in queteProposer)
        
        {
            Debug.Log(q.titre + " | etat = " + gq.Etat(q) + " | prerequisOk = " + gq.prerequisOk(q));
            if(gq.Etat(q)== etatQuete.nonCommencer && gq.prerequisOk(q)) return q ;
        }

        return null ;
    }

    public quete queteLancee(GestionnaireQuete gq)
    {
        foreach (quete q in queteProposer)
        {
            etatQuete e = gq.Etat(q) ;
            if(e== etatQuete.enCours || e == etatQuete.terminer) return q ;
        }

        return null ;
    }

    public quete queteDisponible(GestionnaireQuete gq)
    {

        foreach( quete q in queteProposer)
        {
            if(gq.Etat(q) == etatQuete.nonCommencer && gq.prerequisOk(q)) return q ;
        }
        return null ;
    }

        


}

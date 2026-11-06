using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pnjData : MonoBehaviour
{

    public pnj donnee ;
    public dialogueManager  dialogueManager;

    
    public reactionQuete[] reactions ;

    private Transform joueur ;


    
    public void interagir()
    {

        var gq = GestionnaireQuete.instance ;
        string [] lignes = donnee.ligneDialogue ;
        System.Action aLaFin = null ;


        // sa quete lancer 
        quete q = donnee.queteLancee(gq) ;

        if(q == null)
        {
            // reaction d'une autre quete en cour 
            string[] reac = Reaction(gq , false) ;

            if(reac != null)
            {
                lignes = reac ;
            }
            else
            {
                // proposition de sa propre quete disponible 
                q = donnee.queteDisponible(gq) ;

                // reaction a la quete d'un autre rendu 
                if(q== null)
                {
                    reac = Reaction(gq , true ) ;
                    if(reac != null)
                    {
                        lignes = reac ;
                    }
                }

            }
        }



        if(q != null)
        {

            etatQuete e = gq.Etat(q);
            string[] voulue = q.dialogueSelon(e) ;

            if(voulue != null ) lignes = voulue ;


            

            if(e == etatQuete.nonCommencer)
            {
                aLaFin = () => popUpChoix.instance.afficher("Accepter vous la quete « " + q.titre + " » ?", () => gq.Demarrer(q));
            }
            else if (e == etatQuete.terminer)
            {
                aLaFin = () => gq.rendre(q);
            }
        }

        if(aLaFin == null && donnee.boutiqueVendu != null)
        {
            boutique b = donnee.boutiqueVendu;
    aLaFin = () => uiBoutique.instance.Ouvrir(b);
        }

        

        Vector3 cible = joueur.position;
        cible.y = transform.position.y ; 
        transform.LookAt(cible);
        dialogueManager.instance.demarer(donnee.nomPnj , lignes , aLaFin);
        evenementsJeu.Declencher(typeObjectif.parler , donnee.nomPnj) ;
    }


     string[] Reaction(GestionnaireQuete gq , bool queteRendue)
    {

        foreach(reactionQuete r in reactions)
        {
            if(r.quete == null ) continue ;

            etatQuete e = gq.Etat(r.quete);
            string[] voulu = null ;

            if(!queteRendue)
            {
                if(e == etatQuete.enCours) voulu = r.siEnCours ;

                else if( e == etatQuete.terminer)
                    voulu = r.siObjectifFait.Length >0 ? r.siObjectifFait : r.siEnCours ;
                

            }
            else if (e == etatQuete.rendu ) voulu = r.siRendu ;

            if(voulu != null && voulu.Length > 0 ) return voulu ;
        }
        return null ;
    }


    void OnTriggerEnter(Collider other) 
    {
        if( !other.CompareTag("Player")) return ;
        other.GetComponent<interagir>().pnjProche = this ;
        joueur = other.transform;

    }
    void OnTriggerExit(Collider other)
    {
        if( !other.CompareTag("Player")) return ;
        var inter = other.GetComponent<interagir>();

        if(inter.pnjProche == this) inter.pnjProche = null ;
    }
}

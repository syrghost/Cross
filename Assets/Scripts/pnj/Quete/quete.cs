using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum etatQuete
    {
        nonCommencer,
        enCours,
        terminer,
        rendu 

    }


[CreateAssetMenu(fileName = " nouvelle quete" , menuName = "Data/quete")]
public class quete : ScriptableObject
{ 
    public string titre ;
    public Objectif[] objectifs ;
    public recompense[] recompense ;

    public quete[] prerequis ;

    [TextArea(2,4)] public string [] dialogueAvant ;
    [TextArea(2,4)] public string [] dialoguePendant ;
    [TextArea(2,4)] public string [] dialogueApres;

    public string[]dialogueSelon (etatQuete e)
    {
        switch (e)
        {
            case etatQuete.nonCommencer : return dialogueAvant ;
            case etatQuete.enCours : return dialoguePendant ;

            case etatQuete.terminer : return dialogueApres ;

            default : return null ;
        }
    }

}

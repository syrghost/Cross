using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class reactionQuete 
{
    public quete quete ;
    [TextArea(2,4)] public string[] siEnCours;
    [TextArea(2,4)] public string[] siObjectifFait; //si l'objectif est accomplis le dialogue avant de remettre la quete 
    [TextArea(2,4)] public string[] siRendu;
}

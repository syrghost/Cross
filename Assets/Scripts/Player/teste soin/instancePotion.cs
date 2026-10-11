using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class instancePotion : MonoBehaviour
{

    public GameObject prefabPotionSoin;
   
    public GameObject pointInstancePotion;

    private GameObject potionActuelle;

    public inventaireManager inv;
    private bool peutBoire = true;
    private bool arcEquipee  =>  inv.armeEquipeeActu.monTypeArme == Arme.typeArme.Arc;
    public switchEmplacementArme switchEmplacementArme ;

    public PlayerCombat playerCombat ;

    private bool enTrainDattaquer => playerCombat.enTrainDattaquer ;


    private  bool armeEnMain => switchEmplacementArme.armeEnMain;
    private bool  armeEnCourDeChangement => switchEmplacementArme.enCourDeChangement ;

    

    // Start is called before the first frame update


    // Update is called once per frame
    public void essayerDeBoire()
    {
        if(enTrainDattaquer) return ;
        if(arcEquipee && armeEnMain) return ;
        
        if (inv.consommableEquiper == null  || !peutBoire || armeEnCourDeChangement ) return ;
        
        if (inv.utiliserConsommableEquipee())
        {
                peutBoire = false ;
        }
        
        
    }

    public void instancierPotion()
    {
        
        if(arcEquipee && armeEnMain) return ;
        if( armeEnCourDeChangement) return ;

        potionActuelle = Instantiate(prefabPotionSoin , pointInstancePotion.transform);

        
    }

    public void destructionPotion()
    {
        if(potionActuelle != null) Destroy(potionActuelle);
        peutBoire = true ;
    }
}

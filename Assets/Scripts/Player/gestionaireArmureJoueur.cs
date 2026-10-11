using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gestionaireArmureJoueur : MonoBehaviour
{
    public Transform armureJoueurParent;


    public int defenseTotal {  get; private  set ; }

    private Dictionary<Armure,armureData> registreArmure  = new Dictionary<Armure, armureData>();
    private Dictionary<Armure.typeArmure, armureData> equipee = new Dictionary<Armure.typeArmure, armureData>();
    
    void Start()
    {
        
    }
    void Awake()
    {
        construireRegistre();
    }


    void construireRegistre()
    {
        armureData[] toutesLesArmure = armureJoueurParent.GetComponentsInChildren<armureData>(true);
        foreach(armureData donnee in toutesLesArmure)
        {
            if(donnee.armure == null)
            {
                Debug.LogWarning("arme sans reference : " + donnee.gameObject.name);
                continue;
            }
            registreArmure[donnee.armure] = donnee;
            Afficher(donnee,false);
        }

    }

    public void equiper(Armure armure)
    {
        if (!registreArmure.ContainsKey(armure))
        {
            
            Debug.LogWarning("armure pas de le registre armure  " + armure.nomArmure);
            return;
        }
        desequiper(armure.emplacementArmure);
        
        armureData d = registreArmure[armure];
        Afficher (d,true);
        equipee[armure.emplacementArmure] = d ;
        recalculerDefense();
    }

    public void desequiper(Armure.typeArmure type)
    {
        if( ! equipee.TryGetValue(type, out armureData d))
        {
            return;
        }
        Afficher( d, false);
        equipee.Remove(type);
        recalculerDefense();
    }

    void Afficher(armureData armureData , bool visible)
    {
        foreach(Renderer r in armureData.renderers) r.enabled = visible;
    }

    void recalculerDefense()
    {
        defenseTotal = 0;

        foreach( var defenseArmure in equipee.Values)
        {
            defenseTotal += defenseArmure .armure.defense;
        }
    }


    // TESTE POUR L'UI
    public bool EstArmureEquipee(Armure armure)
    {
        if (armure == null) return false;
        if (equipee.TryGetValue(armure.emplacementArmure, out var data))
        {
            return data.armure == armure;
        }
        return false;
    }
}   

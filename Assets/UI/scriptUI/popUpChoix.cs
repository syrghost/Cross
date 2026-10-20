using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class popUpChoix : MonoBehaviour
{
    // Start is called before the first frame update

    public static popUpChoix instance ;
    public GameObject panneauChoix ;
    private InputAction selectionner,gauche , droite ;
    public PlayerController playerController ;

    public TMP_Text titreTexte ;
    public TMP_Text oui ;
    public TMP_Text non ;

    public Color couleurSelection = Color.blue ;
    public Color couleurNormale = Color.white ;

    System.Action surOui ;
    bool ouverte ;
    int index ;
    int frameOuverture ;


    void Awake()
    {
        instance = this ;

    }

  
    void Start()
    {
        panneauChoix.gameObject.SetActive(false) ;
        var controls = playerController.controls ;
        selectionner = controls.Dialogue.suivant ;
        gauche = controls.Dialogue.oui ;
        droite  = controls.Dialogue.non ;

        oui.color = Color.red ;

        
    }

    public void afficher(string titre , System.Action aSurOui)
    {
        surOui = aSurOui ;
        titreTexte.text = titre ;
        index = 0 ;
        raffraichirSelection () ;

        panneauChoix.gameObject.SetActive(true) ;
        ouverte = true ;
        frameOuverture = Time.frameCount ;
        dialogueManager.instance.garderLeControl = true ;


    }

    // Update is called once per frame
    void Update()
    {

        if(!ouverte || Time.frameCount <= frameOuverture)
        {
            return ;
        }
        if (gauche.WasPressedThisFrame())
        {
            index = 0 ;
            raffraichirSelection();
        }
        else if (droite.WasPressedThisFrame())
        {
            index = 1;
            raffraichirSelection();
        }

        else if (selectionner.WasPressedThisFrame())
        {
            fermer(index == 0) ;
        }
        
        

        
    }

    void raffraichirSelection()
    {
        oui.color = index == 0 ? couleurSelection :   couleurNormale ;
        non.color = index ==1 ? couleurSelection :   couleurNormale ;
        oui.text = (index == 0 ? "> " : "  ") + "Oui";
        non.text = (index == 1 ? "> " : "  ") + "Non";
    }


    void fermer(bool accepte )
    {
        ouverte = false ;
        panneauChoix.gameObject.SetActive(false) ;

        if(accepte ) surOui?.Invoke() ;
        dialogueManager.instance.rendreControl();

    }
}

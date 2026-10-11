using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class uiDefault : MonoBehaviour
{
   public VieJoueur vieJoueur;
   private ProgressBar progressBar;
   private VisualElement root ;
    void Start()
    {
        

    }
    void OnEnable()
    {

        UIDocument uIDocument = GetComponent<UIDocument>();
        root = uIDocument.rootVisualElement;
        progressBar = root.Q<ProgressBar>("vieJoueur");

    }
    // Update is called once per frame
    void Update()
    {
        miseAJourBarre();

    }
    void miseAJourBarre()
    {
        progressBar.highValue = vieJoueur.vieMax;
        progressBar.value = vieJoueur.vieActuelle;
    }
}

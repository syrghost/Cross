using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class slotConsommableUi : MonoBehaviour
{

    [Header("refrence")]

    public Image icone ;
    public TMP_Text quantite;

    public GameObject cadreSelection ; 


    public void Init(Sprite sprite, int qte, bool afficherQte = true)
    {
        
        icone.sprite = sprite;
        quantite.gameObject.SetActive(afficherQte);

        if (afficherQte) quantite.text = qte.ToString();

    }

    public void surbrillance(bool actif)
    {
        cadreSelection.SetActive(actif);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

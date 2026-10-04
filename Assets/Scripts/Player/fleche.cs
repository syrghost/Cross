using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fleche : MonoBehaviour
{
    
    public int degat = 1;
    public float vitesseInitial = 25f;
    public float dureeDeVie = 5f;
    public float gravite =5f;

    private Vector3 velocite;
    public bool aToucher = false;

    public void initialiserFleche(Vector3 direction , int degatArc)
    {
        degat = degatArc;
        velocite = direction.normalized * vitesseInitial;
        Destroy(gameObject,dureeDeVie);
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(aToucher) return ;
        // appliquer la graviter a la flehe 
        velocite += Vector3.down * gravite * Time.deltaTime;

        // deplacer la fleche 

        transform.position += velocite * Time.deltaTime;

        // orienter la fleche selon la direction 
        if (velocite.sqrMagnitude > 0.01)
        {
            transform.rotation = Quaternion.LookRotation(velocite)  * Quaternion.Euler(90f,0f,0f);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if(aToucher) return ;
        if (!other.CompareTag("Ennemis")) return ;
        Vie vieEnnemi = other.GetComponent<Vie>();
        Animator animatorEnnemi = other.GetComponent<Animator>();
        if(vieEnnemi != null && animatorEnnemi != null)
        {
            vieEnnemi.SubirDegat(degat);
            animatorEnnemi.SetTrigger("subirDegat");
            transform.SetParent(vieEnnemi.transform);
        
        }
        aToucher  = true ;
        velocite = Vector3.zero;
        Destroy (gameObject, 3f);


    }
}

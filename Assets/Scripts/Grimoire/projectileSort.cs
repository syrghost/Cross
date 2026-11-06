using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileSort: MonoBehaviour
{


    private grimoire sort ;
    
    private Vector3 velocity ;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(sort == null) return ;

        transform.position += velocity * Time.deltaTime ;

        if (velocity.sqrMagnitude > 0.01)
        {
            transform.rotation = Quaternion.LookRotation(velocity)  * Quaternion.Euler(-90f,-90f,0f);
        }

        
    }

    public void initialiser(grimoire g , Vector3 direction)
    {
        sort = g ;
        velocity = direction.normalized * sort.vitesseProjectile ;
        Destroy(gameObject, sort.dureeVieProjectile);
    }

    void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Ennemis")) return ;

        Vie vieEnnemi = other.GetComponent<Vie>() ;

        Animator animator = other.GetComponent<Animator>();

        if (animator == null ) return ;
        
        if(vieEnnemi != null)
        {
            vieEnnemi.SubirDegat(sort.degatSort);
            animator.SetTrigger("subirDegat") ;

        }
        Destroy(gameObject);
    }
}

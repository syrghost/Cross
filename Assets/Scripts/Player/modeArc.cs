using UnityEngine;

public class modeArc : MonoBehaviour
{
    private bool enVisee = false;
    public Animator animator;
    
    [Header("Paramètres Visée")]
    public float vitesseRotation = 10f; 
    private Transform camTransform;

    void Start()
    {
        
        if (Camera.main != null)
        {
            camTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        
        if (Input.GetMouseButton(1)) 
        {
            enVisee = true;
            OrienterJoueurVersCible(); 
        }
        else 
        {
            enVisee = false;
        }

       
        if (enVisee && Input.GetMouseButtonDown(0)) 
        {
            animator.SetTrigger("DecocherFleche"); 
        }

        
        animator.SetBool("enVisee", enVisee);
    }

    void OrienterJoueurVersCible()
    {
        if (camTransform == null) return;

        
        Vector3 directionVisee = camTransform.forward;
        
        
        directionVisee.y = 0f; 

        if (directionVisee != Vector3.zero)
        {
            Quaternion rotationDeBase = Quaternion.LookRotation(directionVisee);
            Quaternion rotationSouhaiter = rotationDeBase * Quaternion.Euler(0f,90f,0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotationSouhaiter, Time.deltaTime * vitesseRotation);
        }
    }
}
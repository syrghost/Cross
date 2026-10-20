
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class dialogueManager : MonoBehaviour
{

    public static dialogueManager instance ;
    public PlayerController playerController ;
    public Image fondDialogue ;
    public TMP_Text nomPnj ;
    public TMP_Text dialoguePnj ;
    public bool enCours ;
    string[] lignes ;
    int index ;
    private InputActionMap mapPlayer , mapDialogue;
    private InputAction suivantAction ;

    public int frameFin ;
    public int frameDebut ; 
    System.Action surFin ;

    public bool garderLeControl ;



    void Awake()
    {
        instance = this ;
        

        
    }
    
    void Start()
    {
        fondDialogue.gameObject.SetActive(false);
        mapPlayer = playerController.controls.Player ;
        mapDialogue = playerController.controls.Dialogue ;
        suivantAction = playerController.controls.Dialogue.suivant;
    }

    // Update is called once per frame
    void Update()
    {
        if(enCours && suivantAction.WasPressedThisFrame() && Time.frameCount > frameFin)
        {
            ligneSuivante();
        }
        
    }

    public void demarer(string nom , string[] ligneAfficher , System.Action aLaFin = null )
    {

        if(ligneAfficher == null || ligneAfficher.Length == 0 ) return ;

        lignes = ligneAfficher;

        surFin = aLaFin ;

        index  = 0 ;
        enCours = true ;
        frameDebut = Time.frameCount ;

        mapPlayer.Disable();
        mapDialogue.Enable();

        fondDialogue.gameObject.SetActive(true);
        nomPnj.text= nom;
        dialoguePnj.text = lignes[0];

    }

    void ligneSuivante()
    {
        index ++ ;
        if(index >= lignes.Length)
        {
            terminer();
            return ;
        }
        dialoguePnj.text = lignes[index];
    }

    void terminer()
    {
        enCours = false;
        frameFin = Time.frameCount ;

        fondDialogue.gameObject.SetActive(false);

        garderLeControl = false ;

        surFin?.Invoke() ;

        if( !garderLeControl)
        {
            rendreControl();
        }

    }

    public void rendreControl()
    {
        frameFin = Time.frameCount ;

        mapDialogue.Disable();
        mapPlayer.Enable() ;
    }
}

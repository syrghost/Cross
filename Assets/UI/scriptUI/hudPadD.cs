
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI ;
public class hudPadD : MonoBehaviour
{

    [Header("reference")]
    public inventaireManager inv ;

    public instancePotion instancePotion ;
    public PlayerController playerController ;
    [Header("slot Equipee")]

    public Image iconeEquipee ;
    public TMP_Text quantiteEquipee;

    [Header("liste")]

    public GameObject panelListe ;
    public Transform conteneur ;

    public slotConsommableUi prefabSlot ;

    public float dureeAppuieLong = 0.35f;


    private InputAction appuiAction , precedentAction , suivantAction ;
    private float chrono ;
    private bool listeOuverte ;
    private int indexSelection ;
    private Consommable dernierEquiper ;

    private int derniereQuantiter = -1;

    // Start is called before the first frame update
    void Start()
    {
        var controls = playerController.controls;
        appuiAction = controls.Player.utiliserPotion;
        precedentAction = controls.Player.consommablePrecedent;
        suivantAction = controls.Player.consommableSuivant;

        panelListe.SetActive(false);
        rafraichirConsommable();

        
    }

    void rafraichirConsommable()
    {
        dernierEquiper = inv.consommableEquiper;
        derniereQuantiter = dernierEquiper != null ? inv.quantiterConsommable(dernierEquiper) : 0 ;

       bool aUnObjet = dernierEquiper != null ;
       iconeEquipee.enabled = aUnObjet;
       quantiteEquipee.text = aUnObjet ? derniereQuantiter.ToString() : "";

       if(aUnObjet ) iconeEquipee.sprite = dernierEquiper.imageConsommable ;
    }

    void ouvrirListe()
    {
        listeOuverte = true ;
        playerController.inputBloquer = true ;

        foreach(Transform enfant in conteneur) Destroy(enfant.gameObject);

        indexSelection = Mathf.Max(0 , inv.slots.FindIndex(s=> s.objet == inv.consommableEquiper));

        foreach(var slot in inv.slots)
        {
            var ui = Instantiate(prefabSlot , conteneur);
            ui.Init(slot.objet.imageConsommable , slot.quantiter);

            
        }
        panelListe.SetActive(true);
        mettreAjourSurbrillance();

    }

    void mettreAjourSurbrillance()
    {
        for( int i = 0 ; i < conteneur.childCount ; i++)
        {
            conteneur.GetChild(i).GetComponent<slotConsommableUi>().surbrillance(i== indexSelection);
        }
    }

    void gererNavigation()
    {
        int direction = 0 ;
        if(suivantAction.WasPressedThisFrame()) direction = 1 ;

        else if (precedentAction.WasPressedThisFrame()) direction = -1 ;

        if(direction == 0 ) return ;

        indexSelection = Mathf.Clamp( indexSelection + direction , 0 , inv.slots.Count - 1) ;

        mettreAjourSurbrillance();
    }

    void validerEtFermer()
    {
        inv.equiperConsommable(inv.obtenirConsommable(indexSelection));
        fermerListeSansValider() ;
        
    }

    void fermerListeSansValider()
    {
        listeOuverte = false ;
        playerController.inputBloquer = false ;
        panelListe.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if(!playerController.controls.Player.enabled)
        {
            if (listeOuverte)
            {
                fermerListeSansValider();
            }
            
            return ;
        }
        if(appuiAction.WasPressedThisFrame()) chrono = 0f ;
        if (appuiAction.IsPressed())
        {
            chrono += Time.unscaledDeltaTime;

            if(! listeOuverte && chrono >= dureeAppuieLong && inv.slots.Count > 0)
            {
                ouvrirListe() ;
            }
            if (listeOuverte)
            {
                gererNavigation();
            }

        }
        if (appuiAction.WasReleasedThisFrame())
        {
            if(listeOuverte) validerEtFermer() ;

            else if (chrono < dureeAppuieLong)
            {
                instancePotion.essayerDeBoire();
                Debug.Log(" appuie cour essayer de boire");
            }
             

        }

        int qte = inv.consommableEquiper != null  ? inv.quantiterConsommable(inv.consommableEquiper) : 0 ;

        if( inv.consommableEquiper != dernierEquiper || qte != derniereQuantiter)
        {
            rafraichirConsommable();
        }
        
    }
}

using UnityEngine;

public class gestionCarquois : MonoBehaviour
{
    public GameObject attachementCarquois;
    public switchEmplacementArme switchEmplacementArme;
    public GameObject[] carquois; 

    private int derniereArme = -1;

    void Start()
    {
        if (switchEmplacementArme == null || attachementCarquois == null)
        {
            Debug.LogWarning("gestionCarquois : référence non assignée");
            enabled = false;
            return;
        }

        foreach (GameObject c in carquois)
        {
            c.transform.SetParent(attachementCarquois.transform, false);
            c.SetActive(false);
        }
    }

    void Update()
    {
        int armeEquipee = (int)switchEmplacementArme.obtenirTypeArmeEquipee();
        if (armeEquipee == derniereArme) return;
        derniereArme = armeEquipee;

        // on cache tout d'abord
        foreach (GameObject c in carquois) c.SetActive(false);

        if (armeEquipee != (int)Arme.typeArme.Arc) return;

        Arme arme = switchEmplacementArme.armeData.arme;
        if (arme.indexCarquois < 0 || arme.indexCarquois >= carquois.Length)
        {
            Debug.LogWarning("indexCarquois invalide pour " + arme.nomArme);
            return;
        }

        GameObject actif = carquois[arme.indexCarquois];
        actif.transform.localPosition = arme.positionCarquois;
        actif.transform.localRotation = Quaternion.Euler(arme.rotationCarquois);
        actif.SetActive(true);
    }
}
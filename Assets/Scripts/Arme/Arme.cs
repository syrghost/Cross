using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu (fileName = "NouvelleArme" , menuName = " Combat / Arme")]
public class Arme : ScriptableObject
{
    [Header("Infos generales")]
    public string nomArme ;
    public int degat = 10 ;
    public float porteeAttaque = 1 ;

    [Header("Animation")]
    public string nomTriggerAnimator; 
    public string nomStateAnimator; 

    [Header("Timing hitbox")]
    public float dureeActivationHitbox = 0.2f;

    [Header("position Arme")]
    public Vector3 positionDos;
    public Vector3 rotationDos;
    public Vector3 positionMain;
    public Vector3 rotationMain;


    public enum typeArme
    {
        mainNues = 0,
        Lance = 1,
        epeeUneMain = 2,
        epeeDeuxMain = 3,
        Arc = 4
    }

    public typeArme monTypeArme;
    [Header("Si l'arme est un arc")]
    public int indexCarquois; //position dans le tableau de gestion carquois 

    public Vector3 positionCarquois;
    public Vector3 rotationCarquois;

    [Header("Aimentation / lunge")]
    public bool utiliserAimentation = true;
    public float angleDetectionAimentation = 60f;
    public float distanceLunge = 3f;
    public float distanceArret = 1.2f;
    public float dureeLunge = 0.2f;
    public float dureeRotationLunge = 0.1f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

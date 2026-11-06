using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class grimper : MonoBehaviour
{
    [Header("reference")]
    public PlayerController playerController;
    public modeFocus modeFocus;
    public Animator animator;
    public switchEmplacementArme switchEmplacementArme;
    public Transform visuel;                      // heroTesteV2 (le mesh, enfant du Player)

    [Header("detection")]
    public float distanceDetection = 1f;
    public LayerMask coucheParoi;
    public float angleMaxParoi = 20f;

    [Header("escalade")]
    public float vitesseGrimper = 2f;
    public float distanceAuMur = 0.6f;
    public float vitessePlaquage = 10f;
    public float vitesseRotation = 10f;
    public float delaiRegrab = 0.4f;
    public float dureeLacher = 0.5f;
    public float decalageVisuel = 0.35f;          // rapproche le mesh du mur

    [Header("rebord")]
    public float hauteurMains = 1.9f;             // pieds -> mains accrochées (à régler à l'oeil)
    public float toleranceRebord = 0.3f;
    public float vitesseRebord = 1.5f;
    public string nomEtatMontee = "monter";       // nom EXACT de l'état dans l'Animator

    public bool estEnTrainDeGrimper = false;

    private enum Etat { Aucun, Mur, Accrochage, Rebord, Montee }
    private Etat etat = Etat.Aucun;

    private RaycastHit infoParois;
    private InputAction moveAction, sautAction;
    private CharacterController controller;
    private float tempsFinEscalade;
    private bool murDetecter;
    private Vector2 input;

    private float yRebord;
    private Vector3 normaleMur;
    private Coroutine routine;
    private Coroutine bloquageLacher;

    [Header("Monter")]
    public float distanceMontee = 0.8f ;

    [Range(0.3f , 0.9f )] public float partieVertical = 0.6f ;

    void Start()
    {
        controller = playerController.controller;
        var controls = playerController.controls;

        moveAction = controls.Player.Move;
        sautAction = controls.Player.saut;
    }

    void Update()
    {
        input = Vector2.ClampMagnitude(moveAction.ReadValue<Vector2>(), 1f);

        murDetecter = Physics.Raycast(controller.bounds.center, transform.forward,
                                      out infoParois, distanceDetection, coucheParoi);
        Debug.DrawRay(controller.bounds.center, transform.forward * distanceDetection,
                      murDetecter ? Color.green : Color.red);

        if (murDetecter && Mathf.Abs(infoParois.normal.y) > Mathf.Sin(angleMaxParoi * Mathf.Deg2Rad))
            murDetecter = false;

        if (!estEnTrainDeGrimper)
        {
            bool libre = !playerController.inputBloquer
                         && Time.time > tempsFinEscalade
                         && !modeFocus.focusActiver
                         && !switchEmplacementArme.armeEnMain;

            if (murDetecter && libre && input.y > 0.5f)
                activerEscalade();
            return;
        }

        switch (etat)
        {
            case Etat.Mur:    gererEscalade(); break;
            case Etat.Rebord: gererRebord();   break;
            // Accrochage et Montee sont gérés par leurs coroutines
        }
    }

    void LateUpdate()
    {
        if (visuel == null) return;
        float cible = etat != Etat.Aucun ? decalageVisuel : 0f;
        Vector3 p = visuel.localPosition;
        p.z = Mathf.MoveTowards(p.z, cible, 2f * Time.deltaTime);
        visuel.localPosition = p;
    }

    // ---------------------------------------------------------------- MUR

    void activerEscalade()
    {
        estEnTrainDeGrimper = true;
        etat = Etat.Mur;
        playerController.enEscalade = true;
        playerController.resetPhysique();

        if (animator)
            animator.SetBool("grimper", true);
    }

    void gererEscalade()
    {
        if (!murDetecter)
        {
            Debug.Log("lâché : plus de mur");
            arreterEscalade(); return;
        }
        if (sautAction.WasPressedThisFrame())
        {
            Debug.Log("lâché : saut");
            arreterEscalade(); return;
        }
        if (controller.isGrounded && input.y < -0.1f)
        {
            Debug.Log("lâché : sol");
            arreterEscalade(); return;
        }

        // le haut du mur est atteint ?
        if (input.y > -0.1f && detecterRebord(transform.position, out RaycastHit murRebord, out float yHaut))
        {
            routine = StartCoroutine(accrocher(murRebord, yHaut));
            return;
        }

        Vector3 normal = infoParois.normal;

        // se tourner face à la paroi
        Vector3 face = -normal; face.y = 0f;
        if (face.sqrMagnitude > 0.001f)
        {
            Quaternion cible = Quaternion.LookRotation(face);
            transform.rotation = Quaternion.Slerp(transform.rotation, cible, vitesseRotation * Time.deltaTime);
        }

        // déplacement sur la paroi
        Vector3 droite = Vector3.Cross(Vector3.up, -normal).normalized;
        Vector3 mouvement = (droite * input.x + Vector3.up * input.y) * vitesseGrimper;

        // correction de la distance au mur
        float ecart = infoParois.distance - distanceAuMur;
        mouvement += transform.forward * ecart * vitessePlaquage;
        controller.Move(mouvement * Time.deltaTime);

        if (animator)
        {
            animator.SetFloat("grimperX", input.x, 0.1f, Time.deltaTime);
            animator.SetFloat("grimperY", input.y, 0.1f, Time.deltaTime);
        }
    }

    // ------------------------------------------------------------- REBORD

    bool detecterRebord(Vector3 pieds, out RaycastHit mur, out float yHaut)
    {
        yHaut = 0f;
        Vector3 dir = transform.forward;

        // 1. mur devant, à hauteur de poitrine
        Vector3 poitrine = pieds + Vector3.up;
        bool murTrouve = Physics.Raycast(poitrine, dir, out mur, distanceDetection, coucheParoi);
        Debug.DrawRay(poitrine, dir * distanceDetection, murTrouve ? Color.green : Color.red);
        if (!murTrouve) return false;

        // 2. au-dessus des mains : l'espace doit être libre
        Vector3 hautMains = pieds + Vector3.up * (hauteurMains + 0.15f);
        bool libre = !Physics.Raycast(hautMains, dir, distanceDetection, coucheParoi);
        Debug.DrawRay(hautMains, dir * distanceDetection, libre ? Color.green : Color.red);
        if (!libre) return false;

        // 3. le dessus du mur, vers le bas, juste derrière le bord
        Vector3 dessus = hautMains + Vector3.up * 0.5f + dir * (mur.distance + 0.3f);
        bool dessusTrouve = Physics.Raycast(dessus, Vector3.down, out RaycastHit sol, 1.5f, coucheParoi);
        Debug.DrawRay(dessus, Vector3.down * 1.5f, dessusTrouve ? Color.green : Color.red);
        if (!dessusTrouve || sol.normal.y < 0.7f) return false;

        yHaut = sol.point.y;
        return Mathf.Abs(yHaut - (pieds.y + hauteurMains)) < toleranceRebord;
    }

    IEnumerator accrocher(RaycastHit mur, float yHaut)
    {
        etat = Etat.Accrochage;
        yRebord = yHaut;
        normaleMur = mur.normal;
        animator.SetBool("arriverRebord", true);

        Vector3 depart = transform.position;
        Vector3 cible = mur.point + mur.normal * distanceAuMur;
        cible.y = yHaut - hauteurMains;

        Vector3 face = -mur.normal; face.y = 0f;
        Quaternion rotCible = Quaternion.LookRotation(face);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.2f;
            float k = Mathf.Clamp01(t);
            controller.Move(Vector3.Lerp(depart, cible, k) - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotCible, k);
            yield return null;
        }
        etat = Etat.Rebord;
    }

    void gererRebord()
    {
        if (input.y < -0.5f) { arreterEscalade(); return; }          // bas = lâcher

        if (input.y > 0.5f || sautAction.WasPressedThisFrame())      // haut ou saut = monter
        {
            routine = StartCoroutine(monterRebord());
            return;
        }

        bool ok = detecterRebord(transform.position, out RaycastHit mur, out float yHaut);
        if (ok) { yRebord = yHaut; normaleMur = mur.normal; }

        Vector3 droite = Vector3.Cross(Vector3.up, -normaleMur).normalized;
        float dx = ok ? input.x : 0f;

        // on s'arrête au bout du rebord au lieu de tomber
        if (Mathf.Abs(dx) > 0.1f)
        {
            Vector3 prochain = transform.position + droite * Mathf.Sign(dx) * 0.4f;
            if (!detecterRebord(prochain, out _, out _)) dx = 0f;
        }

        Vector3 mouvement = droite * dx * vitesseRebord;
        float ecart = ok ? mur.distance - distanceAuMur : 0f;
        mouvement += transform.forward * ecart * vitessePlaquage;
        mouvement.y = ((yRebord - hauteurMains) - transform.position.y) * 10f;
        controller.Move(mouvement * Time.deltaTime);

        Vector3 face = -normaleMur; face.y = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(face),
                                              vitesseRotation * Time.deltaTime);

        animator.SetFloat("rebordX", dx, 0.1f, Time.deltaTime);
    }

    IEnumerator monterRebord()
    {
        etat = Etat.Montee;
        playerController.enMontee = true;
        animator.SetTrigger("monterRebord");

        float timeout = 4f;

        // attendre d'être entré dans l'état de montée
        while (timeout > 0f && !animator.GetCurrentAnimatorStateInfo(0).IsName(nomEtatMontee))
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        Vector3 avant = -normaleMur; avant.y = 0f; avant.Normalize();
        Vector3 depart = transform.position;
        Vector3 milieu = new Vector3(depart.x, yRebord + 0.05f, depart.z);   // d'abord on monte...
        Vector3 fin = milieu + avant * distanceMontee;                        // ...puis on avance

        // le déplacement suit la progression de l'animation
        while (timeout > 0f)
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            if (!info.IsName(nomEtatMontee)) break;

            float k = Mathf.Clamp01(info.normalizedTime);
            Vector3 voulu = k < partieVertical
                ? Vector3.Lerp(depart, milieu, k / partieVertical)
                : Vector3.Lerp(milieu, fin, (k - partieVertical) / (1f - partieVertical));

            controller.Move(voulu - transform.position);

            if (k >= 1f) break;
            timeout -= Time.deltaTime;
            yield return null;
        }

        controller.Move(fin - transform.position);   // position finale exacte
        playerController.enMontee = false;
        terminerEscalade();
    }

    // -------------------------------------------------------------- FIN

    // lâcher (volontaire ou endurance à 0)
    public void arreterEscalade()
    {
        if (etat == Etat.Montee) return;                  // pas de lâcher en pleine montée
        if (routine != null) StopCoroutine(routine);

        etat = Etat.Aucun;
        estEnTrainDeGrimper = false;
        playerController.enEscalade = false;
        playerController.resetPhysique();
        tempsFinEscalade = Time.time + delaiRegrab;

        if (animator)
        {
            animator.SetBool("grimper", false);
            animator.SetBool("arriverRebord", false);

            if (!controller.isGrounded)
            {
                animator.SetTrigger("lacher");
                if (bloquageLacher != null) StopCoroutine(bloquageLacher);
                bloquageLacher = StartCoroutine(bloquerInputPendantLacherEscalade());
            }
            else
            {
                animator.ResetTrigger("lacher");
            }
        }
    }

    // fin de montée : pas d'anim "lacher", pas de blocage d'input
    void terminerEscalade()
    {
        etat = Etat.Aucun;
        estEnTrainDeGrimper = false;
        playerController.enEscalade = false;
        playerController.resetPhysique();
        tempsFinEscalade = Time.time + delaiRegrab;

        animator.SetBool("grimper", false);
        animator.SetBool("arriverRebord", false);
    }

    IEnumerator bloquerInputPendantLacherEscalade()
    {
        playerController.inputBloquer = true;
        yield return new WaitForSeconds(dureeLacher);
        playerController.inputBloquer = false;
    }
}
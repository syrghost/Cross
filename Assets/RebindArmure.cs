using System.Collections.Generic;
using UnityEngine;

public class RebindArmure : MonoBehaviour
{
    [Tooltip("mixamorig:Hips du joueur (heroTesteV2)")]
    public Transform racineJoueur;

    [Tooltip("Le GameObject 'Armature' copié sous cette pièce (sera supprimé)")]
    public Transform armatureCopie;

    [Tooltip("À cocher seulement si la pièce arrive avec un décalage d'échelle")]
    public bool recalculerBindposes = false;

    void Awake()
    {
        if (racineJoueur == null || armatureCopie == null)
        {
            Debug.LogError("RebindArmure : racineJoueur ou armatureCopie manquant sur " + name);
            return;
        }

        // on cherche le mesh depuis la racine de la pièce (le parent de la copie du squelette)
        Transform racinePiece = armatureCopie.parent != null ? armatureCopie.parent : transform;
        var meshes = racinePiece.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        if (meshes.Length == 0)
        {
            Debug.LogError("RebindArmure : aucun SkinnedMeshRenderer trouvé sous " + racinePiece.name + ", rien n'a été modifié.");
            return;
        }

        // nom -> os du joueur (on ignore la copie du squelette de l'armure)
        var osJoueur = new Dictionary<string, Transform>();
        foreach (Transform t in racineJoueur.GetComponentsInChildren<Transform>(true))
        {
            if (t.IsChildOf(armatureCopie)) continue;
            osJoueur[t.name] = t;
        }

        foreach (var smr in meshes)
        {
            if (smr.sharedMesh == null) continue;

            Transform[] anciens = smr.bones;
            Transform[] nouveaux = new Transform[anciens.Length];

            for (int i = 0; i < anciens.Length; i++)
            {
                if (anciens[i] == null || !osJoueur.TryGetValue(anciens[i].name, out nouveaux[i]))
                {
                    Debug.LogWarning("Os introuvable chez le joueur : "
                        + (anciens[i] != null ? anciens[i].name : "null") + " (" + name + ")");
                    nouveaux[i] = racineJoueur;
                }
            }

            if (recalculerBindposes)
            {
                Mesh mesh = Instantiate(smr.sharedMesh);   // copie, l'asset reste intact
                Matrix4x4[] bindposes = new Matrix4x4[nouveaux.Length];
                for (int i = 0; i < nouveaux.Length; i++)
                    bindposes[i] = nouveaux[i].worldToLocalMatrix * smr.transform.localToWorldMatrix;
                mesh.bindposes = bindposes;
                smr.sharedMesh = mesh;
            }

            smr.bones = nouveaux;

            if (smr.rootBone != null && osJoueur.TryGetValue(smr.rootBone.name, out Transform r))
                smr.rootBone = r;
        }

        Destroy(armatureCopie.gameObject);   // la copie ne sert plus
    }
}
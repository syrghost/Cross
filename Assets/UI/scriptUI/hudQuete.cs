using System.Text;
using TMPro;
using UnityEngine;

public class hudQuete : MonoBehaviour
{
    public TMP_Text texteQuetes;

    void OnEnable()  { GestionnaireQuete.OnQueteChange += Rafraichir; }
    void OnDisable() { GestionnaireQuete.OnQueteChange -= Rafraichir; }
    void Start()     { Rafraichir(null); }

    void Rafraichir(quete _)
    {
        var gq = GestionnaireQuete.instance;
        var sb = new StringBuilder();

        foreach (quete q in gq.quetesActives())
        {
            sb.AppendLine(q.titre);

            if (gq.Etat(q) == etatQuete.terminer)
            {
                sb.AppendLine("  Objectifs remplis !");
                continue;
            }

            for (int i = 0; i < q.objectifs.Length; i++)
            {
                Objectif o = q.objectifs[i];
                string nom = string.IsNullOrEmpty(o.description) ? o.type + " " + o.idCible : o.description;
                sb.AppendLine("  " + nom + " " + gq.Progression(q, i) + "/" + o.nombreRequis);
            }
        }

        texteQuetes.text = sb.ToString();
    }
}
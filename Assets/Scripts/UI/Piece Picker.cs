using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class Piece_Picker : MonoBehaviour
    {

        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text textname;
        [SerializeField] private TMP_Text cost;

        public void Set(PiecePrefabData data)
        {
            icon.sprite = data.icon;
            textname.text = string.Format("{0}", data.pieceType.ToString());
            cost.text = string.Format("$ {0}", data.cost);
        }
    }
}
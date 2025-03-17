using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "Piece", menuName = "Data/Pieces")]
    public class PiecePrefabData : ScriptableObject
    {
        [SerializeField] public PieceType pieceType;
        [SerializeField] public GameObject prefab;
        [SerializeField] public Sprite icon;
        [SerializeField] public int cost;
    }
}
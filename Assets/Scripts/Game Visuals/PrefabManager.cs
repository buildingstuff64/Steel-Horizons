using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game_Visuals
{
    public class PrefabManager : MonoBehaviour
    {
        public static PrefabManager instance;

        [Header("Piece Prefabs")]
        public List<PiecePrefabData> pieces = new List<PiecePrefabData>();

        [Header("Animations Prefabs")]
        [SerializeField] private GameObject projectilePrefab;

        [Header("Board Prefabs")]
        public GameObject gridViewPrefab;
        public GameObject subBoardPrefab;
        public GameObject rotationMarker;

        [Header("Sceneobjects")]
        public GameObject Camera;

        private void Awake()
        {
            instance = this;
        }
        
        public GameObject getPiecePrefab(PieceType t)
        {
            foreach (PiecePrefabData data in pieces)
            {
                if (data.pieceType == t)
                {
                    return data.prefab;
                }
            }
            return null;
        }

        public GameObject createProjectile(Transform t)
        {
            GameObject g = Instantiate(projectilePrefab, t);
            return g;
        }
    }
}
using Assets.Scripts.Game_Visuals;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(BoardViewer))]
    public class BoardManager : MonoBehaviour
    {
        public BoardViewer Viewer;
        public Board board;

    }
}
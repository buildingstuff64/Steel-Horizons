using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Game_Logic;
using static Assets.Scripts.Game_Visuals.PrefabManager;
//using Assets.Scripts.PiecePrefabData;

namespace Assets.Scripts.Game_Logic.FormationMaker{
    
    public class CostSystem : MonoBehaviour
    {
            private int piececost;
            
            public int getPieceCost(PieceType t){

                switch(t){
                case PieceType.Mech: return 1;
                case PieceType.Sniper: return 3;
                case PieceType.Shield: return 3;
                case PieceType.Teleporter: return 2;
                case PieceType.Support: return 2;
                case PieceType.Battleship: return 1;
                case PieceType.Destroyer: return 3;
                case PieceType.ShieldShip: return 3;
                case PieceType.Submarine: return 2;
                case PieceType.Carrier: return 2;
                default: return 0;
                }
            }
    } 
}
using Assets.Scripts.Game_Logic.SubPieces;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Game_Visuals.Visual_Sub_Pieces
{
    public abstract class VisualPiece : MonoBehaviour
    {
        public Piece piece;
        public object data;
        public BoardViewer viewer;

        public void setVisuals()
        {
            foreach (var child in transform.GetComponentsInChildren<Transform>())
            {
                if (child.name == "Team")
                {
                    child.GetComponent<MeshRenderer>().material.DOColor(piece.team, 1f);
                }

                if (child.name == "Team2")
                {
                    child.GetComponent<MeshRenderer>().material.DOColor(Color.Lerp(child.GetComponent<MeshRenderer>().material.color, piece.team, 0.5f), 1f);
                }

                if (child.GetComponent<TMP_Text>() != null)
                {
                    child.GetComponent<TMP_Text>().DOColor(piece.team, 1f);
                }
            }
        }

        virtual public void PlayMoveAnimation(Square from, Square to, Vector3 rotation, Action onComplete)
        {
            Sequence moveSequence = DOTween.Sequence();
            moveSequence.Append(transform.DOMove(to.position, 1f).SetEase(Ease.InOutSine));
            moveSequence.Append(transform.DORotate(rotation, 0.25f).SetEase(Ease.InOutSine));
            moveSequence.OnComplete(() => { onComplete.Invoke(); });
        }

        virtual public void PlayAttackAnimation(VisualPiece from, VisualPiece to, Action onComplete)
        {
            DestroyImmediate(to);
            onComplete.Invoke();
        }

        virtual public void PlayTurnStartAnimation(Action onComplete)
        {
            onComplete.Invoke();
        }

        public static Type getVisualPieceType(PieceType type)
        {
            switch (type)
            {
                case PieceType.None:
                    Debug.Log("No piece selected.");
                    break;

                case PieceType.Mech:
                    return typeof(VisualMech);

                case PieceType.Sniper:
                    return typeof(VisualSniper);

                case PieceType.Shield:
                    return typeof(VisualShield);

                case PieceType.Teleporter:
                    return typeof(VisualTeleporter);

                case PieceType.Support:
                    return typeof(VisualSupport);

                case PieceType.Battleship:
                    return typeof(VisualMech);

                case PieceType.Destroyer:
                    return typeof(VisualMech);

                case PieceType.ShieldShip:
                    return typeof(VisualMech);

                case PieceType.Submarine:
                    return typeof(VisualSubmarine);

                case PieceType.Carrier:
                    return typeof(VisualMech);

                case PieceType.Army:
                    return typeof(VisualMech);

                case PieceType.Structure:
                    return typeof(VisualStructure);
                default:
                    Debug.LogWarning("Unknown piece type!");
                    break;
            }
            return null;
        }

        public void PlayPathAnimation(List<Square> path, Action onComplete)
        {
            Sequence moveSequence = DOTween.Sequence();
            List<Vector3> pathPosition = new List<Vector3>();
            foreach (Square square in path)
            {
                pathPosition.Add(square.position);
            }
            transform.DOPath(pathPosition.ToArray(), 2f).SetEase(Ease.InOutSine).OnComplete(() => onComplete.Invoke());
        }

    }
}
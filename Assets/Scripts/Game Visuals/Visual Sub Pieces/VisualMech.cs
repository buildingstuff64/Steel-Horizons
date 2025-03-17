using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game_Visuals.Visual_Sub_Pieces
{
    public class VisualMech : VisualPiece
    {
        public override void PlayAttackAnimation(VisualPiece from, VisualPiece to, Action onComplete)
        {
            Sequence moveSequence = DOTween.Sequence();
            moveSequence.Append(transform.DOMove(to.piece.square.position, 1f).SetEase(Ease.InOutSine));
            moveSequence.OnComplete(() => 
            { 
                viewer.removePiece(to.piece);
                onComplete.Invoke();
            });
            
        }

    }
}
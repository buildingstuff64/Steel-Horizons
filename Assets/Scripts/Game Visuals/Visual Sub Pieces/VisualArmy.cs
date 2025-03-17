using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game_Visuals.Visual_Sub_Pieces
{
    public class VisualArmy : VisualPiece
    {
        public override void PlayMoveAnimation(Square from, Square to, Vector3 rotation, Action onComplete)
        {
            Sequence moveSequence = DOTween.Sequence();
            moveSequence.Append(transform.DOMoveX(to.piece.square.position.x, 1f).SetEase(Ease.InSine));
            moveSequence.Append(transform.DOMoveZ(to.piece.square.position.z, 1f).SetEase(Ease.OutSine));
            moveSequence.OnComplete(() =>
            {
                onComplete.Invoke();
            });
        }

    }
}
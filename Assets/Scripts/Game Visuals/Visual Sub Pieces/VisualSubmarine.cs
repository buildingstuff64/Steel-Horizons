using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game_Visuals.Visual_Sub_Pieces
{
    public class VisualSubmarine : VisualPiece
    {
        public override void PlayAttackAnimation(VisualPiece from, VisualPiece to, Action onComplete)
        {
            GameObject g = PrefabManager.instance.createProjectile(transform);
            g.transform.position = from.transform.position + Vector3.up * 0.25f;
            g.transform.DOMove(to.transform.position + Vector3.up * 0.25f, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                Destroy(g);
                viewer.removePiece(to.piece);
                onComplete.Invoke();
            });
            transform.DOShakePosition(0.5f, 0.1f);
        }

        public override void PlayMoveAnimation(Square from, Square to, Vector3 rotation, Action onComplete)
        {
            Sequence moveSequence = DOTween.Sequence();
            moveSequence.Append(transform.DOMoveY(-0.25f, 2f));
            moveSequence.Append(transform.DOMove(to.position + Vector3.down*0.25f, 0f));
            moveSequence.Append(transform.DORotate(rotation, 0f));
            moveSequence.Append(transform.DOMoveY(0, 2f));
            moveSequence.OnComplete(() => onComplete.Invoke());
        }
    }
}
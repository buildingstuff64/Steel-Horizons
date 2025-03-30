using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game_Visuals.Visual_Sub_Pieces
{
    public class VisualTeleporter : VisualPiece
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
            moveSequence.Append(transform.DOShakeScale(0.5f, 0.5f));

            moveSequence.Join(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InExpo));
            moveSequence.Join(transform.DOMove(transform.position + transform.forward, 0.5f).SetEase(Ease.InExpo));

            moveSequence.Append(transform.DORotate(rotation, 0f));
            moveSequence.Append(transform.DOMove(to.position - piece.lookDirection, 0f));

            moveSequence.Join(transform.DOMove(to.position, 0.5f).SetEase(Ease.OutExpo));
            moveSequence.Join(transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutExpo));

            moveSequence.Append(transform.DOShakeScale(0.5f, 0.25f));
            moveSequence.OnComplete(() => onComplete.Invoke());
        }
    }
}
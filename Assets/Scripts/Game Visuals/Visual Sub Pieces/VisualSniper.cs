using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game_Visuals.Visual_Sub_Pieces
{
    public class VisualSniper : VisualPiece
    {
        public override void PlayAttackAnimation(VisualPiece from, VisualPiece to, Action onComplete)
        {
            if (!from.piece.isSupported)
            {
                GameObject g = PrefabManager.instance.createProjectile(transform);
                g.transform.position = transform.position + Vector3.up * 0.25f;
                g.transform.DOMove(to.piece.square.position + Vector3.up * 0.25f, 1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    Destroy(g);
                    viewer.removePiece(to.piece);
                    onComplete.Invoke();
                });
                transform.DOShakePosition(0.5f, 0.1f);
            }
            else
            {
                List<Square> squares = from.piece.getAttackDirection(from.piece.square, from.piece.lookDirection, true);
                GameObject g = PrefabManager.instance.createProjectile(transform);
                g.transform.position = transform.position + Vector3.up * 0.25f;

                Sequence seq = DOTween.Sequence();
                foreach (var item in squares)
                {
                    seq.Append(g.transform.DOMove(item.position + Vector3.up * 0.25f, 1f / (float)squares.Count).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        viewer.removePiece(item.piece);
                    }));
                }
                seq.OnComplete(() =>
                {
                    Destroy(g);
                    onComplete.Invoke();
                });
                seq.Play();

                transform.DOShakePosition(0.5f, 0.1f);
            }
        }

    }
}
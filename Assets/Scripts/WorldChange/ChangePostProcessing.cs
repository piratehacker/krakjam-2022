﻿using System;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.Rendering;

namespace WorldChange
{
    public class ChangePostProcessing : WorldChangeLogic
    {
        [SerializeField] private WorldTypeDict<Volume> volumes;
        [SerializeField] private float transitionDuration;

        private void Start()
        {
        }

        public override void OnWorldTypeChange(WorldTypeController.WorldType type)
        {
            var from = volumes.GetInverse(type);
            var to = volumes[type];

            DOTween.To(() => to.weight, (v) => to.weight = v, 1, transitionDuration).SetEase(Ease.InOutCubic);
            DOTween.To(() => from.weight, (v) => from.weight = v, 0, transitionDuration).SetEase(Ease.InOutCubic);
        }
    }
}

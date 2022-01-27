﻿using InteractiveObjects;
using Player;
using UnityEngine;

namespace UsableItems
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class UsableItem : InteractiveObject
    {
        public abstract void Use();

        public virtual void OnPickup()
        {
        }

        public virtual void OnDrop()
        {
        }

        protected override bool OnInteract()
        {
            HandController.Current.PickUpItem(this);
            return true;
        }
    }
}

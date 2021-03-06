﻿using UnityEngine;

namespace Assets.Scripts.Arena.Tiles
{
    public class BreakableTile : Deployable
    {
        private float _health = 100f;


        [InGameProperty(Name = "Health")]
        public float Health
        {
            get { return _health; }
            set
            {
                _health = value;
                if (_health <= 0)
                {
                    Destroy(gameObject);
                }

                UpdateListOfProperties();
            }
        }

        public override string GetDisplayName()
        {
            return "Breakable Tile";
        }


        public void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Player")
            {
                Health -= 30f;
            }
        }
    }
}
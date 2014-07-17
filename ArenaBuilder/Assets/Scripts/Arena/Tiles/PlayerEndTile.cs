using UnityEngine;

namespace Assets.Scripts.Arena.Tiles
{
    public class PlayerEndTile : Deployable
    {
        public override string GetDisplayName()
        {
            return "Player Destination";
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                AdvanceBrain.Instance.PlayerReachedDestination();
            }
        }
    }
}
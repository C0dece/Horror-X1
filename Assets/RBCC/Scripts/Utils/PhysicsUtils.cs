using RBCC.Scripts.Environment.Platforms;
using UnityEngine;

namespace RBCC.Scripts.Utils
{
    public class PhysicsUtils
    {
        /// <summary>
        /// Recursive function to get the relative point velocity of an object.
        /// Velocity is based on rigidbodies or MovingPlatform script.
        /// For example considering :
        /// - Root
        ///     - Object1
        ///         -Object2
        ///     - Player
        /// GetRelativePointVelocity(Player, Object2) returns the velocity relative to the player.
        /// That means that if Root is moving, the velocity returned is not the world velocity.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="movingObject"></param>
        /// <param name="point">world point in global space</param>
        /// <returns></returns>
        public static Vector3 GetRelativePointVelocity(Transform reference, GameObject movingObject, Vector3 point)
        {
            Vector3 vel = Vector3.zero;
        
            Rigidbody r = movingObject.GetComponent<Rigidbody>();
            if (r != null)
            {
                vel = r.GetPointVelocity(point);
            }

            MovingPlatformComponent mp = movingObject.GetComponent<MovingPlatformComponent>();
            if (mp != null)
            {
                vel = mp.Velocity;
            }


            Transform parent = movingObject.transform.parent;
        
            if (parent != null && parent != reference.parent)
            {
                return vel + GetRelativePointVelocity(reference, parent.gameObject, point);
            }
            else
            {
                return vel;
            }
        }
    }
}

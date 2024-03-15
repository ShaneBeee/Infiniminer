using UnityEngine;

namespace World.Entity {

    public class Entity : MonoBehaviour {

        // OBJECTS
        [Header("Entity Game Objects")] 
        [SerializeField] internal World world;

        // VALUES
        [Header("Entity Values")] 
        [SerializeField] internal float walkSpeed = 7f;

        [SerializeField] internal float sprintSpeed = 14f;
        [SerializeField] internal float jumpForce = 12f;
        [SerializeField] internal float gravity = 9.81f;
        internal bool isGrounded;

        /// <summary>
        /// Teleport the entity to a new position
        /// </summary>
        /// <param name="pos">new position</param>
        public void Teleport(Vector3 pos) {
            transform.position = pos;
        }

    }

}
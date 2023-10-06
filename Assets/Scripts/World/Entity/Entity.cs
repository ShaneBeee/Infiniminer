using UnityEngine;

namespace World.Entity {

    public class Entity : MonoBehaviour {

        // OBJECTS
        [SerializeField] protected World world;

        // VALUES
        [SerializeField] protected float walkSpeed = 7f;
        [SerializeField] protected float sprintSpeed = 6f;
        [SerializeField] protected float jumpForce = 12f;
        [SerializeField] protected float gravity = 9.81f;
        [SerializeField] public bool isGrounded;
    }

}
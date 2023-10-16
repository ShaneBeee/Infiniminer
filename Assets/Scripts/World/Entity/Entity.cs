using UnityEngine;

namespace World.Entity {

    public class Entity : MonoBehaviour {

        // OBJECTS
        [Header("Entity Game Objects")]
        [SerializeField] protected World world;

        // VALUES
        [Header("Entity Values")]
        [SerializeField] protected float walkSpeed = 7f;
        [SerializeField] protected float sprintSpeed = 14f;
        [SerializeField] protected float jumpForce = 12f;
        [SerializeField] protected float gravity = 9.81f;
        [SerializeField] public bool isGrounded;
        
    }

}
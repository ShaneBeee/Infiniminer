using UnityEngine;

namespace World.Entity {

    public class Entity : MonoBehaviour {

        // OBJECTS
        [SerializeField] protected World world;

        // VALUES
        [SerializeField] protected float walkSpeed = 3f;
        [SerializeField] protected float sprintSpeed = 6f;
        [SerializeField] protected float jumpForce = 5f;
        [SerializeField] protected float gravity = 9.81f;
        [SerializeField] protected float entityWidth = 0.3f;
        [SerializeField] public bool isGrounded;

        protected float CheckDownSpeed(float downSpeed) {
            var transformPosition = transform.position;
            var x = transformPosition.x;
            var y = transformPosition.y;
            var z = transformPosition.z;
            var width = entityWidth;
            if (world.CheckForBlock(x - width, y + downSpeed, z - width) ||
                world.CheckForBlock(x + width, y + downSpeed, z - width) ||
                world.CheckForBlock(x + width, y + downSpeed, z + width) ||
                world.CheckForBlock(x - width, y + downSpeed, z + width)) {
                isGrounded = true;
                return 0;
            }

            isGrounded = false;
            return downSpeed;
        }

        protected float CheckUpSpeed(float upSpeed) {
            var transformPosition = transform.position;
            var x = transformPosition.x;
            var y = transformPosition.y;
            var z = transformPosition.z;
            var width = entityWidth;
            if (world.CheckForBlock(x - width, y + 2f + upSpeed, z - width) ||
                world.CheckForBlock(x + width, y + 2f + upSpeed, z - width) ||
                world.CheckForBlock(x + width, y + 2f + upSpeed, z + width) ||
                world.CheckForBlock(x - width, y + 2f + upSpeed, z + width)) {
                return 0;
            }

            return upSpeed;
        }

        private const float BlockCheckOffset = 0.1f;

        protected bool CanMoveNorth() {
            var pos = transform.position;
            var width = entityWidth;
            var ne = pos + new Vector3(width, 0, width + BlockCheckOffset);
            var nw = pos + new Vector3(-width, 0, width + BlockCheckOffset);

            return !world.CheckForBlock(ne) && !world.CheckForBlock(nw) &&
                   !world.CheckForBlock(ne + Vector3.up) && !world.CheckForBlock(nw + Vector3.up);
        }

        protected bool CanMoveEast() {
            var pos = transform.position;
            var width = entityWidth;
            var ne = pos + new Vector3(width + BlockCheckOffset, 0, width);
            var se = pos + new Vector3(width + BlockCheckOffset, 0, -width);

            return !world.CheckForBlock(ne) && !world.CheckForBlock(se) &&
                   !world.CheckForBlock(ne + Vector3.up) && !world.CheckForBlock(se + Vector3.up);
        }

        protected bool CanMoveSouth() {
            var pos = transform.position;
            var width = entityWidth;
            var sw = pos + new Vector3(-width, 0, -width - BlockCheckOffset);
            var se = pos + new Vector3(width, 0, -width - BlockCheckOffset);

            return !world.CheckForBlock(sw) && !world.CheckForBlock(se) &&
                   !world.CheckForBlock(sw + Vector3.up) && !world.CheckForBlock(se + Vector3.up);
        }

        protected bool CanMoveWest() {
            var pos = transform.position;
            var width = entityWidth;
            var sw = pos + new Vector3(-width - BlockCheckOffset, 0, -width);
            var nw = pos + new Vector3(-width - BlockCheckOffset, 0, width);

            return !world.CheckForBlock(sw) && !world.CheckForBlock(nw) &&
                   !world.CheckForBlock(sw + Vector3.up) && !world.CheckForBlock(nw + Vector3.up);
        }
    }

}
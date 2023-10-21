using System;
using UnityEngine;
using World.Block;

namespace World.Entity {
    public class PlayerController : MonoBehaviour {

        public Player player;

        [SerializeField] private Camera playerCamera;
        private Rigidbody rigidBody;
        private BoxCollider playerCollider;

        private float mouseHorizontal;
        private float mouseVertical;
        private float jumpKeyLastPressed;
        private Gamemode previousGamemode;

        private void Awake() {
            rigidBody = player.gameObject.GetComponent<Rigidbody>();
            rigidBody.useGravity = false;
            player.LockCursor(true);

            playerCollider = player.gameObject.GetComponent<BoxCollider>();
            if (player.gamemode == Gamemode.SPECTATOR) {
                playerCollider.enabled = false;
            }

            previousGamemode = player.gamemode;
        }

        private void Update() {
            if (player.IsMenuOpen()) return;
            GetPlayerInput();
            if (player.gamemode != Gamemode.SPECTATOR) {
                PlaceCursorBlock();
            }

            if (player.gamemode != previousGamemode) {
                playerCollider.enabled = player.gamemode != Gamemode.SPECTATOR;
                previousGamemode = player.gamemode;
            }

            // Double jump quickly to enable/disable flight mode
            if (Input.GetKeyDown(KeyCode.Space) && player.gamemode == Gamemode.CREATIVE) {
                // Max double jump delay for flight mode
                if (Time.time - jumpKeyLastPressed < 0.35f) {
                    player.isFlying = !player.isFlying;
                    if (player.isFlying) {
                        // reset velocity so we don't fly away
                        ResetVelocity();
                    }
                }

                jumpKeyLastPressed = Time.time;
            }
        }

        private void FixedUpdate() {
            if (player.IsMenuOpen()) return;

            // Check ability to fly
            if (player.gamemode == Gamemode.SURVIVAL) {
                player.isFlying = false;
            }

            // Apply gravity
            if (player.gamemode != Gamemode.SPECTATOR && !player.isFlying) {
                var pull = Vector3.down * (player.gravity * rigidBody.mass);
                rigidBody.AddForce(pull, ForceMode.Acceleration);
            }

            // Check if player is on ground
            var playerBlockPos = transform.position.ToVector3Int();
            var blockBelow = playerBlockPos + Vector3Int.down;
            if (player.world.GetBlock(blockBelow) != Blocks.AIR &&
                Math.Abs((transform.position.y) - playerBlockPos.y) < 0.01) {
                player.isGrounded = true;
                player.isFlying = false;
            } else {
                player.isGrounded = false;
            }

            // Handle jumping
            if (Input.GetKey(KeyCode.Space)) {
                if ((player.gamemode == Gamemode.SURVIVAL || player.gamemode == Gamemode.CREATIVE) &&
                    player.isGrounded) {
                    player.isGrounded = false;
                    rigidBody.AddForce(Vector3.up * (player.jumpForce * 100));
                } else if ((player.gamemode == Gamemode.CREATIVE && player.isFlying) ||
                           player.gamemode == Gamemode.SPECTATOR) {
                    var pos = rigidBody.position;
                    pos.y += 0.1f;
                    rigidBody.Move(pos, Quaternion.identity);
                }
            } else if (Input.GetKey(KeyCode.LeftShift)) {
                if (player.gamemode == Gamemode.SPECTATOR ||
                    (player.gamemode == Gamemode.CREATIVE && player.isFlying)) {
                    var pos = rigidBody.position;
                    pos.y -= 0.1f;
                    rigidBody.Move(pos, Quaternion.identity);
                }
            }
        }

        private void LateUpdate() {
            if (player.IsMenuOpen()) {
                ResetVelocity();
                return;
            }

            // Rotate player horizontal
            player.playerBody.Rotate(Vector3.up * (mouseHorizontal * 5.0f));

            // Rotate camera vertical
            var rotY = Mathf.Clamp(mouseVertical * 5.0f, -90f, 90f);
            playerCamera.transform.localRotation = Quaternion.Euler(-rotY, 0f, 0f);

            // Move player
            var speed = player.isSprinting ? player.sprintSpeed : player.walkSpeed;
            if (player.isFlying) speed *= 1.5f;
            var vel = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed;
            vel.y = rigidBody.velocity.y;
            // If the player stops moving, stop their sprinting
            if (vel.z <= player.walkSpeed) {
                player.isSprinting = false;
            }

            vel = player.playerBody.TransformDirection(vel);
            rigidBody.velocity = vel;
        }

        private void ResetVelocity() {
            rigidBody.velocity = new Vector3(0, 0, 0);
        }

        private void GetPlayerInput() {
            mouseHorizontal = Input.GetAxis("Mouse X");
            mouseVertical += Input.GetAxis("Mouse Y");

            if (Input.GetKeyDown(KeyCode.LeftControl)) {
                player.isSprinting = !player.isSprinting;
            }

            // Place/Break blocks
            if (player.highlightBlock.gameObject.activeSelf) {
                if (Input.GetMouseButtonDown(0)) {
                    player.world.SetBlock(player.highlightBlock.position.ToVector3Int(), Blocks.AIR);
                } else if (Input.GetMouseButtonDown(1) && player.placeBlock.gameObject.activeSelf) {
                    player.world.SetBlock(player.placeBlock.position.ToVector3Int(), Blocks.DIRT);
                }
            }
        }

        private void PlaceCursorBlock() {
            var step = player.checkIncrement;
            var lastPos = new Vector3();

            while (step < player.reach) {
                var transform1 = playerCamera.transform;
                var pos = transform1.position + (transform1.forward * step);

                if (player.world.CheckForBlock(pos.ToVector3Int())) {
                    player.highlightBlock.position = pos.ToVector3Int();
                    player.placeBlock.position = lastPos;

                    player.highlightBlock.gameObject.SetActive(true);
                    if (transform.position.ToVector3Int() != player.placeBlock.position.ToVector3Int()) {
                        player.placeBlock.gameObject.SetActive(true);
                    } else {
                        player.placeBlock.gameObject.SetActive(false);
                    }

                    return;
                }

                lastPos = pos.ToVector3Int();
                step += player.checkIncrement;
            }

            player.highlightBlock.gameObject.SetActive(false);
            player.placeBlock.gameObject.SetActive(false);
        }
    }
}
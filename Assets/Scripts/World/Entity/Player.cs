using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using World.Block;

namespace World.Entity {

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Player : Entity {

        // OBJECTS
        [Header("Player Game Objects")] [SerializeField]
        private GameObject menuScreen;

        [SerializeField] private Transform highlightBlock;
        [SerializeField] private Transform placeBlock;
        [SerializeField] private Transform playerBody;
        private Camera playerCamera;
        private Rigidbody rigidBody;
        private BoxCollider playerCollider;

        // VALUES
        [Header("Player Values")] [SerializeField]
        private float checkIncrement = 0.1f;

        [SerializeField] private float reach = 8f;
        [SerializeField] private Gamemode gamemode;

        private bool isSprinting;
        private bool isFlying;
        private float _mouseHorizontal;
        private float _mouseVertical;
        private float jumpKeyLastPressed;

        private void Start() {
            var cameraObject = gameObject.GetComponentInChildren<Camera>();
            if (cameraObject != null) {
                this.playerCamera = cameraObject;
            } else {
                Debug.LogError("Camera on player == null");
            }

            this.rigidBody = gameObject.GetComponent<Rigidbody>();
            this.rigidBody.useGravity = false;
            LockCursor(true);

            this.playerCollider = GetComponent<BoxCollider>();
            if (gamemode == Gamemode.SPECTATOR) {
                this.playerCollider.enabled = false;
            }
        }

        private void Update() {
            if (IsMenuOpen()) return;
            GetPlayerInput();
            if (gamemode != Gamemode.SPECTATOR) {
                PlaceCursorBlock();
            }

            // Double jump quickly to enable/disable flight mode
            if (Input.GetKeyDown(KeyCode.Space) && gamemode == Gamemode.CREATIVE) {
                // Max double jump delay for flight mode
                if (Time.time - jumpKeyLastPressed < 0.35f) {
                    isFlying = !isFlying;
                    if (isFlying) {
                        // reset velocity so we don't fly away
                        ResetVelocity();
                    }
                }

                jumpKeyLastPressed = Time.time;
            }
        }

        private void FixedUpdate() {
            if (IsMenuOpen()) return;

            // Check ability to fly
            if (gamemode == Gamemode.SURVIVAL) {
                isFlying = false;
            }

            // Apply gravity
            if (gamemode != Gamemode.SPECTATOR && !isFlying) {
                var pull = Vector3.down * (gravity * rigidBody.mass);
                rigidBody.AddForce(pull, ForceMode.Acceleration);
            }

            // Check if player is on ground
            var playerBlockPos = transform.position.ToVector3Int();
            var blockBelow = playerBlockPos + Vector3Int.down;
            if (world.GetBlock(blockBelow) != Blocks.AIR &&
                Math.Abs((transform.position.y) - playerBlockPos.y) < 0.01) {
                isGrounded = true;
                isFlying = false;
            } else {
                isGrounded = false;
            }

            // Handle jumping
            if (Input.GetKey(KeyCode.Space)) {
                if ((gamemode == Gamemode.SURVIVAL || gamemode == Gamemode.CREATIVE) && isGrounded) {
                    isGrounded = false;
                    rigidBody.AddForce(Vector3.up * (jumpForce * 100));
                } else if ((gamemode == Gamemode.CREATIVE && isFlying) || gamemode == Gamemode.SPECTATOR) {
                    var pos = rigidBody.position;
                    pos.y += 0.1f;
                    rigidBody.Move(pos, Quaternion.identity);
                }
            } else if (Input.GetKey(KeyCode.LeftShift)) {
                if (gamemode == Gamemode.SPECTATOR || (gamemode == Gamemode.CREATIVE && isFlying)) {
                    var pos = rigidBody.position;
                    pos.y -= 0.1f;
                    rigidBody.Move(pos, Quaternion.identity);
                }
            }
        }

        private void LateUpdate() {
            if (IsMenuOpen()) {
                ResetVelocity();
                return;
            }

            // Rotate player horizontal
            playerBody.Rotate(Vector3.up * (_mouseHorizontal * 5.0f));

            // Rotate camera vertical
            var rotY = Mathf.Clamp(_mouseVertical * 5.0f, -90f, 90f);
            playerCamera.transform.localRotation = Quaternion.Euler(-rotY, 0f, 0f);

            // Move player
            var speed = isSprinting ? sprintSpeed : walkSpeed;
            if (isFlying) speed *= 1.5f;
            var vel = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed;
            vel.y = rigidBody.velocity.y;
            // If the player stops moving, stop their sprinting
            if (vel.z <= walkSpeed) {
                isSprinting = false;
            }

            vel = playerBody.TransformDirection(vel);
            rigidBody.velocity = vel;
        }

        private bool IsMenuOpen() {
            return menuScreen.activeSelf;
        }

        private void ResetVelocity() {
            rigidBody.velocity = new Vector3(0, 0, 0);
        }

        private static void LockCursor(bool locked) {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        private void GetPlayerInput() {
            _mouseHorizontal = Input.GetAxis("Mouse X");
            _mouseVertical += Input.GetAxis("Mouse Y");

            if (Input.GetKeyDown(KeyCode.LeftControl)) {
                isSprinting = !isSprinting;
            }

            // Place/Break blocks
            if (highlightBlock.gameObject.activeSelf) {
                if (Input.GetMouseButtonDown(0)) {
                    world.SetBlock(highlightBlock.position.ToVector3Int(), Blocks.AIR);
                } else if (Input.GetMouseButtonDown(1) && placeBlock.gameObject.activeSelf) {
                    world.SetBlock(placeBlock.position.ToVector3Int(), Blocks.DIRT);
                }
            }
        }

        private void PlaceCursorBlock() {
            var step = checkIncrement;
            var lastPos = new Vector3();

            while (step < reach) {
                var transform1 = playerCamera.transform;
                var pos = transform1.position + (transform1.forward * step);

                if (world.CheckForBlock(pos.ToVector3Int())) {
                    highlightBlock.position = pos.ToVector3Int();
                    placeBlock.position = lastPos;

                    highlightBlock.gameObject.SetActive(true);
                    if (transform.position.ToVector3Int() != placeBlock.position.ToVector3Int()) {
                        placeBlock.gameObject.SetActive(true);
                    } else {
                        placeBlock.gameObject.SetActive(false);
                    }

                    return;
                }

                lastPos = pos.ToVector3Int();
                step += checkIncrement;
            }

            highlightBlock.gameObject.SetActive(false);
            placeBlock.gameObject.SetActive(false);
        }

    }

}
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using World.Block;

namespace World.Entity {

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Player : Entity {

        // OBJECTS
        [Header("Player Game Objects")]
        [SerializeField] private GameObject menuScreen;
        [SerializeField] private Transform highlightBlock;
        [SerializeField] private Transform placeBlock;
        [SerializeField] private Transform playerBody;
        private new Camera camera;
        private Rigidbody rigidBody;

        // VALUES
        [Header("Player Values")]
        [SerializeField] private float checkIncrement = 0.1f;
        [SerializeField] private float reach = 8f;

        private bool isSpringing;
        private float _mouseHorizontal;
        private float _mouseVertical;

        private void Start() {
            var cameraObject = gameObject.GetComponentInChildren<Camera>();
            if (cameraObject != null) {
                this.camera = cameraObject;
            } else {
                Debug.LogError("Camera on player == null");
            }

            this.rigidBody = gameObject.GetComponent<Rigidbody>();
            this.rigidBody.useGravity = false;
            LockCursor(true);
        }

        private void Update() {
            if (IsMenuOpen()) return;
            GetPlayerInput();
            PlaceCursorBlock();
        }

        private void FixedUpdate() {
            if (IsMenuOpen()) return;
            var pull = Vector3.down * (gravity * rigidBody.mass);
            rigidBody.AddForce(pull, ForceMode.Acceleration);
        }

        private void LateUpdate() {
            if (IsMenuOpen()) {
                rigidBody.velocity = new Vector3(0, 0, 0);
                return;
            }

            // Rotate player horizontal
            playerBody.Rotate(Vector3.up * (_mouseHorizontal * 5.0f));

            // Rotate camera vertical
            var rotY = Mathf.Clamp(_mouseVertical * 5.0f, -90f, 90f);
            camera.transform.localRotation = Quaternion.Euler(-rotY, 0f, 0f);

            // Move player
            var speed = isSpringing ? sprintSpeed : walkSpeed;
            var vel = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed;
            vel.y = rigidBody.velocity.y;
            // If the player stops, stop their sprinting
            if (vel.z <= walkSpeed) {
                isSpringing = false;
            }

            vel = playerBody.TransformDirection(vel);
            rigidBody.velocity = vel;

            if (isGrounded && Input.GetKeyDown(KeyCode.Space)) {
                rigidBody.AddForce(Vector3.up * (jumpForce * 100));
                //isGrounded = false;
            }
        }

        private bool IsMenuOpen() {
            return menuScreen.activeSelf;
        }

        private static void LockCursor(bool locked) {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        private void GetPlayerInput() {
            _mouseHorizontal = Input.GetAxis("Mouse X");
            _mouseVertical += Input.GetAxis("Mouse Y");

            if (Input.GetButtonDown("Sprint") && isGrounded) {
                isSpringing = !isSpringing;
            }

            // Place/Break blocks
            if (highlightBlock.gameObject.activeSelf) {
                if (Input.GetMouseButtonDown(0)) {
                    world.SetBlock(highlightBlock.position.ToVector3Int(), Blocks.AIR);
                } else if (Input.GetMouseButtonDown(1)) {
                    world.SetBlock(placeBlock.position.ToVector3Int(), Blocks.DIRT);
                }
            }
        }

        private void PlaceCursorBlock() {
            var step = checkIncrement;
            var lastPos = new Vector3();

            while (step < reach) {
                var transform1 = camera.transform;
                var pos = transform1.position + (transform1.forward * step);

                if (world.CheckForBlock(pos.ToVector3Int())) {
                    highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y),
                        Mathf.FloorToInt(pos.z));
                    placeBlock.position = lastPos;

                    highlightBlock.gameObject.SetActive(true);
                    placeBlock.gameObject.SetActive(true);

                    return;
                }

                lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                step += checkIncrement;
            }

            highlightBlock.gameObject.SetActive(false);
            placeBlock.gameObject.SetActive(false);
        }

    }

}
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using World.Block;

namespace World.Entity {

    public class Player : Entity {

        // SCREENS
        [SerializeField] private GameObject menuScreen;

        // OBJECTS
        private Transform _camera;
        [SerializeField] private Transform highlightBlock;
        [SerializeField] private Transform placeBlock;
        
        // VALUES
        [SerializeField] private float checkIncrement = 0.1f;
        [SerializeField] private float reach = 8f;
        
        private bool _isSprinting;
        private float _horizontal;
        private float _vertical;
        private float _mouseHorizontal;
        private float _mouseVertical;
        private Vector3 _velocity;
        private float _verticalMomentum;
        private bool _jumpRequest;

        private void Start() {
            var component = gameObject.GetComponentInChildren<Camera>();
            if (component != null) {
                _camera = component.transform;
            } else {
                Debug.LogError("Camera on player == null");
            }
            LockCursor(true);
        }

        private void FixedUpdate() {
            if (IsMenuOpen()) return;

            CalculateVelocity();
            if (_jumpRequest) {
                Jump();
            }

            transform.Translate(_velocity, Space.World);
        }

        private void Update() {
            if (IsMenuOpen()) return;
            GetPlayerInput();
            PlaceCursorBlock();
        }
        
        private void LateUpdate() {
            if (IsMenuOpen()) return;
            // Rotate player horizontal
            transform.Rotate(Vector3.up * (_mouseHorizontal * 5.0f));
            
            // Rotate camera vertical
            var rotY = Mathf.Clamp(_mouseVertical * 5.0f, -90f, 90f); 
            _camera.localRotation = Quaternion.Euler(-rotY, 0f, 0f);
        }

        private bool IsMenuOpen() {
            return menuScreen.activeSelf;
        }

        private static void LockCursor(bool locked) {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        private void Jump() {
            _verticalMomentum = jumpForce;
            isGrounded = false;
            _jumpRequest = false;
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void CalculateVelocity() {
            // Affect vertical momentum with gravity
            if (_verticalMomentum > gravity) {
                _verticalMomentum += Time.fixedDeltaTime * gravity;
            }

            // if sprinting
            if (_isSprinting) {
                _velocity = ((transform.forward * _vertical) + (transform.right * _horizontal)) *
                            (Time.fixedDeltaTime * sprintSpeed);
            } else {
                _velocity = ((transform.forward * _vertical) + (transform.right * _horizontal)) *
                            (Time.fixedDeltaTime * walkSpeed);
            }

            // apply vertical momentum
            _velocity += Vector3.up * (_verticalMomentum * Time.fixedDeltaTime);

            // check if can move forward (if blocked by a block stop player)
            if (_velocity.z > 0 && !CanMoveNorth()) _velocity.z = 0;
            if (_velocity.z < 0 && !CanMoveSouth()) _velocity.z = 0;
            if (_velocity.x > 0 && !CanMoveEast()) _velocity.x = 0;
            if (_velocity.x < 0 && !CanMoveWest()) _velocity.x = 0;

            if (_velocity.y < 0) {
                _velocity.y = CheckDownSpeed(_velocity.y);
            } else if (_velocity.y > 0) {
                _velocity.y = CheckUpSpeed(_velocity.y);
            }
        }

        private void GetPlayerInput() {
            _horizontal = Input.GetAxis("Horizontal");
            _vertical = Input.GetAxis("Vertical");
            _mouseHorizontal = Input.GetAxis("Mouse X");
            _mouseVertical += Input.GetAxis("Mouse Y");

            if (Input.GetButtonDown("Sprint") && isGrounded) {
                _isSprinting = true;
            } else if (Input.GetButtonUp("Sprint")) {
                _isSprinting = false;
            }

            if (isGrounded && Input.GetButtonDown("Jump")) {
                _jumpRequest = true;
            }

            if (highlightBlock.gameObject.activeSelf) {
                if (Input.GetMouseButtonDown(0)) {
                    world.EditBlock(highlightBlock.position, Blocks.AIR);
                } else if (Input.GetMouseButtonDown(1)) {
                    world.EditBlock(placeBlock.position, Blocks.DIRT);
                }
            }
        }

        private void PlaceCursorBlock() {
            var step = checkIncrement;
            var lastPos = new Vector3();

            while (step < reach) {
                var pos = _camera.position + (_camera.forward * step);

                if (world.CheckForBlock(pos)) {
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
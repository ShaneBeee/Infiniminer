using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using World.Block;

namespace World.Entity {

    public class Player : MonoBehaviour {

        public bool isGrounded;
        public bool isSprinting;

        private Transform _camera;

        // SCREENS
        [SerializeField] private GameObject menuScreen;

        // OBJECTS
        [SerializeField] private World world;
        [SerializeField] private Transform highlightBlock;
        [SerializeField] private Transform placeBlock;

        // VALUES
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float sprintSpeed = 6f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float gravity = -9.81f;

        [SerializeField] private float playerWidth = 0.3f;
        //[SerializeField] private float bounceTolerance = 0.1f;

        [SerializeField] private float checkIncrement = 0.1f;
        [SerializeField] private float reach = 8f;

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

        private void LateUpdate() {
            if (IsMenuOpen()) return;
            transform.Rotate(Vector3.up * (_mouseHorizontal * 5.0f));
            _camera.Rotate(Vector3.right * (-_mouseVertical * 5.0f));
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
            if (isSprinting) {
                _velocity = ((transform.forward * _vertical) + (transform.right * _horizontal)) *
                            (Time.fixedDeltaTime * sprintSpeed);
            } else {
                _velocity = ((transform.forward * _vertical) + (transform.right * _horizontal)) *
                            (Time.fixedDeltaTime * walkSpeed);
            }

            // apply vertical momentum
            _velocity += Vector3.up * (_verticalMomentum * Time.fixedDeltaTime);

            if ((_velocity.z > 0 && Front) || (_velocity.z < 0 && Back)) {
                _velocity.z = 0;
            }

            if ((_velocity.x > 0 && Right) || (_velocity.x < 0 && Left)) {
                _velocity.x = 0;

            }

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
            _mouseVertical = Input.GetAxis("Mouse Y");

            if (Input.GetButtonDown("Sprint") && isGrounded) {
                isSprinting = true;
            } else if (Input.GetButtonUp("Sprint")) {
                isSprinting = false;
            }

            if (isGrounded && Input.GetButtonDown("Jump")) {
                _jumpRequest = true;
            }

            if (highlightBlock.gameObject.activeSelf) {
                if (Input.GetMouseButtonDown(0)) {
                    var position = highlightBlock.position;
                    var chunkX = Mathf.FloorToInt(position.x / 16);
                    var chunkZ = Mathf.FloorToInt(position.z / 16);
                    var chunk = world.GetChunk(chunkX, chunkZ);

                    var blockX = Mathf.FloorToInt(position.x % 16);
                    var blockY = Mathf.FloorToInt(position.y);
                    var blockZ = Mathf.FloorToInt(position.z % 16);
                    chunk.EditBlock(new Vector3(blockX, blockY, blockZ), Blocks.AIR);
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

        private float CheckDownSpeed(float downSpeed) {
            var transformPosition = transform.position;
            var x = transformPosition.x;
            var y = transformPosition.y;
            var z = transformPosition.z;
            var width = playerWidth;
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

        private float CheckUpSpeed(float upSpeed) {
            var transformPosition = transform.position;
            var x = transformPosition.x;
            var y = transformPosition.y;
            var z = transformPosition.z;
            var width = playerWidth;
            if (world.CheckForBlock(x - width, y + 2f + upSpeed, z - width) ||
                world.CheckForBlock(x + width, y + 2f + upSpeed, z - width) ||
                world.CheckForBlock(x + width, y + 2f + upSpeed, z + width) ||
                world.CheckForBlock(x - width, y + 2f + upSpeed, z + width)) {
                return 0;
            }

            return upSpeed;
        }

        private bool Front {
            get {
                var transformPosition = transform.position;
                var x = transformPosition.x;
                var y = transformPosition.y;
                var z = transformPosition.z;
                var width = playerWidth;

                return world.CheckForBlock(x, y, z + width) || world.CheckForBlock(x, y + 1f, z + width);
            }
        }

        private bool Back {
            get {
                var transformPosition = transform.position;
                var x = transformPosition.x;
                var y = transformPosition.y;
                var z = transformPosition.z;
                var width = playerWidth;

                return world.CheckForBlock(x, y, z - width) || world.CheckForBlock(x, y + 1f, z - width);
            }
        }

        private bool Left {
            get {
                var transformPosition = transform.position;
                var x = transformPosition.x;
                var y = transformPosition.y;
                var z = transformPosition.z;
                var width = playerWidth;

                return world.CheckForBlock(x - width, y, z) ||
                       world.CheckForBlock(x - width, y + 1f, z);
            }
        }

        private bool Right {
            get {
                var transformPosition = transform.position;
                var x = transformPosition.x;
                var y = transformPosition.y;
                var z = transformPosition.z;
                var width = playerWidth;

                return world.CheckForBlock(x + width, y, z) ||
                       world.CheckForBlock(x + width, y + 1f, z);
            }
        }

    }

}
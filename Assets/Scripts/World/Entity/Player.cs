using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace World.Entity {

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    public class Player : Entity {

        // OBJECTS
        [Header("Player Game Objects")] [SerializeField]
        private GameObject menuScreen;

        [SerializeField] internal Transform highlightBlock;
        [SerializeField] internal Transform placeBlock;
        [SerializeField] internal Transform playerBody;

        // VALUES
        [Header("Player Values")] [SerializeField]
        internal float checkIncrement = 0.1f;

        [SerializeField] internal float reach = 8f;
        internal Gamemode gamemode = Gamemode.CREATIVE;

        internal bool isSprinting;
        internal bool isFlying;

        public void SetGamemode(Gamemode gamemode) {
            this.gamemode = gamemode;
        }

        internal bool IsMenuOpen() {
            return menuScreen.activeSelf;
        }

        public void LockCursor(bool locked) {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

    }

}
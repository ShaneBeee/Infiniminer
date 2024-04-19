using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using World.Entity;

namespace Screens {
    public class MenuScreen : MonoBehaviour {

        [SerializeField] private Player player;
        [SerializeField] private Dropdown gamemode;

        private void Start() {
            gamemode.value = (int) player.gamemode;
        }

        public void CloseMenu() {
            gameObject.SetActive(false);
            player.SetGamemode((Gamemode)gamemode.value);
            player.LockCursor(true);
        }

        public void CloseGame() {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

    }
}
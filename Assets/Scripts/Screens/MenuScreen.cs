using UnityEditor;
using UnityEngine;

namespace Screens {
    public class MenuScreen : MonoBehaviour {

        public void CloseMenu() {
            var menuActive = gameObject.activeSelf;
            gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
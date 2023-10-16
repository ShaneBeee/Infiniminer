using UnityEngine;
using UnityEngine.UI;

namespace Screens {
    public class ScreenManager : MonoBehaviour {

        [SerializeField] private GameObject loadingScreenCanvas;
        [SerializeField] private GameObject debugScreenCanvas;
        [SerializeField] private GameObject menuScreenCanvas;
        [SerializeField] private Slider slider;

        private void Start() {
            loadingScreenCanvas.SetActive(true);
            debugScreenCanvas.SetActive(false);
            menuScreenCanvas.SetActive(false);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.F3)) {
                debugScreenCanvas.SetActive(!debugScreenCanvas.activeSelf);
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                var menuActive = menuScreenCanvas.activeSelf;
                menuScreenCanvas.SetActive(!menuActive);
                Cursor.lockState = !menuActive ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = !menuActive;
            }
        }

        public void UpdateProgressBar(float value) {
            slider.value = value;
        }

        public void LoadingScreenEnabled(bool visible) {
            loadingScreenCanvas.SetActive(visible);
        }

    }

}
using UnityEngine;
using UnityEngine.UI;
using World.Entity;

namespace Screens {
    public class DebugScreen : MonoBehaviour {

        [SerializeField] private Player player;

        private Text debugText;
        private float frameRate;
        private float timer;

        private void Start() {
            debugText = GetComponentInChildren<Text>();
            if (debugText == null) {
                Debug.LogError("DebugText == null");
            }
        }

        private void Update() {
            // Only prepare if active
            if (!gameObject.activeSelf) return;

            if (timer > 1f) {
                frameRate = (int)(1f / Time.unscaledDeltaTime);
                timer = 0;
            } else {
                timer += Time.deltaTime;
            }

            if (!gameObject.activeSelf) return;
            var position = player.transform.position;
            var x = position.x.ToString("0.00");
            var y = position.y.ToString("0.00");
            var z = position.z.ToString("0.00");
            debugText.text = "DebugScreen \n\n" +
                             "FPS: " + frameRate + "\n" +
                             "XYZ: " + x + ", " + y + ", " + z + "\n";
        }

    }

}
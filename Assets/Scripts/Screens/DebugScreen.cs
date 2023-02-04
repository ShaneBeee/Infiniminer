using UnityEngine;
using UnityEngine.UI;
using World.Entity;

public class DebugScreen : MonoBehaviour {
    
    [SerializeField] private Player _player;
    
    private Text debugText;
    private float frameRate;
    private float timer;
    
    // Start is called before the first frame update
    void Start() {
        debugText = GetComponentInChildren<Text>();
        if (debugText == null) {
            Debug.LogError("DebugText == null");
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.F3)) {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        if (timer > 1f) {
            frameRate = (int) (1f / Time.unscaledDeltaTime);
            timer = 0;
        } else {
            timer += Time.deltaTime;
        }

        if (gameObject.activeSelf) {
            var position = _player.transform.position;
            var x = position.x.ToString("0.00");
            var y = position.y.ToString("0.00");
            var z = position.z.ToString("0.00");
            debugText.text = "DebugScreen \n\n" +
                             "FPS: " + frameRate + "\n" +
                             "XYZ: " + x + ", " + y + ", " + z + "\n";
        }
    }
    
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class TapEffect : MonoBehaviour {
    private bool isFinishedSplashScreen = false;

    [SerializeField] ParticleSystem tapEffect;
//      [SerializeField] Camera _camera;
    private Camera _camera;


    void Awake() {
      _camera = GetComponent<Camera>();
//         DontDestroyOnLoad(tapEffect);
    }

    void Update() {
      if (Input.GetMouseButtonDown(0)) {
        if (isFinishedSplashScreen) {
          var pos = _camera.ScreenToWorldPoint(Input.mousePosition + _camera.transform.forward * 10);
          tapEffect.transform.position = pos;
          tapEffect.Emit(6);
        }
      }

      if (UnityEngine.Rendering.SplashScreen.isFinished) {
        isFinishedSplashScreen = true;
      }
   }
}

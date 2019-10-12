using UnityEngine;
using System.Collections;

public class MobileUtilsScript : MonoBehaviour
{

    private int FramesPerSec;
    private float frequency = 1.0f;
    private string fps;
    private GUIStyle style;


    void Start()
    {
        style = new GUIStyle();
        style.fontSize = 32;
        style.stretchWidth = true;
        style.stretchHeight = true;
        StartCoroutine(FPS());
    }

    private IEnumerator FPS()
    {
        for (; ; )
        {
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            // Display it

            fps = string.Format("FPS: {0}", Mathf.RoundToInt(frameCount / timeSpan));
        }
    }


    void OnGUI()
    {
        
        GUI.TextArea(new Rect(Screen.width - 200, 10, 250, 40), fps, style);
    }
}
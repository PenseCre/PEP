using UnityEngine;
using UnityEngine.UI;

public class FeedbackSlider : MonoBehaviour
{
    private Slider slider;
    private Object onEndEdit;
    private string onEndEditMethodName;

    void Start()
    {
        try
        {
            slider = GetComponent<Slider>();
            onEndEdit = slider.onValueChanged.GetPersistentTarget(0);
            onEndEditMethodName = slider.onValueChanged.GetPersistentMethodName(0).Remove(0, 4);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.StackTrace);
            throw;
        }
    }

    public static object GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName).GetValue(src, null);
    }

    void Update()
    {
        try
        {
            float v;
            bool b = float.TryParse(GetPropValue(onEndEdit, onEndEditMethodName).ToString(), out v);
            if (b)
            {
                if (v != slider.value && v != float.NaN)
                {
                    slider.value = v;
                }
            }
            else
            {
                Debug.LogError("Could not parse value");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.StackTrace);
            throw;
        }
    }
}

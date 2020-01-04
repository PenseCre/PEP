using UnityEngine;
using UnityEngine.UI;

public class FeedbackSlider : MonoBehaviour
{
    private Slider slider;
    private Object onEndEdit;
    private string onEndEditMethodName;
    public bool adjustMinMax = false;
    [Range(0.001f, float.MaxValue)]
    public float minMaxRange = 1f;

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

    public void AdjustMinMax(float v)
    {
        slider.minValue = v - minMaxRange;
        slider.maxValue = v + minMaxRange;
    }

    public void AdjustMinMax()
    {
        AdjustMinMax(slider.value);
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
                    if (adjustMinMax/* && (v - minMaxRange) != slider.minValue*/)
                    {
                        slider.minValue = v - minMaxRange;
                        slider.maxValue = v + minMaxRange;
                        adjustMinMax = false;
                    }
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

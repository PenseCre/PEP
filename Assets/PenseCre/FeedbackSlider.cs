using UnityEngine;
using UnityEngine.UI;

public class FeedbackSlider : MonoBehaviour
{
    Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
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
            bool b = float.TryParse(GetPropValue(slider.onValueChanged.GetPersistentTarget(0),
                slider.onValueChanged.GetPersistentMethodName(0).Remove(0, 4)).ToString(), out v);
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

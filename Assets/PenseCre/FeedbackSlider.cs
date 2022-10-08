using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FeedbackSlider : MonoBehaviour
{
    private Slider slider;
    private Object onEndEdit;
    private string onEndEditMethodName;
    public UnityEvent UpdateCallback;
    public FeedbackSlider mutuallyExclusiveLink;
    public bool adjustMinMax = false;
    private bool adjustMinMax_persistent = false;
    private bool selected = false;
    [Range(0.001f, float.MaxValue)]
    public float minMaxRange = 1f;
    public int onValueChangedMethodID = 0;

    public bool Selected { get => selected; set => selected = value; }

    private void Awake()
    {
        adjustMinMax_persistent = adjustMinMax;
    }
    private void OnEnable()
    {
        adjustMinMax = adjustMinMax_persistent;
    }
    void Start()
    {
        try
        {
            slider = GetComponent<Slider>();
            onEndEdit = slider.onValueChanged.GetPersistentTarget(onValueChangedMethodID);
            onEndEditMethodName = slider.onValueChanged.GetPersistentMethodName(onValueChangedMethodID).Remove(0, 4);
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

    public void SendValueToMutuallyExclusivelyLinkedSlider()
    {
        if (mutuallyExclusiveLink == null) return;
        if (mutuallyExclusiveLink.selected) return;
        mutuallyExclusiveLink.slider.minValue = slider.minValue;
        mutuallyExclusiveLink.slider.maxValue = slider.maxValue;
        mutuallyExclusiveLink.slider.value = slider.value;
        if (mutuallyExclusiveLink.adjustMinMax)
        {
            mutuallyExclusiveLink.AdjustMinMax();
        }
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
        if (selected) return;
        try
        {
            float v;
            bool b = float.TryParse(GetPropValue(onEndEdit, onEndEditMethodName).ToString(), out v);
            if (b)
            {
                if (v != slider.value && v != float.NaN)
                {
                    if (adjustMinMax_persistent && adjustMinMax/* && (v - minMaxRange) != slider.minValue*/)
                    {
                        slider.minValue = v - minMaxRange;
                        slider.maxValue = v + minMaxRange;
                        adjustMinMax = false;
                    }
                    slider.value = v;
                    UpdateCallback?.Invoke();
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

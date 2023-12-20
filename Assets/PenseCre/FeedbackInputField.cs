using UnityEngine;
using UnityEngine.UI;

public class FeedbackInputField : MonoBehaviour
{
    private InputField inputField;
    private Object onEndEdit;
    private string onEndEditMethodName;

    void Start()
    {
        try
        {
            inputField = GetComponent<InputField>();
            if (inputField.onEndEdit.GetPersistentEventCount() > 0)
            {
                onEndEdit = inputField.onEndEdit.GetPersistentTarget(0);
            }
            if (onEndEdit == null)
            {
                inputField = null;
                Debug.LogWarning("No OnEndEdit event assigned");
                return;
            }
            onEndEditMethodName = inputField.onEndEdit.GetPersistentMethodName(0).Remove(0, 4).ToString();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.StackTrace);
            throw;
        }
    }

    public static object GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName).GetValue(src, null);
    }

    void Update()
    {
        if (!inputField || inputField.isFocused) return;
        if (onEndEdit == null) return;
        try
        {
            string v;
            v = GetPropValue(onEndEdit, onEndEditMethodName).ToString();
            if (v != inputField.text && v != "" && v != null)
                inputField.text = v;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.StackTrace);
            throw;
        }
    }
}

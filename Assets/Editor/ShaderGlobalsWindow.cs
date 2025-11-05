using UnityEditor;
using UnityEngine;

public class ShaderGlobalsWindow : EditorWindow
{
    float progressValue = 0.4f; // valor inicial

    [MenuItem("Tools/Shader Progress Controller")]
    public static void ShowWindow()
    {
        GetWindow<ShaderGlobalsWindow>("Progress Control");
    }

    void OnGUI()
    {
        GUILayout.Label("Control del Float Global: _Progress", EditorStyles.boldLabel);

        progressValue = EditorGUILayout.Slider("Progress", progressValue, 0f, 1f);
        Shader.SetGlobalFloat("_Progress", progressValue);
    }
}

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public static class EditorUtils
{
    // Splitter code from : http://answers.unity3d.com/questions/216584/horizontal-line.html
    private static readonly GUIStyle splitter;
    private static readonly Color splitterColor = EditorGUIUtility.isProSkin ? new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);
    public static readonly string EDITOR_CREATED_TAG = "__EDITOR_CREATED";

    public static string MainSettingsStyle = "ProgressBarBack";
    public static string GroupPaneStyle = "GroupBox";
    public static string InternalPaneStyle = "AppToolbar";
    public static string ElementStyle = "HelpBox";
    public static string TitleBarStyle = "TL LogicBar 1";
    public static string OptionMenuStyle = "PaneOptions";
    public static string GreyStyle = "flow node 0";
    public static string BlueStyle = "flow node 1";
    public static string TealStyle = "flow node 2";
    public static string GreenStyle = "flow node 3";
    public static string YellowStyle = "flow node 4";
    public static string OrangeStyle = "flow node 5";
    public static string RedStyle = "flow node 6";
    public static GUIStyle MainSettingsFont = new GUIStyle();
    public static GUIStyle TextWrapStyle = new GUIStyle();
    public static GUIStyle LeftAlignStyle = new GUIStyle();

    static EditorUtils()
    {
        splitter = new GUIStyle();
        splitter.normal.background = EditorGUIUtility.whiteTexture;
        splitter.stretchWidth = true;
        splitter.margin = new RectOffset(0, 0, 7, 7);

        TextWrapStyle = new GUIStyle();
        TextWrapStyle.wordWrap = true;
        TextWrapStyle.normal.textColor = Color.white;

        if (SystemInfo.graphicsDeviceID == 0)
        {
            LeftAlignStyle = new GUIStyle(InternalPaneStyle);
        }
        else
        {
            LeftAlignStyle = new GUIStyle();
        }
        LeftAlignStyle.alignment = TextAnchor.UpperLeft;
        LeftAlignStyle.stretchWidth = false;


        MainSettingsFont.fontSize = 14;
        MainSettingsFont.fontStyle = FontStyle.Bold;
    }

    private static int getScreenWidth()
    {
#if UNITY_EDITOR_OSX
			return 400;
#else
        return Screen.width;
#endif
    }

    public static void CreateMinMaxSliderBar(string label,
        ref int lowValue,
        ref int highValue,
        int minValue,
        int maxValue)
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();


        // Set sane bounds
        lowValue = Mathf.Max(Mathf.Min(lowValue, maxValue), minValue);
        highValue = Mathf.Max(Mathf.Min(highValue, maxValue), minValue);

        Rect spacingRect = GUILayoutUtility.GetRect(1, 30);

        int indentPixels = 25 * EditorGUI.indentLevel;

        int frameWidth = getScreenWidth() - indentPixels - 100;

        Rect frame = new Rect(indentPixels, spacingRect.y, frameWidth, 20);

        // Mimic a progress bar
        GUI.Box(frame, "", "ProgressBarBack");

        float delta = maxValue - minValue;
        Rect filled = new Rect(frame.x + frame.width * (lowValue - minValue) / delta,
            frame.y,
            8 + (frame.width - 8) * (highValue - lowValue) / delta,
            frame.height);
        GUI.Box(filled, GUIContent.none, "ProgressBarBar");

        GUIStyle labelStyle = new GUIStyle();
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.normal.textColor = Color.black;
        labelStyle.fontSize = 10;
        GUI.Label(frame, label + " [" + lowValue + " - " + highValue + "]", labelStyle);


        Rect sliderRect = new Rect(EditorGUI.indentLevel * 10,
            frame.y + 12,
            getScreenWidth() - (EditorGUI.indentLevel * 10) - 100,
            frame.height);
        float lowFloat = lowValue;
        float highFloat = highValue;
        EditorGUI.MinMaxSlider(sliderRect, ref lowFloat, ref highFloat, minValue, maxValue);

        lowValue = (int)lowFloat;
        highValue = (int)highFloat;
    }

    public static void CreateMinMaxSliderBarF(string label,
        ref float lowValue,
        ref float highValue,
        float minValue,
        float maxValue)
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();


        // Set sane bounds
        lowValue = Mathf.Max(Mathf.Min(lowValue, maxValue - 1), minValue);
        highValue = Mathf.Max(Mathf.Min(highValue, maxValue), minValue + 1);

        Rect spacingRect = GUILayoutUtility.GetRect(1, 30);

        int indentPixels = 25 * EditorGUI.indentLevel;

        int frameWidth = getScreenWidth() - indentPixels - 100;

        Rect frame = new Rect(indentPixels, spacingRect.y, frameWidth, 20);

        // Mimic a progress bar
        GUI.Box(frame, "", "ProgressBarBack");

        float delta = maxValue - minValue;
        Rect filled = new Rect(frame.x + frame.width * (lowValue - minValue) / delta,
            frame.y,
            8 + (frame.width - 8) * (highValue - lowValue) / delta,
            frame.height);
        GUI.Box(filled, GUIContent.none, "ProgressBarBar");

        GUIStyle labelStyle = new GUIStyle();
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.normal.textColor = Color.black;
        labelStyle.fontSize = 10;
        GUI.Label(frame, label + " [" + lowValue + " - " + highValue + "]", labelStyle);


        Rect sliderRect = new Rect(EditorGUI.indentLevel * 10,
            frame.y + 12,
            getScreenWidth() - (EditorGUI.indentLevel * 10) - 100,
            frame.height);
        EditorGUI.MinMaxSlider(sliderRect, ref lowValue, ref highValue, minValue, maxValue);
    }

    // GUILayout Style
    public static void Splitter(Color rgb, float thickness = 1)
    {
        Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitter, GUILayout.Height(thickness));

        if (Event.current.type == EventType.Repaint)
        {
            Color restoreColor = GUI.color;
            GUI.color = rgb;
            splitter.Draw(position, false, false, false, false);
            GUI.color = restoreColor;
        }
    }

    public static void Splitter(float thickness, GUIStyle splitterStyle)
    {
        Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitterStyle, GUILayout.Height(thickness));

        if (Event.current.type == EventType.Repaint)
        {
            Color restoreColor = GUI.color;
            GUI.color = new Color(.2f, .2f, .2f, 1f);
            splitterStyle.Draw(position, false, false, false, false);
            GUI.color = restoreColor;
        }
    }

    public static void Splitter(float thickness = 1)
    {
        Splitter(thickness, splitter);
    }

    // GUI Style
    public static void Splitter(Rect position)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Color restoreColor = GUI.color;
            GUI.color = splitterColor;
            splitter.Draw(position, false, false, false, false);
            GUI.color = restoreColor;
        }
    }

    public static void ClearPrefabs()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag(EDITOR_CREATED_TAG))
        {
            Editor.DestroyImmediate(obj);
        }
    }

    public static string GetAssetFilename(UnityEngine.Object asset)
    {
        string assetPath = AssetDatabase.GetAssetPath(asset);
        int lastSlash = assetPath.LastIndexOf("/");
        if (lastSlash < 0)
        {
            return "";
        }
        string fileName = assetPath.Substring(lastSlash + 1, assetPath.LastIndexOf(".") - lastSlash - 1);
        return fileName;
    }

    public static string GetAssetDirectory(UnityEngine.Object asset)
    {
        string assetPath = AssetDatabase.GetAssetPath(asset);
        int lastSlash = assetPath.LastIndexOf("/");
        if (lastSlash < 0)
        {
            return "";
        }
        string folderName = assetPath.Substring(0, lastSlash);
        return folderName;
    }

    public static bool DoesContainPath(string parentPath, string childPath)
    {
        return childPath.LastIndexOf(parentPath) > -1;
    }

    public static void CreateParentDirectoriesForPath(string assetPath)
    {
        assetPath = System.IO.Path.GetDirectoryName(assetPath);
        string[] directories = assetPath.Split('/');

        string parentPath = "";

        for (int i = 0; i < directories.Length; ++i)
        {
            string currentPath = parentPath + directories[i];
            if (!AssetDatabase.IsValidFolder(currentPath))
            {
                string noTrailingSlash = parentPath.Substring(0, parentPath.Length - 1);
                AssetDatabase.CreateFolder(noTrailingSlash, directories[i]);
            }
            parentPath = currentPath + '/';
        }
    }

    public static void SetObjectDirty(UnityEngine.Object o)
    {
        EditorUtility.SetDirty(o);
    }

    public static void SetObjectDirty(GameObject go)
    {
        EditorUtility.SetDirty(go);
        EditorSceneManager.MarkSceneDirty(go.scene); //This used to happen automatically from SetDirty
    }

    public static void SetObjectDirty(Component comp)
    {
        EditorUtility.SetDirty(comp);
        EditorSceneManager.MarkSceneDirty(comp.gameObject.scene); //This used to happen automatically from SetDirty
    }

    public class EditorGlobalsCache
    {
        private float cachedLabelWidth;
        private Color cachedContentColor;
        private int cachedIndentLevel;

        public float labelWidth { get { return EditorGUIUtility.labelWidth; } set { EditorGUIUtility.labelWidth = value; } }
        public Color contentColor { get { return GUI.contentColor; } set { GUI.contentColor = value; } }
        public int indentLevel { get { return EditorGUI.indentLevel; } set { EditorGUI.indentLevel = value; } }

        // create a new cache and it'll automatically save off relevant globals.
        public EditorGlobalsCache()
        {
            cachedLabelWidth = EditorGUIUtility.labelWidth;
            cachedContentColor = GUI.contentColor;
            cachedIndentLevel = EditorGUI.indentLevel;
        }

        // call restore to bring them back to their former glory
        public void Restore()
        {
            EditorGUIUtility.labelWidth = cachedLabelWidth;
            GUI.contentColor = cachedContentColor;
            EditorGUI.indentLevel = cachedIndentLevel;
        }
    }
}

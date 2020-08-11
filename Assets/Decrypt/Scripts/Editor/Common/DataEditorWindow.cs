using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

public abstract class DataEditorWindow : ScrollableEditorWindow
{
    protected SerializedObject serializedObject;
    protected ScriptableObject data;

    protected abstract string FileName { get; }
    protected abstract Type DataType { get; }
    protected virtual Type[] DataTypes { get { return new Type[1] { DataType }; } }
    protected abstract string AssetTypeName { get; }
    protected virtual string DefaultPath { get { return "Assets/"; } }
    protected virtual bool ForceDefaultPath { get { return false; } }

    public static void Open<TWindow>(string editorTabName)
        where TWindow : EditorWindow
    {
        // Open the window
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        DataEditorWindow window = GetWindow<TWindow>(false, editorTabName, true) as DataEditorWindow;
        watch.Stop();
        if (watch.ElapsedMilliseconds > 500)
        {
            Debug.LogFormat("EditorWindow.GetWindow for window type {0} took {1} seconds", typeof(TWindow), watch.ElapsedMilliseconds / 1000f);
        }
        window.Show();
        FocusWindowIfItsOpen<SceneView>();
        window.Initialize();
    }

    public static void OpenType(Type editorWindowType, string editorTabName)
    {
        // Open the window
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        DataEditorWindow window = GetWindow(editorWindowType, false, editorTabName, true) as DataEditorWindow;
        watch.Stop();
        if (watch.ElapsedMilliseconds > 500)
        {
            Debug.LogFormat("EditorWindow.GetWindow for window type {0} took {1} seconds", editorWindowType, watch.ElapsedMilliseconds / 1000f);
        }
        window.Show();
        FocusWindowIfItsOpen<SceneView>();
        window.Initialize();
    }

    public virtual void Initialize()
    {
    }

    protected virtual void updateData(ScriptableObject data)
    {
        this.data = data;
        if (data != null)
        {
            serializedObject = new SerializedObject(data);
        }
    }

    protected virtual void OnSelectionChange()
    {
        Initialize();
        Repaint();
    }

    void OnEnable()
    {
        Initialize();
        updateData(data);
    }

    void OnDisable()
    {
        Clear();
    }

    void OnDestroy()
    {
        Clear();
    }

    void OnFocus()
    {
        Initialize();
    }

    void Update()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Clear();
        }
    }

    protected virtual void Clear()
    {
        // implemented in children
    }

    public static void CreateAssetType(Type assetType,
        string fileName,
        Type editorWindowType,
        string editorTabName,
        string defaultPath = "Assets/",
        bool forceDefaultPath = false)
    {
        ScriptableObject asset = CreateInstance(assetType);

        string path = defaultPath;
        if (!forceDefaultPath)
        {
            foreach (UnityEngine.Object obj in UnityEditor.Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets))
            {
                path = UnityEditor.AssetDatabase.GetAssetPath(obj);
                if (System.IO.File.Exists(path))
                {
                    path = System.IO.Path.GetDirectoryName(path);
                }
                break;
            }
        }

        string assetPath = System.IO.Path.Combine(path, fileName + ".asset");
        EditorUtils.CreateParentDirectoriesForPath(assetPath);
        string fullPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

        // Make the asset and select it
        AssetDatabase.CreateAsset(asset, fullPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(fullPath);
        EditorUtility.FocusProjectWindow();

        if (editorWindowType != null)
        {
            // Open the window
            OpenType(editorWindowType, editorTabName);
        }
    }

    protected virtual void handleNoData()
    {
        GUILayout.Label("Select or create a " + AssetTypeName + " file");
        if (GUILayout.Button("Create new " + AssetTypeName))
        {
            CreateAssetType(DataType, FileName, GetType(), AssetTypeName, DefaultPath, ForceDefaultPath);
        }
    }

    public override void OnGUI()
    {
        if (data == null || serializedObject == null)
        {
            handleNoData();
            return;
        }

        serializedObject.Update();

        base.OnGUI();

        if (data == null || serializedObject == null)
        {
            return;
        }

        bool isDirty = GUI.changed;

        if (OptionMenu.Dirty)
        {
            isDirty = true;
            OptionMenu.Dirty = false;
        }

        if (isDirty)
        {
            Undo.RecordObject(data, AssetTypeName + " Editor Changed");
            EditorUtils.SetObjectDirty(data);
            onChanged();
        }
    }

    protected override void drawHeader()
    {
        base.drawHeader();

        drawHeaderFileInfo();
    }

    protected virtual void drawHeaderFileInfo()
    {
        if (data == null)
        {
            EditorUtils.EditorGlobalsCache cache = new EditorUtils.EditorGlobalsCache();
            GUI.contentColor = Color.red;
            EditorGUILayout.LabelField("No valid data file found in a parent directory.");
            cache.Restore();
        }
        else
        {
            EditorGUILayout.LabelField(string.Format("{0}: {1}", AssetTypeName, data.name));
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(AssetDatabase.GetAssetPath(data));
                if (GUILayout.Button("Select", GUILayout.MaxWidth(60f)))
                {
                    Selection.activeObject = data;
                    EditorGUIUtility.PingObject(data);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorUtils.Splitter();
    }

    protected virtual void onChanged()
    {
        serializedObject.ApplyModifiedProperties();
    }

    protected string getFileName()
    {
        string path = AssetDatabase.GetAssetPath(data);

        string[] split = path.Split('/');

        string name = split[split.Length - 1];

        name = name.Replace(".asset", "");

        return name;
    }
}

public class DataEditorElementDropdown
{
    private struct Element
    {
        public UnityEngine.Object obj;
        public string name;
    }

    private List<Element> dataElements = new List<Element>();
    private string[] nameArray = null;
    //private List<UnityEngine.Object> dataElements = new List<UnityEngine.Object>();
    //private List<string> elementNames = new List<string>();

    private int selectedIndex = 0;
    private Type dataType;
    private string label;

    private Func<UnityEngine.Object, bool> filterElementFn;
    private Func<UnityEngine.Object, string> getElementNameFn;
    private Action<UnityEngine.Object> onSelectionChangeFn;

    public int Count { get { return dataElements.Count; } }
    public Type DataType { get { return dataType; } }
    public int SelectedIndex
    {
        get { return selectedIndex; }
        set
        {
            if (selectedIndex != value)
            {
                selectedIndex = value % dataElements.Count;
                onSelectionChangeFn(dataElements[selectedIndex].obj);
            }
        }
    }

    public DataEditorElementDropdown(string label,
        Type dataType,
        Func<UnityEngine.Object, bool> filterElementFn,
        Func<UnityEngine.Object, string> getElementNameFn,
        Action<UnityEngine.Object> onSelectionChangeFn)
    {
        this.label = label;
        this.dataType = dataType;
        this.filterElementFn = filterElementFn;
        this.getElementNameFn = getElementNameFn;
        this.onSelectionChangeFn = onSelectionChangeFn;

        UpdateDropdownElements();
    }

    public DataEditorElementDropdown(string label, Type dataType)
        : this(label, dataType, DefaultElementFilter, DefaultElementName, DefaultOnSelectionChange) { }

    public DataEditorElementDropdown(string label, Type dataType, Func<UnityEngine.Object, bool> filterElementFn)
        : this(label, dataType, filterElementFn, DefaultElementName, DefaultOnSelectionChange) { }

    public DataEditorElementDropdown(string label, Type dataType, Func<UnityEngine.Object, bool> filterElementFn, Func<UnityEngine.Object, string> getElementNameFn)
        : this(label, dataType, filterElementFn, getElementNameFn, DefaultOnSelectionChange) { }

    public static bool DefaultElementFilter(UnityEngine.Object obj)
    {
        return true;
    }

    public static string DefaultElementName(UnityEngine.Object obj)
    {
        return obj.name;
    }

    public static void DefaultOnSelectionChange(UnityEngine.Object obj)
    {
        Selection.activeObject = obj;
    }

    public void SetSelection(UnityEngine.Object obj)
    {
        int index = -1;
        for (int i = 0; i < dataElements.Count; ++i)
        {
            if (dataElements[i].obj.Equals(obj))
            {
                index = i;
                break;
            }
        }
        if (index >= 0)
        {
            selectedIndex = index;
        }
    }

    public UnityEngine.Object GetSelection()
    {
        if (selectedIndex >= 0)
        {
            return dataElements[selectedIndex].obj;
        }
        else
        {
            return null;
        }
    }

    public void UpdateDropdownElements()
    {
        dataElements.Clear();

        string[] elementGuids = AssetDatabase.FindAssets("t: " + dataType, new string[] { "Assets/Wavedash" });

        foreach (string guid in elementGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, dataType);
            if (filterElementFn(obj))
            {
                string name = getElementNameFn(obj);
                dataElements.Add(new Element() { obj = obj, name = name });
            }
        }

        ListUtil.Sort(dataElements, (a, b) => a.name.CompareTo(b.name) < 0);
        nameArray = new string[dataElements.Count];
        for (int i = 0; i < dataElements.Count; ++i)
        {
            nameArray[i] = dataElements[i].name;
        }
    }

    public void Draw()
    {
        EditorGUILayout.BeginHorizontal();

        if (Count > 0)
        {
            EditorGUI.BeginChangeCheck();

            selectedIndex = EditorGUILayout.Popup(label, selectedIndex, nameArray);

            if (EditorGUI.EndChangeCheck())
            {
                // Code to execute if GUI.changed
                // was set to true inside the block of code above.
                onSelectionChangeFn(dataElements[selectedIndex].obj);
            }
        }
        else
        {
            if (GUILayout.Button("Update Dropdown"))
            {
                UpdateDropdownElements();
            }
        }

        EditorGUILayout.EndHorizontal();
    }
}

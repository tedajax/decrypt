using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Collections;

public class OptionMenu
{
    private static Func<object> bufferedCopyFunction;
    private static Type bufferedType;
    public static bool Dirty = false;

    public static void Create<T>(T[] elements, T element, Action<IEnumerable<T>> callback)
    {
        List<T> list = new List<T>(elements);
        Create(list, element, callback);
        Dirty = true;
    }

    public static void Create<T>(List<T> elements, T element, Action<IEnumerable<T>> callback = null)
    {
        if (elements == null || elements.Count == 0)
        {
            return;
        }

        if (callback == null)
        {
            // Create no-op
            callback = delegate (IEnumerable<T> list) { };
        }

        GenericMenu menu = new GenericMenu();

        T firstElement = elements[0];

        if ((firstElement != null && firstElement.Equals(element))
            || (firstElement == null && element == null)
            || elements.Count == 1)
        {
            menu.AddDisabledItem(new GUIContent("Move Up"));
            menu.AddDisabledItem(new GUIContent("Move To Top"));
        }
        else
        {
            menu.AddItem(new GUIContent("Move Up"), false, delegate () { move<T>(elements, element, -1, callback); });
            menu.AddItem(new GUIContent("Move To Top"), false, delegate () { move<T>(elements, element, -elements.Count, callback); });
        }

        if ((elements[elements.Count - 1] != null && elements[elements.Count - 1].Equals(element))
            || elements.Count == 1)
        {
            menu.AddDisabledItem(new GUIContent("Move Down"));
            menu.AddDisabledItem(new GUIContent("Move To Bottom"));
        }
        else
        {
            menu.AddItem(new GUIContent("Move Down"), false, delegate () { move<T>(elements, element, 1, callback); });
            menu.AddItem(new GUIContent("Move To Bottom"), false, delegate () { move<T>(elements, element, elements.Count, callback); });
        }

        menu.AddSeparator("");

        if (element != null && element is ISaveAsset)
        {
            if ((element as ISaveAsset).CanSave)
            {
                menu.AddItem(new GUIContent("Save"), false, delegate () { (element as ISaveAsset).SaveAsset(); });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Save"));
            }
        }

        if (element != null && element is ICloneable)
        {
            menu.AddItem(new GUIContent("Copy"), false, delegate () { copy<T>(elements, element, callback); });
        }
        else
        {
            menu.AddDisabledItem(new GUIContent("Copy"));
        }

        if (element != null
            && bufferedCopyFunction != null
            && bufferedType.IsInstanceOfType(element))
        {
            menu.AddItem(new GUIContent("Paste"), false, delegate () { paste<T>(elements, element, callback); });
        }
        else
        {
            menu.AddDisabledItem(new GUIContent("Paste"));
        }

        menu.AddSeparator("");

        if (element is ICloneable)
        {
            menu.AddItem(new GUIContent("Duplicate"), false, delegate () { duplicate<T>(elements, element, callback); });
        }
        else
        {
            menu.AddDisabledItem(new GUIContent("Duplicate"));
        }

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Remove"), false, delegate () { remove<T>(elements, element, callback); });
        if (element is ScriptableObject)
        {
            menu.AddItem(new GUIContent("Delete Asset"), false, delegate () { delete<T>(elements, element, callback); });
        }
        else
        {
            menu.AddDisabledItem(new GUIContent("Delete Asset"));
        }

        menu.ShowAsContext();

        EditorGUIUtility.ExitGUI();


        Dirty = true;

    }

    private static void move<T>(List<T> elements,
        T element,
        int indices,
        Action<IEnumerable<T>> callback)
    {
        int newIndex = Mathf.Clamp(elements.IndexOf(element) + indices, 0, elements.Count - 1);
        elements.Remove(element);
        elements.Insert(newIndex, element);

        callback(elements);

        Dirty = true;
    }

    private static void duplicate<T>(List<T> elements,
        T element,
        Action<IEnumerable<T>> callback)
    {
        elements.Insert(elements.IndexOf(element) + 1, (T)(element as ICloneable).Clone());
        callback(elements);

        Dirty = true;
    }

    private static void copy<T>(List<T> elements,
        T element,
        Action<IEnumerable<T>> callback)
    {
        bufferedCopyFunction = (element as ICloneable).Clone;
        bufferedType = typeof(T);
        callback(elements);

        Dirty = true;
    }

    private static void save<T>(List<T> elements,
    T element,
    Action<IEnumerable<T>> callback)
    {
        callback(elements);
    }

    private static void paste<T>(List<T> elements,
        T element,
        Action<IEnumerable<T>> callback)
    {
        if (bufferedCopyFunction == null || typeof(T) != bufferedType)
        {
            return;
        }

        T copy = (T)bufferedCopyFunction();
        elements.Insert(elements.IndexOf(element) + 1, copy);
        callback(elements);

        Dirty = true;
    }

    public static void Add<T>(List<T> elements,
        T element)
    {
        elements.Add(element);
    }

    public static T[] Add<T>(T[] elements,
        T element)
    {
        List<T> list = new List<T>(elements);
        list.Add(element);
        Dirty = true;
        return list.ToArray();
    }

    public static T[] RemoveAt<T>(T[] elements,
        int element)
    {
        List<T> list = new List<T>(elements);
        list.RemoveAt(element);
        Dirty = true;
        return list.ToArray();
    }

    private static void remove<T>(List<T> elements,
        T element,
        Action<IEnumerable<T>> callback)
    {
        elements.Remove(element);
        callback(elements);
        Dirty = true;
    }

    private static void delete<T>(List<T> elements,
        T element,
        Action<IEnumerable<T>> callback)
    {
        if (element is ScriptableObject)
        {
            remove<T>(elements, element, callback);
            ScriptableObject obj = (ScriptableObject)(object)element;
            GameObject.DestroyImmediate(obj, true);
        }
        callback(elements);
        Dirty = true;
    }

}

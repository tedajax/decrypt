using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;

public abstract class ScrollableEditorWindow : EditorWindow
{
    private Vector2 scrollPosition;

    protected abstract string Title { get; }

    public virtual void OnGUI()
    {
        drawTitle();

        drawHeader();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MinHeight(1000.0f));
        {
            draw();
        }
        EditorGUILayout.EndScrollView();
    }

    protected virtual void drawTitle()
    {
        GUIStyle fontStyle = new GUIStyle();
        fontStyle.font = (Font)Resources.Load("Fonts/Workhorse");
        fontStyle.fontSize = 30;
        fontStyle.alignment = TextAnchor.UpperCenter;
        fontStyle.normal.textColor = Color.white;
        fontStyle.hover.textColor = Color.white;
        EditorGUILayout.BeginVertical(EditorStyles.largeLabel);
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("", Title, fontStyle, GUILayout.Height(32));
            }
            EditorGUILayout.EndHorizontal();
            drawAlerts();
        }
        EditorGUILayout.EndVertical();
    }

    protected virtual void drawHeader() { }
    protected virtual void drawAlerts() { }
    protected abstract void draw();
}

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class TechTreeEditorWindow : EditorWindow
{
    private TechTreeDatabase techDatabase;
    private Vector2 panOffset;
    private Vector2 dragStart;
    private bool isDragging;

    private Dictionary<string, Rect> nodeRects = new Dictionary<string, Rect>();

    private Dictionary<string, Color> categoryColors = new Dictionary<string, Color>()
    {
        {"Automation", new Color(0.8f, 0.6f, 0.2f)},
        {"AI & Advanced", new Color(0.5f, 0.7f, 1f)},
        {"Construction", new Color(0.7f, 0.7f, 0.7f)},
        {"Electronics", new Color(0.3f, 0.9f, 0.6f)},
        {"Time Tech", new Color(0.7f, 0.3f, 0.9f)},
        {"Biology & Life Sciences", new Color(0.4f, 0.9f, 0.4f)},
        {"Transportation", new Color(0.9f, 0.7f, 0.2f)},
        {"Recycling & Environment", new Color(0.2f, 0.7f, 0.2f)},
        {"Exploration & Research", new Color(0.6f, 0.4f, 1f)},
        {"Health & Hygiene", new Color(1f, 0.6f, 0.6f)},
        {"Food", new Color(0.9f, 0.5f, 0.2f)},
        {"Water", new Color(0.2f, 0.6f, 1f)},
        {"Security", new Color(0.5f, 0.5f, 0.5f)},
        {"Shelter", new Color(0.7f, 0.5f, 0.3f)},
        {"Agriculture & Production", new Color(0.5f, 0.8f, 0.3f)},
        {"Tools & Construction", new Color(0.7f, 0.7f, 0.4f)},
        {"Energy", new Color(1f, 1f, 0.3f)},
        {"Communication", new Color(0.2f, 0.8f, 1f)},
    };

    private float zoom = 1f;
    private Vector2 zoomCoordsOrigin = Vector2.zero;
    private const float zoomMin = 0.5f;
    private const float zoomMax = 2.0f;

    private Vector2 scrollPosition;

    private const float canvasSize = 10000f;

    private string selectedNode = null; // نود انتخاب شده

    [MenuItem("Tools/Tech Tree Viewer")]
    public static void ShowWindow()
    {
        GetWindow<TechTreeEditorWindow>("Tech Tree Viewer");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        techDatabase = (TechTreeDatabase)EditorGUILayout.ObjectField("Tech Tree Database", techDatabase, typeof(TechTreeDatabase), false);
        EditorGUILayout.Space();

        if (techDatabase == null)
        {
            EditorGUILayout.HelpBox("Assign a TechTreeDatabase asset.", MessageType.Info);
            return;
        }

        HandleZoom();
        HandlePanning();
        HandleKeyboardInput();

        DrawZoomArea();
    }

    private void HandleZoom()
    {
        Event e = Event.current;
        if (e.type == EventType.ScrollWheel)
        {
            float oldZoom = zoom;
            zoom = Mathf.Clamp(zoom - e.delta.y * 0.05f, zoomMin, zoomMax);

            Vector2 mousePos = e.mousePosition;
            Vector2 delta = (mousePos - zoomCoordsOrigin - panOffset) - (mousePos - zoomCoordsOrigin - panOffset) * (zoom / oldZoom);
            panOffset += delta;
            e.Use();
        }
    }

    private void HandlePanning()
    {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 2)
        {
            isDragging = true;
            dragStart = e.mousePosition - panOffset;
            e.Use();
        }
        else if (e.type == EventType.MouseDrag && isDragging)
        {
            panOffset = e.mousePosition - dragStart;
            Repaint();
        }
        else if (e.type == EventType.MouseUp && isDragging)
        {
            isDragging = false;
        }
    }

    private void HandleKeyboardInput()
    {
        Event e = Event.current;
        if (selectedNode != null && nodeRects.ContainsKey(selectedNode))
        {
            Vector2 moveAmount = Vector2.zero;
            float moveStep = 20f;

            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.LeftArrow)
                {
                    moveAmount.x += moveStep;
                    e.Use();
                }
                else if (e.keyCode == KeyCode.RightArrow)
                {
                    moveAmount.x -= moveStep;
                    e.Use();
                }
                else if (e.keyCode == KeyCode.UpArrow)
                {
                    moveAmount.y += moveStep;
                    e.Use();
                }
                else if (e.keyCode == KeyCode.DownArrow)
                {
                    moveAmount.y -= moveStep;
                    e.Use();
                }

                if (moveAmount != Vector2.zero)
                {
                    panOffset += moveAmount;
                    Repaint();
                }
            }
        }
    }

    private void DrawZoomArea()
    {
        Rect viewRect = new Rect(0, 0, position.width, position.height);
        Rect canvasRect = new Rect(0, 0, canvasSize, canvasSize);

        scrollPosition = GUI.BeginScrollView(viewRect, scrollPosition, canvasRect);

        Matrix4x4 prevMatrix = GUI.matrix;

        GUI.matrix = Matrix4x4.TRS(zoomCoordsOrigin + panOffset, Quaternion.identity, Vector3.one * zoom) *
                     Matrix4x4.TRS(-zoomCoordsOrigin, Quaternion.identity, Vector3.one);

        DrawTechTree();

        GUI.matrix = prevMatrix;

        GUI.EndScrollView();
    }

    private void DrawTechTree()
    {
        if (techDatabase.techItems == null) return;
        nodeRects.Clear();

        var categories = categoryColors.Keys.ToList();
        var itemsByCategory = new Dictionary<string, List<TechItem>>();

        foreach (var cat in categories)
            itemsByCategory[cat] = new List<TechItem>();

        foreach (var item in techDatabase.techItems)
        {
            if (!itemsByCategory.ContainsKey(item.category))
                itemsByCategory[item.category] = new List<TechItem>();

            itemsByCategory[item.category].Add(item);
        }

        var levels = new Dictionary<string, int>();
        foreach (var item in techDatabase.techItems)
            levels[item.name] = (item.prerequisites == null || item.prerequisites.Count == 0) ? 0 : -1;

        bool changed = true;
        while (changed)
        {
            changed = false;
            foreach (var item in techDatabase.techItems)
            {
                if (levels[item.name] == -1)
                {
                    int maxPre = -1;
                    bool allSet = true;
                    foreach (var pre in item.prerequisites)
                    {
                        if (!levels.ContainsKey(pre) || levels[pre] == -1)
                        {
                            allSet = false;
                            break;
                        }
                        maxPre = Mathf.Max(maxPre, levels[pre]);
                    }
                    if (allSet)
                    {
                        levels[item.name] = maxPre + 1;
                        changed = true;
                    }
                }
            }
        }

        int maxLevel = levels.Values.Max();

        float xSpacing = 320f;
        float ySpacing = 320f;
        float nodeWidth = 220f;
        float nodeHeight = 90f;
        float catSpace = 80f;
        float levelSpace = 60f;
        float startX = 100f;
        float startY = 80f;

        for (int catIdx = 0; catIdx < categories.Count; catIdx++)
        {
            string cat = categories[catIdx];
            float x = startX + catIdx * (nodeWidth + catSpace);
            Rect headerRect = new Rect(x, startY - 40f, nodeWidth, 30f);
            EditorGUI.DrawRect(headerRect, categoryColors[cat] * 0.5f + Color.white * 0.5f);
            GUI.Label(headerRect, cat, EditorStyles.boldLabel);
        }

        for (int catIdx = 0; catIdx < categories.Count; catIdx++)
        {
            string cat = categories[catIdx];
            var items = itemsByCategory[cat];

            var itemsByLevel = new Dictionary<int, List<TechItem>>();
            foreach (var item in items)
            {
                int lvl = levels[item.name];
                if (!itemsByLevel.ContainsKey(lvl)) itemsByLevel[lvl] = new List<TechItem>();
                itemsByLevel[lvl].Add(item);
            }

            for (int lvl = 0; lvl <= maxLevel; lvl++)
            {
                if (!itemsByLevel.ContainsKey(lvl)) continue;
                var itemsAtCell = itemsByLevel[lvl];

                float cellHeight = nodeHeight * Mathf.Max(1, itemsAtCell.Count) + 10f * (itemsAtCell.Count - 1);
                float x = startX + catIdx * (nodeWidth + catSpace);
                float y = startY + lvl * (nodeHeight + levelSpace) + (cellHeight - (itemsAtCell.Count * nodeHeight + 10f * (itemsAtCell.Count - 1))) / 2f;

                for (int i = 0; i < itemsAtCell.Count; i++)
                {
                    var item = itemsAtCell[i];
                    Rect nodeRect = new Rect(x, y + i * (nodeHeight + 10f), nodeWidth, nodeHeight);
                    nodeRects[item.name] = nodeRect;

                    DrawNode(nodeRect, item);
                }
            }
        }

        // رسم خطوط ارتباط
        foreach (var item in techDatabase.techItems)
        {
            if (item.prerequisites == null) continue;
            Rect toRect = nodeRects[item.name];
            foreach (var pre in item.prerequisites)
            {
                if (!nodeRects.ContainsKey(pre)) continue;
                Rect fromRect = nodeRects[pre];
                Vector3 startPos = new Vector3(fromRect.xMax, fromRect.y + fromRect.height / 2f, 0);
                Vector3 endPos = new Vector3(toRect.xMin, toRect.y + toRect.height / 2f, 0);
                Handles.DrawBezier(startPos, endPos, startPos + Vector3.right * 50, endPos + Vector3.left * 50, Color.white, null, 3f);
            }
        }

        // رسم border دور نود انتخاب شده
        if (selectedNode != null && nodeRects.ContainsKey(selectedNode))
        {
            Rect selectedRect = nodeRects[selectedNode];
            Color borderColor = Color.yellow;
            float borderWidth = 4f;

            Handles.BeginGUI();
            Handles.color = borderColor;
            Vector3 p1 = new Vector3(selectedRect.xMin, selectedRect.yMin);
            Vector3 p2 = new Vector3(selectedRect.xMax, selectedRect.yMin);
            Vector3 p3 = new Vector3(selectedRect.xMax, selectedRect.yMax);
            Vector3 p4 = new Vector3(selectedRect.xMin, selectedRect.yMax);
            Handles.DrawAAPolyLine(borderWidth, p1, p2, p3, p4, p1);
            Handles.EndGUI();
        }

        HandleNodeSelection();
    }

    private void DrawNode(Rect rect, TechItem item)
    {
        Color bgColor;
        if (!categoryColors.TryGetValue(item.category, out bgColor))
            bgColor = Color.gray;

        EditorGUI.DrawRect(rect, bgColor * 0.6f);

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(rect.x, rect.y + 15, rect.width, 20), item.name, titleStyle);

        GUIStyle descStyle = new GUIStyle(EditorStyles.label);
        descStyle.wordWrap = true;
        descStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(rect.x + 15, rect.y + 30, rect.width - 10, rect.height - 35), item.description, descStyle);
    }

    private void HandleNodeSelection()
    {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Vector2 mousePos = (e.mousePosition - panOffset - zoomCoordsOrigin) / zoom + zoomCoordsOrigin;

            foreach (var kvp in nodeRects)
            {
                if (kvp.Value.Contains(mousePos))
                {
                    selectedNode = kvp.Key;
                    Repaint();
                    break;
                }
            }
        }
    }
}

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Deepwave.Core.Editor
{
    /// <summary>
    /// Custom Unity Inspector drawer that enhances the appearance of MonoBehaviour inspectors
    /// with a stylized header and bordered layout, applying a horror-themed aesthetic.
    /// Inherits from Unity's Editor class to override the default Inspector GUI.
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
    public class CustomBehaviourInspector : UnityEditor.Editor
    {
        private const float HeaderHeight = 38f;
        private const int HeaderFontSize = 12;

        private static readonly Color HeaderBackgroundColor = new Color32(40, 5, 5, 255);
        private static readonly Color HeaderTextColor = new Color32(220, 220, 220, 255);
        private static readonly Color TextShadowColor = new(0, 0, 0, 0.5f);
        private static readonly Color BorderColor = new Color32(50, 0, 0, 255);
        private static readonly Color BorderBackgroundColor = new Color32(20, 20, 20, 255);

        private InspectorHeaderAttribute _headerAttribute;
        private bool _shouldDisplayHeader;
        private static Texture2D _gradientTexture;

        private void OnEnable()
        {
            _headerAttribute = target.GetType().GetCustomAttribute<InspectorHeaderAttribute>(false);
            _shouldDisplayHeader = _headerAttribute != null;
        }

        public override void OnInspectorGUI()
        {
            if (_shouldDisplayHeader)
                DrawCustomInspector();
            else
                base.OnInspectorGUI();
        }

        private void DrawCustomInspector()
        {
            Rect outerRect = EditorGUILayout.BeginVertical();
            Rect borderRect = GetBorderRect(outerRect);
            bool hasVisibleFields = HasVisibleFields(serializedObject);

            EditorGUI.DrawRect(borderRect, BorderBackgroundColor);

            using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
            {
                DrawHeader(CreateHeaderContent(), target);
                serializedObject.Update();

                DrawOuterBorder(borderRect);
                if (hasVisibleFields)
                    EditorGUILayout.Space(10f);

                using var changeScope = new EditorGUI.ChangeCheckScope();
                DrawPropertiesExcluding(serializedObject, "m_Script");
                if (changeScope.changed)
                    serializedObject.ApplyModifiedProperties();
            }

            if (hasVisibleFields)
                EditorGUILayout.Space(10f);

            EditorGUILayout.EndVertical();
        }

        private static bool HasVisibleFields(SerializedObject serializedObject)
        {
            var prop = serializedObject.GetIterator();
            bool enterChildren = true;
            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (prop.name != "m_Script")
                    return true;
            }
            return false;
        }

        private static Rect GetBorderRect(Rect rect)
        {
            const float borderOffsetX = 16f;
            const float borderExpandWidth = 18f;
            return new Rect(rect.x - borderOffsetX, rect.y, rect.width + borderExpandWidth, rect.height);
        }

        private GUIContent CreateHeaderContent()
        {
            string title = _headerAttribute.title.ToUpperInvariant();
            return string.IsNullOrEmpty(_headerAttribute.icon)
                ? new GUIContent(title)
                : EditorGUIUtility.TrTextContentWithIcon($" {title}", _headerAttribute.icon);
        }

        private static void DrawOuterBorder(Rect rect)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), BorderColor);
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 1, rect.width, 1), BorderColor);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 1, rect.height), BorderColor);
            EditorGUI.DrawRect(new Rect(rect.xMax - 1, rect.y, 1, rect.height), BorderColor);
        }

        public static void DrawHeader(GUIContent headerContent, UnityEngine.Object targetObject = null)
        {
            var headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = HeaderFontSize,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = HeaderTextColor },
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(0, 0, 2, 2)
            };

            using (new EditorGUILayout.VerticalScope())
            {
                Rect headerRect = GUILayoutUtility.GetRect(1, HeaderHeight);
                Rect borderRect = GetBorderRect(headerRect);

                DrawGradientBackground(borderRect, new Color32(10, 10, 10, 255), HeaderBackgroundColor);

                var overlayTexture = Resources.Load<Texture2D>("EditorIcons/horror_grunge");
                if (overlayTexture != null)
                    GUI.DrawTexture(borderRect, overlayTexture, ScaleMode.StretchToFill, true, 0.3f);

                if (targetObject != null)
                {
                    var monoScript = GetMonoScript(targetObject);
                    if (monoScript != null)
                        HandleScriptPing(borderRect, monoScript);
                }

                DrawGlowingText(borderRect, headerContent, headerStyle);
            }
        }

        private static void DrawGradientBackground(Rect rect, Color startColor, Color endColor)
        {
            if (_gradientTexture == null)
                _gradientTexture = GenerateVerticalGradientTexture(1, 64, startColor, endColor);
            GUI.DrawTexture(rect, _gradientTexture, ScaleMode.StretchToFill);
        }

        private static Texture2D GenerateVerticalGradientTexture(int width, int height, Color topColor, Color bottomColor)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int y = 0; y < height; y++)
            {
                Color color = Color.Lerp(topColor, bottomColor, (float)y / (height - 1));
                for (int x = 0; x < width; x++)
                    texture.SetPixel(x, y, color);
            }
            texture.Apply();
            texture.wrapMode = TextureWrapMode.Clamp;
            return texture;
        }

        private static void DrawGlowingText(Rect rect, GUIContent content, GUIStyle style)
        {
            var shadowRect = new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height);
            var shadowStyle = new GUIStyle(style) { normal = { textColor = TextShadowColor } };
            EditorGUI.LabelField(shadowRect, content, shadowStyle);
            EditorGUI.LabelField(rect, content, style);
        }

        private static MonoScript GetMonoScript(UnityEngine.Object targetObject)
        {
            return targetObject switch
            {
                MonoBehaviour monoBehaviour => MonoScript.FromMonoBehaviour(monoBehaviour),
                ScriptableObject scriptableObject => MonoScript.FromScriptableObject(scriptableObject),
                _ => null
            };
        }

        private static void HandleScriptPing(Rect headerRect, MonoScript monoScript)
        {
            var evt = Event.current;
            if (headerRect.Contains(evt.mousePosition) && evt.type == EventType.MouseDown && evt.button == 0)
                EditorGUIUtility.PingObject(monoScript);
        }
    }
}

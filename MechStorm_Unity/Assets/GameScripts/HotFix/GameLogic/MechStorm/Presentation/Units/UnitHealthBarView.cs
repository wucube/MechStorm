using System;
using UnityEngine;
using UnityEngine.UI;

namespace MechStorm.Presentation.Units
{
    public sealed class UnitHealthBarView
    {
        private const float CanvasScale = 0.01f;
        private static readonly Vector2 CanvasSize = new(120f, 28f);
        private static readonly Vector2 BarSize = new(100f, 10f);

        private readonly Transform _canvasTransform;
        private readonly RectTransform _fillRect;
        private readonly Text _valueText;
        private readonly Camera _camera;

        public UnitHealthBarView(Transform followTarget, Camera camera)
        {
            if (followTarget == null)
            {
                throw new ArgumentNullException(nameof(followTarget));
            }

            _camera = camera;
            var canvas = CreateCanvas(followTarget);
            _canvasTransform = canvas.transform;

            var backgroundRect = CreateImage("Background", canvas.transform, BarSize, new Vector2(0f, 3f), new Color(0f, 0f, 0f, 0.7f));
            _fillRect = CreateImage("Fill", backgroundRect, BarSize, Vector2.zero, new Color(0.2f, 0.9f, 0.25f, 1f));
            _fillRect.anchorMin = new Vector2(0f, 0.5f);
            _fillRect.anchorMax = new Vector2(0f, 0.5f);
            _fillRect.pivot = new Vector2(0f, 0.5f);
            _fillRect.anchoredPosition = Vector2.zero;

            _valueText = CreateText(canvas.transform);
        }

        public void RefreshValue(int currentDurability, int maxDurability)
        {
            var safeMax = Mathf.Max(0, maxDurability);
            var safeCurrent = Mathf.Clamp(currentDurability, 0, safeMax);
            var ratio = safeMax > 0 ? (float)safeCurrent / safeMax : 0f;

            _fillRect.localScale = new Vector3(ratio, 1f, 1f);
            _valueText.text = $"{safeCurrent}/{safeMax}";
        }

        public void RefreshFacing()
        {
            var camera = _camera != null ? _camera : Camera.main;
            if (camera == null)
            {
                return;
            }

            _canvasTransform.rotation = camera.transform.rotation;
        }

        private static Canvas CreateCanvas(Transform followTarget)
        {
            var canvasObject = new GameObject("UnitHealthBarCanvas", typeof(RectTransform), typeof(Canvas));
            canvasObject.transform.SetParent(followTarget, false);
            canvasObject.transform.localPosition = Vector3.zero;
            canvasObject.transform.localRotation = Quaternion.identity;
            canvasObject.transform.localScale = Vector3.one * CanvasScale;

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var rectTransform = (RectTransform)canvasObject.transform;
            rectTransform.sizeDelta = CanvasSize;

            return canvas;
        }

        private static RectTransform CreateImage(string name, Transform parent, Vector2 size, Vector2 anchoredPosition, Color color)
        {
            var imageObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imageObject.transform.SetParent(parent, false);

            var rectTransform = (RectTransform)imageObject.transform;
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = anchoredPosition;

            var image = imageObject.GetComponent<Image>();
            image.color = color;

            return rectTransform;
        }

        private static Text CreateText(Transform parent)
        {
            var textObject = new GameObject("ValueText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObject.transform.SetParent(parent, false);

            var rectTransform = (RectTransform)textObject.transform;
            rectTransform.sizeDelta = new Vector2(120f, 14f);
            rectTransform.anchoredPosition = new Vector2(0f, -8f);

            var text = textObject.GetComponent<Text>();
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.fontSize = 12;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            return text;
        }
    }
}

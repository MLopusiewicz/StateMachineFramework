using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateMachineFramework.View {
    public class GridDrawerVE : VisualElement {
        public class GridDrawerFactory : UxmlFactory<GridDrawerVE, GridDrawerTraits> { }
        public class GridDrawerTraits : VisualElement.UxmlTraits {
            UxmlFloatAttributeDescription gridWidth = new UxmlFloatAttributeDescription { name = "GridWidth", defaultValue = 160 };
            UxmlFloatAttributeDescription lineWidth = new UxmlFloatAttributeDescription { name = "LineWidth", defaultValue = 1 };
            UxmlIntAttributeDescription divisions = new UxmlIntAttributeDescription { name = "Divisions", defaultValue = 5 };
            UxmlFloatAttributeDescription minorLineWidth = new UxmlFloatAttributeDescription { name = "MinorLineWidth", defaultValue = 0.1f };

            UxmlColorAttributeDescription majorLinesColor = new UxmlColorAttributeDescription { name = "MajorLinesColor", defaultValue = Color.white };
            UxmlColorAttributeDescription minorLinesColor = new UxmlColorAttributeDescription { name = "minorLinesColor", defaultValue = Color.black };
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
                get {
                    yield return new UxmlChildElementDescription(typeof(VisualElement));
                }
            }
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var target = ve.Q<GridDrawerVE>();

                target._gap = gridWidth.GetValueFromBag(bag, cc);
                target._majorLineWidth = lineWidth.GetValueFromBag(bag, cc);
                target._divisions = divisions.GetValueFromBag(bag, cc);
                target._minorLineWidth = minorLineWidth.GetValueFromBag(bag, cc);
                target._majorLineColor = majorLinesColor.GetValueFromBag(bag, cc);
                target._minorLineColor = minorLinesColor.GetValueFromBag(bag, cc);
            }
        }

        static readonly CustomStyleProperty<float> gridGap = new CustomStyleProperty<float>("--grid-gap");
        static readonly CustomStyleProperty<int> gridDivisions = new CustomStyleProperty<int>("--grid-divisions");
        static readonly CustomStyleProperty<float> majorLineWidth = new CustomStyleProperty<float>("--major-line__width");
        static readonly CustomStyleProperty<Color> majorLineColor = new CustomStyleProperty<Color>("--major-line__color");

        static readonly CustomStyleProperty<float> minorLineWidth = new CustomStyleProperty<float>("--minor-line__width");
        static readonly CustomStyleProperty<Color> minorLineColor = new CustomStyleProperty<Color>("--minor-line__color");

        protected void OnStylesResolved(CustomStyleResolvedEvent evt) {

            if (evt.customStyle.TryGetValue(gridGap, out _gap))
                MarkDirtyRepaint();

            if (evt.customStyle.TryGetValue(gridDivisions, out _divisions))
                MarkDirtyRepaint();

            if (evt.customStyle.TryGetValue(majorLineWidth, out _majorLineWidth))
                MarkDirtyRepaint();
            if (evt.customStyle.TryGetValue(majorLineColor, out _majorLineColor))
                MarkDirtyRepaint();

            if (evt.customStyle.TryGetValue(minorLineWidth, out _minorLineWidth))
                MarkDirtyRepaint();
            if (evt.customStyle.TryGetValue(minorLineColor, out _minorLineColor))
                MarkDirtyRepaint();
        }
        public float Gap => _gap;
        float _gap;
        int _divisions;
        float _majorLineWidth;
        Color _majorLineColor;
        float _minorLineWidth;
        Color _minorLineColor;

        public GridDrawerVE() {
            RegisterCallback<CustomStyleResolvedEvent>(OnStylesResolved);
            generateVisualContent += GenerateVisualContent;
        }

        private void GenerateVisualContent(MeshGenerationContext context) {
            var painter = context.painter2D;
            if (_gap == 0)
                return;
            var majorAmount = (this.localBound.size / _gap);
            for (int i = 0; i < majorAmount.x; i++) {
                painter.lineWidth = _majorLineWidth;
                painter.strokeColor = _majorLineColor;
                var w = Vector2.right * i * _gap;
                MakeLine(painter, w, w + this.localBound.height * Vector2.up);

                for (int j = 0; j < _divisions - 1; j++) {
                    painter.lineWidth = _minorLineWidth;
                    painter.strokeColor = _minorLineColor;
                    w = w + Vector2.right * (_gap / _divisions);
                    MakeLine(painter, w, w + this.localBound.height * Vector2.up);
                }
            }
            for (int i = 0; i < majorAmount.y; i++) {
                painter.lineWidth = _majorLineWidth;
                painter.strokeColor = _majorLineColor;
                var w = Vector2.up * i * _gap;
                MakeLine(painter, w + this.localBound.width * Vector2.right, w);

                for (int j = 0; j < _divisions - 1; j++) {
                    painter.lineWidth = _minorLineWidth;
                    painter.strokeColor = _minorLineColor;
                    w = w + Vector2.up * (_gap / _divisions);
                    MakeLine(painter, w + this.localBound.width * Vector2.right, w);
                }
            }

        }
        void MakeLine(Painter2D painter, Vector2 start, Vector2 end) {
            painter.BeginPath();
            painter.LineTo(start);
            painter.LineTo(end);
            painter.Stroke();
        }

    }
}
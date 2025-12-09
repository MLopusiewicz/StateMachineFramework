using UnityEngine;
using UnityEngine.UIElements;

namespace StateMachineFramework.View {
    public class ViewPortVE : VisualElement {

        public const string VIEW_PORT = "view-port";
        public const string VIEW_PORT_CONTAINER = "view-port-container";
        public const string VIEW_PORT_GRID = "view-port-grid";
        public class ViewPortFactory : UxmlFactory<ViewPortVE, ViewPortTraits> { }
        public class ViewPortTraits : UxmlTraits {
            public UxmlFloatAttributeDescription zoomMultiplier = new UxmlFloatAttributeDescription() { name = "ZoomMultiplier", defaultValue = 0.5f };

            public UxmlFloatAttributeDescription zoomClampMax = new UxmlFloatAttributeDescription() { name = "ZoomMax", defaultValue = 3f };
            public UxmlFloatAttributeDescription zoomClampMin = new UxmlFloatAttributeDescription() { name = "ZoomMin", defaultValue = 0.5f };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var target = ve.Q<ViewPortVE>();
                target.ZoomMultiplier = zoomMultiplier.GetValueFromBag(bag, cc);
                target.ZoomMin = zoomClampMin.GetValueFromBag(bag, cc);
                target.ZoomMax = zoomClampMax.GetValueFromBag(bag, cc);
            }
        }

        VisualElement container;
        public float ZoomMultiplier { get; set; }
        public float ZoomMin { get; set; }
        public float ZoomMax { get; set; }

        float zoomScale = 1;
        bool isDragging;
        public float ZoomScale => zoomScale;
        public override VisualElement contentContainer => container != null ? container : this;
        GridDrawerVE grid;
        public ViewPortVE() {
            SetupContainers();

            this.RegisterCallback<WheelEvent>(OnScrolled);
            this.RegisterCallback<MouseDownEvent>(OnMouseDown);
            this.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            this.RegisterCallback<MouseUpEvent>(OnMouseUp); 
            this.RegisterCallback<KeyDownEvent>(Recenter, TrickleDown.TrickleDown);
        }

        private void Recenter(KeyDownEvent evt) {
            if (evt.target == this)
                if (evt.keyCode == KeyCode.A) {
                    Recenter();
                }
        }

        void SetupContainers() {
            this.AddToClassList(VIEW_PORT);
            grid = VEHelper.Make<GridDrawerVE>(this, "Grid", VIEW_PORT_GRID);
            grid.pickingMode = PickingMode.Ignore;
            container = VEHelper.Make<VisualElement>(this, "ContentContainer", VIEW_PORT_CONTAINER);
            container.pickingMode = PickingMode.Ignore;
        }

        private void OnScrolled(WheelEvent evt) {
            if (evt.shiftKey) {
                MoveContainer(-Vector2.up * 10 * evt.delta.x);
            } else if (evt.altKey) {
                MoveContainer(-Vector2.right * 10 * evt.delta.y);
            } else {

                zoomScale -= evt.delta.y * ZoomMultiplier;
                zoomScale = Mathf.Clamp(zoomScale, ZoomMin, ZoomMax);
                Zoom(zoomScale, evt.mousePosition);
            }

            evt.StopPropagation();
        }

        private void OnMouseDown(MouseDownEvent evt) {
            if (evt.button == 2)
                if (evt.target == this)
                    isDragging = true;

        }
        private void OnMouseMove(MouseMoveEvent evt) {
            if (isDragging) {
                MoveContainer(evt.mouseDelta);
            }
        }
        void MoveContainer(Vector3 v) {
            container.transform.position += v;
            grid.transform.position += v;
        }

        void Zoom(float targetScale, Vector3 pos) {
            var transformed = container.WorldToLocal(pos);
            container.transform.scale = Vector3.one * targetScale;
            grid.transform.scale = Vector3.one * targetScale;
            var post = container.WorldToLocal(pos);
            MoveContainer(-(transformed - post));
        }

        private void OnMouseUp(MouseUpEvent evt) {
            isDragging = false;
            var offset = new Vector2(grid.transform.position.x % grid.Gap, grid.transform.position.y % grid.Gap);
            grid.transform.position = offset;
        }
        public void Center(Vector2 pos) {
            container.transform.position = (-pos + this.localBound.size / 2) * zoomScale;
        }

        public void Recenter() {
            Vector2 center = Vector2.zero;
            int counter = 0;

            this.Query<NodeVE>().ForEach((ve) => {
                center += ve.localBound.center;
                counter++;
            });
            if (counter > 0)
                Center(center / counter);
        }
    }

}
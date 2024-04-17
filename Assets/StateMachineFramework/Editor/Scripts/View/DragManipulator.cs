using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateMachineFramework.View {
    public class DragManipulator : Manipulator {
        bool isDragging;
        VisualElement container;
        public Action<Vector2> OnPositionSet;
        ViewPortVE viewPort;
        public DragManipulator(VisualElement container, ViewPortVE viewPort) {
            this.container = container;
            this.viewPort = viewPort;
        }

        protected override void RegisterCallbacksOnTarget() {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            target.RegisterCallback<FocusOutEvent>(FocusOut);
            target.RegisterCallback<MouseOutEvent>(OnMouseOut);
        }


        protected override void UnregisterCallbacksFromTarget() {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            target.UnregisterCallback<FocusOutEvent>(FocusOut);
            target.UnregisterCallback<MouseOutEvent>(OnMouseOut);

        }

        private void OnMouseOut(MouseOutEvent evt) {
            EndDrag();
        }
        private void OnMouseDown(MouseDownEvent evt) {
            if (evt.target == target) {
                if (evt.button == 0) {
                    isDragging = true;
                    target.BringToFront();
                }
            }

        }
        private void OnMouseMove(MouseMoveEvent evt) {
            if (isDragging) {
                Vector3 translation = evt.mouseDelta / viewPort.ZoomScale;
                target.transform.position += translation;
            }
        }

        private void OnMouseUp(MouseUpEvent evt) {
            EndDrag();
        }

        private void FocusOut(FocusOutEvent evt) {
            EndDrag();
        }
        void EndDrag() {
            OnPositionSet?.Invoke(target.transform.position);
            isDragging = false;
        }

    }
}
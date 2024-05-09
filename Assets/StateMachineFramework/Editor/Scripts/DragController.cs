using StateMachineFramework.View;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {
    public class DragController {
        public bool IsDragging { get; private set; }
        List<NodeVE> selectedNodes;
        ViewPortVE viewPort;
        public Action<List<NodeVE>> OnPositionSet;
        public List<NodeVE> draggedNodes = new();
        float dragTreshhold = 10;
        Vector2 drag = Vector2.zero;

        public DragController(List<NodeVE> selection, ViewPortVE viewPort) {
            selectedNodes = selection;
            this.viewPort = viewPort;
        }
        public void NodeAdded(NodeVE ve) {
            ve.RegisterCallback<MouseDownEvent>(OnMouseDown);
            ve.RegisterCallback<MouseUpEvent>(OnMouseUp);

            viewPort.RegisterCallback<MouseUpEvent>(OnMouseUp);
            viewPort.RegisterCallback<MouseLeaveEvent>(OnMouseOut);
        }

        private void OnMouseOut(MouseLeaveEvent evt) {
            EndDrag();
        }

        private void OnMouseDown(MouseDownEvent evt) {

            if (evt.button != 0)
                return;
            if (evt.target is not NodeVE node)
                return;

            if (selectedNodes.Contains(node) || evt.shiftKey)
                draggedNodes.AddRange(selectedNodes);

            if (!draggedNodes.Contains(node))
                draggedNodes.Add(node);

            foreach (var n in draggedNodes)
                n.BringToFront();
            drag = Vector3.zero;

            viewPort.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        }

        private void OnMouseMove(MouseMoveEvent evt) {
            drag += evt.mouseDelta / viewPort.ZoomScale;
            if (drag.magnitude > dragTreshhold && !IsDragging) {
                IsDragging = true;
                foreach (var n in draggedNodes)
                    n.transform.position += (Vector3)drag;

            }

            if (IsDragging) {
                Vector3 translation = evt.mouseDelta / viewPort.ZoomScale;
                foreach (var n in draggedNodes)
                    n.transform.position += translation;
            }
        }

        private void OnMouseUp(MouseUpEvent evt) {
            EndDrag();
        }

        void EndDrag() {
            if (IsDragging) {
                OnPositionSet?.Invoke(draggedNodes);
                IsDragging = false;
            }
            draggedNodes.Clear();

            viewPort.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        }

    }
}
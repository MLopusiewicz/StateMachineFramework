using StateMachineFramework.View;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {
    public class DragController {
        bool isDragging;
        List<NodeVE> selectedNodes;
        ViewPortVE viewPort;
        public Action OnPositionSet;

        public DragController(List<NodeVE> selection, ViewPortVE viewPort) {
            selectedNodes = selection;
            this.viewPort = viewPort;
        }
        public void NodeAdded(NodeVE ve) {
            ve.RegisterCallback<MouseDownEvent>(OnMouseDown);
            ve.RegisterCallback<MouseUpEvent>(OnMouseUp);

            viewPort.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            viewPort.RegisterCallback<MouseLeaveEvent>(OnMouseOut);
        }

        private void OnMouseOut(MouseLeaveEvent evt) {
            EndDrag();
        }

        private void OnMouseDown(MouseDownEvent evt) {
            if (evt.target is NodeVE node) {
                if (selectedNodes.Contains(node))
                    if (evt.button == 0) {
                        isDragging = true;
                        foreach (var n in selectedNodes)
                            n.BringToFront();
                    }
            }

        }
        private void OnMouseMove(MouseMoveEvent evt) {
            if (isDragging) {
                Vector3 translation = evt.mouseDelta / viewPort.ZoomScale;
                foreach (var n in selectedNodes)
                    n.transform.position += translation;
            }
        }

        private void OnMouseUp(MouseUpEvent evt) {
            EndDrag();
        }

        void EndDrag() {
            if (isDragging) {
                OnPositionSet?.Invoke();
                isDragging = false;
            }
        }

    }
}
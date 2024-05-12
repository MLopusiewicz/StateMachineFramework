using StateMachineFramework.Runtime;
using StateMachineFramework.View;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateMachineFramework.Editor {
    public class NodeTreeView {
        public ViewPortVE viewPort;
        public VisualElement nodeContainer;
        public TwoWayDictionary<NodeVE, Node> nodes = new();
        public StateMachineEditor editor;

        DragController drag;

        List<NodeVE> selectedNodes = new();

        public NodeTreeView(StateMachineEditor editor) {
            viewPort = editor.rootVisualElement.Q<ViewPortVE>();
            nodeContainer = viewPort.Q(name: "NodeContainer");
            viewPort.AddManipulator(
               new ContextualMenuManipulator((evt) => {
                   var p = evt.localMousePosition;
                   evt.menu.AppendAction("Add node", (x) => CreateNode(p), DropdownMenuAction.AlwaysEnabled);
                   evt.menu.AppendAction("Add Tree", (x) => CreateTree(p), DropdownMenuAction.AlwaysEnabled);
               }));
            this.editor = editor;

            viewPort.RegisterCallback<KeyDownEvent>(KeyPressed, TrickleDown.TrickleDown);

            this.editor.depthPanel.OnDepthChanged += this.Init;
            drag = new DragController(selectedNodes, viewPort);
            drag.OnPositionSet += UpdatePositions;
            this.editor.selection.OnSelectionCleared += ClearSelection;
        }

        public void Init(TreeNode tree) {
            nodes.Clear();
            nodeContainer.contentContainer.Clear();
            foreach (var n in tree.nodes) {
                MakeNodeVE(n);
            }

            MakeNodeVE(tree.enterNode).AddToClassList(NodeVE.NODE_SPECIAL_ENTER);
            MakeNodeVE(tree.exitNode).AddToClassList(NodeVE.NODE_SPECIAL_EXIT);
            editor.stateMachine.anyState.position = tree.anyPos;
            MakeNodeVE(editor.stateMachine.anyState).AddToClassList(NodeVE.NODE_SPECIAL_ANY);
            editor.depthPanel.AddDepthLayer(tree);
        }

        public void Clear() {
            nodes.Clear();
            nodeContainer.contentContainer.Clear();
        }

        public void CreateNode(Vector2 position) {
            position = viewPort.contentContainer.WorldToLocal(position);
            var node = new Node() { name = "new node" };
            var ser = editor.serialization.GetSerializedNode(editor.depthPanel.ActiveTree).FindPropertyRelative("nodes");
            ser.arraySize++;
            node.position = position;
            ser.GetArrayElementAtIndex(ser.arraySize - 1).managedReferenceValue = node;
            editor.serialization.Apply();
            editor.serialization.AddNode(node);
            Redraw();

        }

        void CreateTree(Vector2 p) {
            p = viewPort.contentContainer.WorldToLocal(p);
            editor.serialization.MakeTree(p);
            Redraw();
        }

        NodeVE MakeNodeVE(Node node) {
            NodeVE n = new NodeVE();
            n.Init(node, editor.serialization.GetSerializedNode(node));
            nodeContainer.Add(n);
            n.AddManipulator(
                new ContextualMenuManipulator((evt) => {
                    var p = evt.localMousePosition;
                    evt.menu.ClearItems();

                    SpecialNode special = node as SpecialNode;
                    if (special != null && special.canBeSource) {
                        evt.menu.AppendAction("Make transition", (x) => { editor.transitionMaker.Start(nodes[n]); }, DropdownMenuAction.AlwaysEnabled);
                    }

                    if (special == null) {
                        evt.menu.AppendAction("Make transition", (x) => { editor.transitionMaker.Start(nodes[n]); }, DropdownMenuAction.AlwaysEnabled);
                        evt.menu.AppendAction("Remove", (x) => RemoveSelected(), DropdownMenuAction.AlwaysEnabled);
                        evt.menu.AppendAction("Rename", (x) => n.RenameState(), DropdownMenuAction.AlwaysEnabled);
                    }
                    evt.StopPropagation();
                }));
            nodes.Add(n, node);


            if (node is TreeNode t) {
                n.AddToClassList("node-tree");
                n.RegisterCallback<MouseDownEvent>((x) => TreeClick(x, t));
            }

            n.RegisterCallback<MouseDownEvent>((x) => NodeClicked(x, node));
            n.RegisterCallback<MouseUpEvent>(NodeSelection);
            drag.NodeAdded(n);
            return n;
        }


        void UpdatePositions(List<NodeVE> sel) {
            foreach (var a in sel) {

                if (nodes[a] is SpecialNode sn) {
                    var tree = editor.serialization.GetSerializedNode(editor.depthPanel.ActiveTree);
                    tree.FindPropertyRelative($"{sn.name.Split(" ")[0].ToLower()}Pos").vector2Value = a.transform.position;
                } else {
                    editor.serialization.GetSerializedNode(nodes[a]).FindPropertyRelative("position").vector2Value = a.transform.position;
                }
                editor.serialization.Apply();
            }
        }

        void KeyPressed(KeyDownEvent evt) {

            if (evt.keyCode == KeyCode.Delete) {
                RemoveSelected();
            }
        }

        void RemoveSelected() {
            if (selectedNodes.Count == 0)
                return;
            foreach (var ve in selectedNodes) {
                editor.serialization.RemoveNode(nodes[ve]);
                nodes.Remove(ve);
            }
            editor.serialization.Apply();
            selectedNodes.Clear();
            Redraw();
        }

        void TreeClick(MouseDownEvent evt, TreeNode t) {
            if (evt.button != 0)
                return;

            if (evt.clickCount == 2) {
                Init(t);
                evt.StopImmediatePropagation();
            }
        }


        void NodeClicked(MouseDownEvent x, Node n) {

            NodeVE target = x.target as NodeVE;

            if (x.clickCount != 1)
                return;

            if (editor.transitionMaker.isPicking) {
                editor.transitionMaker.MakeTransition(n);
                return;
            }

            if (x.button != 0)
                return;

        }

        private void NodeSelection(MouseUpEvent x) {

            NodeVE target = x.target as NodeVE;


            foreach (var a in selectedNodes) {
                a.RemoveFromClassList("selected");
            }
            if (drag.IsDragging) {
                selectedNodes.Clear();
                selectedNodes.AddRange(drag.draggedNodes);
            } else {
                if (x.clickCount == 1)
                    if (x.shiftKey) {
                        if (!selectedNodes.Contains(target)) {
                            selectedNodes.Add(target);
                        } else {
                            selectedNodes.Remove(target);
                        }
                    } else {
                        selectedNodes.Clear();
                        selectedNodes.Add(target);
                    }
            }

            if (selectedNodes.Count == 1)
                editor.nodeInspector.Show(nodes[selectedNodes[0]]);
            else
                editor.nodeInspector.Show(null);

            foreach (var a in selectedNodes) {
                a.AddToClassList("selected");
            }
            editor.transitions.ClearSelection();
        }


        public void ClearSelection() {

            foreach (var ve in selectedNodes)
                ve.RemoveFromClassList("selected");
            selectedNodes.Clear();
            editor.nodeInspector.Clear();

        }


        void Redraw() {
            Init(editor.depthPanel.ActiveTree);
        }

    }
}
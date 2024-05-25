using UnityEditor;

namespace StateMachineFramework.Editor {
    public class Unredo {
        StateMachineEditor editor;
        public Unredo(StateMachineEditor editor) {
            Undo.undoRedoPerformed += Redraw;
            this.editor = editor;
        }
        public void Redraw() { 
            editor.nodeView.Init(editor.depthPanel.ActiveTree);
            editor.transitions.Redraw();
            editor.transitionInspector.Redraw();
            editor.nodeInspector.Redraw();

        }

        public void Dispose() {

            Undo.undoRedoPerformed -= Redraw;
        }
    }
}
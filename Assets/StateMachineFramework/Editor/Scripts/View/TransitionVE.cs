using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace StateMachineFramework.View {
    public class TransitionVE : VisualElement {

        public const string TRANSITION = "transition-anchor";
        public const string TRANSITION_ACTION = "transition-action";
        public const string TRANSITION_LINE = "transition-line";
        public const string TRANSITION_ARROW = "transition-arrow";

        public const string TRANSITION_ERROR = "transition-error";


        public class TransitionFactory : UxmlFactory<TransitionVE, TransitionTraits> { }
        public class TransitionTraits : UxmlTraits {
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var target = ve.Q<TransitionVE>();
            }
        }

        VisualElement line;
        VisualElement arrow;
        VisualElement actionVE;

        public Action<MouseDownEvent, TransitionVE> OnSelected;
        public TransitionVE() {
            this.usageHints = UsageHints.DynamicTransform;
            SetupContainters();
        }

        private void SetupContainters() {
            this.AddToClassList(TRANSITION);
            actionVE = VEHelper.Make<VisualElement>(this, "action", TRANSITION_ACTION);
            line = VEHelper.Make<VisualElement>(actionVE, "Line", TRANSITION_LINE);
            arrow = VEHelper.Make<VisualElement>(line, "Arrow", TRANSITION_ARROW);

            arrow.pickingMode = PickingMode.Ignore;
            line.pickingMode = PickingMode.Ignore;
            actionVE.RegisterCallback<MouseDownEvent>((x) => {
                OnSelected?.Invoke(x, this);
            });
        }

        public void Init(Vector2 source, Vector2 target) {
            Vector2 dir = target - source;
            this.transform.position = source;
            this.style.rotate = new Rotate(Vector2.SignedAngle(Vector2.up, -dir));
            actionVE.style.height = dir.magnitude;
        }
        VisualElement from, to;

        public void Init(VisualElement from, VisualElement to, VisualElement container) {
            this.from = from;
            this.to = to;

            Init(from.localBound.center, to.localBound.center);
            container.RegisterCallback<GeometryChangedEvent>(Update);
            from.RegisterCallback<GeometryChangedEvent>(Update);
            to.RegisterCallback<GeometryChangedEvent>(Update);
        }

        public void Update(GeometryChangedEvent evt = null) {
            Init(from.localBound.center, to.localBound.center);
        }

        public void AddTransitionCount() {
            VEHelper.Make<VisualElement>(line, "AdditionalArrow", TRANSITION_ARROW);
        }

    }

}
using NUnit.Framework;
using StateMachineFramework.Runtime;
using System.Collections.Generic;

public class ParameterTransitionTests {
    [Test]
    public void TriggerAnyStateTransition() {

        var sm = GetLogic(out var entryHistory, out var exitHistory, out var param);

        MakeTransition(0, 2);
        sm.CheckTransitions();
        (param[0] as TriggerParameter).Value = true;

        Assert.AreEqual(nodes[3], entryHistory[^1]);
        Assert.AreEqual(nodes[2], exitHistory[^1]);
    }


    List<Node> nodes = new() {
            new TreeNode() {name ="Root" },
            new SpecialNode() {name = "Any State" },
            new TreeNode() {name = "A" },
            new TreeNode() {name = "B"},
            new Node() {name = "Aa"},
            new Node() {name = "Ab"},
            new Node() {name = "Ba"},
            new TreeNode() {name = "Bb"},
            new Node() {name = "Bba"},
        };

    StateMachineLogic GetLogic(out List<Node> enterHistory, out List<Node> exitHistory, out List<IParameter> param) {

        (nodes[0] as TreeNode).nodes = new List<Node>() { nodes[3], nodes[2] };
        (nodes[2] as TreeNode).nodes = new List<Node>() { nodes[5], nodes[4] };
        (nodes[3] as TreeNode).nodes = new List<Node>() { nodes[7], nodes[6] };
        (nodes[7] as TreeNode).nodes = new List<Node>() { nodes[8] };

        foreach (var a in nodes) {
            if (a is TreeNode t)
                foreach (var n in t.nodes) {
                    n.parent = t;
                }
        }

        param = new List<IParameter>();
        param.Add(new TriggerParameter("Trigger", false));
        param.Add(new BoolParameter("Boolean", false));
        param.Add(new FloatParameter("Float", 0));
        param.Add(new IntParameter("Int", 0));

        nodes[1].transitions.Add(new Transition() {
            source = nodes[1],
            target = nodes[3],
            conditions = new List<TransitionCondition>() {
                new TransitionCondition() { parameter = param[0], equation = new TriggerEquation() }
            }
        });

        var sm = new StateMachineLogic(nodes, param);

        List<Node> nodesExited = new();
        List<Node> nodesEntered = new();


        sm.OnNodeExit += (node) => {
            nodesExited.Add(node);
        };
        sm.OnNodeEnter += (node) => {
            nodesEntered.Add(node);
        };
        enterHistory = nodesEntered;
        exitHistory = nodesExited;

        return sm;
    }

    void MakeTransition(int a, int b) {
        nodes[a].transitions.Add(new Transition() { source = nodes[a], target = nodes[b] });
    }
}

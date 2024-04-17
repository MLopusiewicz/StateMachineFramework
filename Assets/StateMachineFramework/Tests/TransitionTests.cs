using NUnit.Framework;
using StateMachineFramework.Runtime;
using System.Collections.Generic;
using UnityEngine.Experimental.AI;

public class TransitionTests {

    [Test]
    public void MoveFromAny() {
        StateMachineLogic sm = GetLogic(out var nodesEntered, out var nodesExited);

        sm.Move(GetTransition(0, 1));
        sm.Move(new Transition() { target = nodes[5] });


        Assert.AreEqual(nodesExited[^1], sm.nodes[1]);
        Assert.AreEqual(nodesEntered[^2], sm.nodes[2]);
        Assert.AreEqual(nodesEntered[^1], sm.nodes[5]);
    }

    [Test]
    public void MoveHorizontal() {
        StateMachineLogic sm = GetLogic(out var nodesEntered, out var nodesExited);

        sm.Move(GetTransition(0, 1));
        sm.Move(GetTransition(1, 2));

        Assert.AreEqual(nodesExited[0], sm.nodes[1]);
        Assert.AreEqual(nodesEntered[1], sm.nodes[2]);
    }

    [Test]
    public void MoveUp() {
        StateMachineLogic sm = GetLogic(out var nodesEntered, out var nodesExited);

        sm.Move(GetTransition(0, 7));
        sm.Move(GetTransition(7, 2));

        Assert.AreEqual(nodesExited[^3], sm.nodes[7]);
        Assert.AreEqual(nodesExited[^2], sm.nodes[6]);
        Assert.AreEqual(nodesExited[^1], sm.nodes[2]);
        Assert.AreEqual(nodesEntered[^1], sm.nodes[2]);

    }
    [Test]
    public void Reentering() {
        StateMachineLogic sm = GetLogic(out var nodesEntered, out var nodesExited);

        sm.Move(GetTransition(0, 1));
        sm.Move(new Transition() { target = nodes[1] });

        Assert.AreEqual(nodesExited[^1], sm.nodes[1]);
        Assert.AreEqual(nodesEntered[^1], sm.nodes[1]);

    }

    [Test]
    public void MoveDown() {
        StateMachineLogic sm = GetLogic(out var nodesEntered, out var nodesExited);

        sm.Move(GetTransition(0, 3));

        Assert.AreEqual(nodesEntered[0], sm.nodes[1]);
        Assert.AreEqual(nodesEntered[1], sm.nodes[3]);
    }
    [Test]
    public void MoveDiagonalUp() {
        StateMachineLogic sm = GetLogic(out var nodesEntered, out var nodesExited);

        sm.Move(GetTransition(0, 3));
        sm.Move(GetTransition(3, 2));

        Assert.AreEqual(nodesExited[^2], sm.nodes[3]);
        Assert.AreEqual(nodesExited[^1], sm.nodes[1]);
        Assert.AreEqual(nodesEntered[^1], sm.nodes[2]);
    }
    [Test]
    public void MoveDiagonalDown() {
        StateMachineLogic sm = GetLogic(out var nodesEntered, out var nodesExited);

        sm.Move(GetTransition(0, 1));
        sm.Move(GetTransition(1, 5));

        Assert.AreEqual(nodesExited[^1], sm.nodes[1]);
        Assert.AreEqual(nodesEntered[^2], sm.nodes[2]);
        Assert.AreEqual(nodesEntered[^1], sm.nodes[5]);
    }



    List<Node> nodes = new() {
            new TreeNode() {name ="Root" },
            new TreeNode() {name = "A" },
            new TreeNode() {name = "B"},
            new Node() {name = "Aa"},
            new Node() {name = "Ab"},
            new Node() {name = "Ba"},
            new TreeNode() {name = "Bb"},
            new Node() {name = "Bba"},
        };
    StateMachineLogic GetLogic(out List<Node> enterHistory, out List<Node> exitHistory) {

        (nodes[0] as TreeNode).nodes = new List<Node>() { nodes[1], nodes[2] };
        (nodes[1] as TreeNode).nodes = new List<Node>() { nodes[3], nodes[4] };
        (nodes[2] as TreeNode).nodes = new List<Node>() { nodes[5], nodes[6] };
        (nodes[6] as TreeNode).nodes = new List<Node>() { nodes[7] };

        nodes[1].parent = nodes[0];
        nodes[2].parent = nodes[0];
        nodes[3].parent = nodes[1];
        nodes[4].parent = nodes[1];
        nodes[5].parent = nodes[2];
        nodes[6].parent = nodes[2];
        nodes[7].parent = nodes[6];

        var sm = new StateMachineLogic(nodes, new List<IParameter>());

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
    Transition GetTransition(int a, int b) {
        return new Transition() { source = nodes[a], target = nodes[b] };
    }
}
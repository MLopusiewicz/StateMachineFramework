![Logo](images/LogoWithText.png)
## Goal
Framework was developed to speed up creation of state machines. The solution is designed   to work with code-based state machines by adding visual layer for transitions and states.  
With State behaviours and tree structure it allows for complex state-machines.

## Key features
- Visual setup 
- Runtime visual feedback
- Holds scene-references
- Tree structure with sub states for nested state behaviours
- Runtime-editor parameter set (for testing purposes)
- List of state behaviours per state (for state reusing)
- Light and basic but easily extendable 
- No bloat

## Known Limitations
- No transitions between layers 
- no selection box (UX) 
## Working with framework

Editor window is designed to resemble unity-animator as much as possible.  Create own stateBehaviours by inheriting from StateBehaviour.cs. 
StateBehaviour implements Awake, Update, Exit, Enter:
- **Awake** - called once in awake (order of nodes not enforced). 
- **Update** - called on all active nodes  every frame (from parent to child).
- **Enter** - called when transitioning into state.
- **Exit** - called when transitioning out of state. 
 **BehaviourState** execution order is from top to bottom.
### Transitions
As transitions have no "transition time" whenever the parameter is changed transitions from current nodes, as well as AnyNode transitions, are evaluated. In case transition evaluates as true, the current state is exited and new state enters on same frame. 
#### Endless loops
Be careful of endless loops. It's possible to create a circular path of transitions, which will lead to multiple entering/exiting nodes in the loop in one frame. To prevent this, it's only possible to go through 20 transitions per frame.

#### Tree
Enter - Substate transitions are only evaluated whenever the TreeNode is Entered.  Meaning, if the active state is Node_1 and transition from Enter-Node_2 evaluates to true, it doesn't move to Node_2.
 

![[TreeEntryTransition.png]]
Transitioning to exit will re-enter the parent tree,   evaluating the enter transitions as well as exiting&entering parent behaviours.
#### Priority 
As there are possible active substates and trees at the same time. The priority runs from the root to the nested nodes. Meaning: If both parent and child have valid transitions, parent transition will take precedence.

## FAQ:
- Why not make transition condition limited to hardcoded float/int/bool/trigger parameters? 
	- So you can test it out by changing parameters in the editor during runtime.


## Report a bug
to report a bug  create new issue:
https://github.com/MLopusiewicz/StateMachineFramework/issues



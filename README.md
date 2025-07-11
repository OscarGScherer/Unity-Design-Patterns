# Unity Design Patters
Generic implementations of commonly used design patterns.

**How to use**
* [Singleton](#singleton)
* [State Machine](#state-machine)
* [Behaviour Tree](#behaviour-tree)
* [Event Bus](#event-bus)

**Other**
* [How to install this package](#how-to-install-this-package)

# Singleton
To create a singleton, simply inherit from ```Singleton```. Note that if you want to use OnEnable and OnDisable, you must override the existing methods and call the base implementation for both, since those are used to keep track of each Singleton type.
```c#
public class ExampleSingleton : Singleton
{
    // Your custom functions and code here...

    // Optionally override OnEnable
    protected override void OnEnable()
    {
        base.OnEnable(); // Make sure you call the base method
        // Your custom OnEnable code here...
    }

    // Optionally override OnDisable
    protected override void OnDisable()
    {
        base.OnDisable(); // Make sure you call the base method
        // Your custom OnDisable code here...
    }
}
```
Below is how you would get your singleton from somewhere else in the code. If the singleton doesn't exist, an empty game object will be created and a new instance of the singleton will be added as a component to it.
```c#
ExampleSingleton exampleSingleton = Singleton.Get<ExampleSingleton>();
```

# State Machine

> ```namespace DesignPatterns.StateMachine```

To create a state machine you need to inherit from the ```StateMachine``` class. Then you need to call ```SetStartingState<T>``` once to set it up, then call ```UpdateStateMachine``` to update it.
```c#
public class StateMachineExample : StateMachine
{
    void Start()
    {
        // Setting the starting to be this ExampleState
        SetStartingState<ExampleState>();
    }
    void Update()
    {
        // Udpating the SM every frame using deltaTime
        UpdateStateMachine(Time.deltaTime);
    }
}
```
To create states you need to inherit from the ```State``` class. You need to implement ```Initialize``` and ```Update```, and also may override ```OnEnter``` and ```OnExit```.
Inside ```Initialize``` you should get all the necessary components for your state to work, it is called once for every state, when the state machine enters it for the first time.
In order to define a transition to another state, inside ```Udpate``` you can return an instance of ```StateType<T>```, which is just an empty class holding a reference to some 
type that inherits from ```State```, the type defines which state the state machine should transition to, noting that each state machine is limited to having only one instance of a 
state per type (for instance, this state machine will only have a single instance of an ExampleState).
```c#
public class ExampleState : State
{
    public override void Initialize(GameObject gameObject)
    {
        // Get all necessary references for the state to work...
    }
    public override void OnEnter()
    {
        // Custom logic here...
    }
    public override StateType Update(float deltaTime)
    {
        // Custom logic here...
        // Return the state we want to transition to, through a StateType
        return new StateType<ExampleState>();
    }
    public override void OnExit()
    {
        // Custom logic here...
    }
}
```
The custom inspector lets you check the variables and properties of the current state through reflection.
You can also override ```ToDebugString()``` inside state to print some custom debug info.

![image](https://github.com/user-attachments/assets/48155fe0-b2e1-465b-bc38-5c1cce55785f)


# Behaviour Tree

To create a behaviour tree, you need to inherit from ```BehaviourTree```. And to make a node, you inherit from ```BehaviourTree.Node```.
In your behaviour tree, you have to implement ```BuildTree()```, and make sure to call ```base.Start()```. You have ```Sequencer``` and ```Selector``` types
to build your tree. Sequencer's execute all its children and returns ```Node.State.SUCCESS``` when all of them succeed, and returns early with ```Node.State.FAILURE``` or ```Node.State.IN_PROGRESS```,
as soon as one of its children returns one of those states. Selectors returns ```Node.State.Success``` or ```Node.State.IN_PROGRESS```, as soon as any of its children return on of these
states, and return ```Node.State.FAILURE``` if all of its children fail.
```c#
public class ExampleBehaviourTree : BehaviourTree
{
    // Necessary function to set up the tree
    protected override Node BuildTree()
    {
        Sequencer rootSequencer = new Sequencer("Root");
        // Adding custom nodes, passing along the necessary references
        rootSequencer.Attatch(new ExampleNode("Example node", GetComponent<Component1>(), GetComponent<Component2>()));
        Selector selector1 = (Selector) rootSequencer.Attatch(new Selector("Selector 1"));
        // Build the rest of your tree like that...
        // Return the root
        return rootSequencer;
    }

    // Make sure you call InitializeTree() before you execute the tree
    void Start() => InitializeTree();

    // Executing the tree every frame
    void Update() => EvaluateTree(Time.deltaTime);
}
```
For nodes you need only to implement ```EvaluateProccess(float deltaTime)```, which needs to return ```State.SUCCESS```, ```State.FAILURE```, ```State.IN_PROGRESS```.
```c#
public class ExampleNode : BehaviourTree.Node
{
    public ExampleNode(string name, Component1 component1, Component2 component2) : base(name)
    {
        // Getting all the necessary references for the node to work...
    }
    protected override State EvaluateProccess(float deltaTime)
    {
        // Custom execution code here...
        // Then return SUCCESS, FAILURE, or IN_PROGRESS
        return State.SUCCESS;
    }
}
```
The custom inspector lets you visualize the current execution state of the behaviour tree, and select
specific nodes to inpect them. You can also override ```ToDebugString()``` on any of your nodes to print some custom debug info.

![image](https://github.com/user-attachments/assets/1cd0c8e8-e0ac-4047-9014-22285d682ab7)


## Event Bus

Lets any class in your project register or raise events, it serves as a middleman between system's you don't want to couple directly.
To use it, you just need to call ```EventBus<T>.RaiseEvent```, ```EventBus<T>.RegisterToEvent```, and ```EventBus<T>.UnregisterFromEvent```.

```c#
public class ClassA : MonoBehaviour
{
    public enum EventNames { EventA, EventB, }

    void Start()
    {
        EventBus<string>.RegisterToEvent(EventNames.EventA, ExampleFunction);
    }

    void ExampleFunction(string message) => Debug.Log(message);
}

public class ClassB : MonoBehaviour
{
    void SomePreccess()
    {
        EventBus<string>.RaiseEvent(EventNames.EventA, "Raised event!");
    }
}
```

## How to install this package
1. Open package manager
2. Click on '+', on the top left corner
3. Select Install package from git URL
4. Paste this link: https://github.com/OscarGScherer/Unity-Design-Patterns.git
  
For a more detaield explanation, check this documentation page:
* https://docs.unity3d.com/6000.0/Documentation/Manual/upm-ui-giturl.html

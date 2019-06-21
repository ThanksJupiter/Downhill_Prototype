/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMController : MonoBehaviour
{
    private FSM stateMachine;
    private FSM.FSMState idleState;
    private FSM.FSMState moveToState;
    private FSM.FSMState performActionState;

    [SerializeField]
    private int states;

    // Use this for initialization
    void Start()
    {
        stateMachine = new FSM();
        createIdleState();
        createMoveToState();
        createPerformActionState();
        stateMachine.pushState(idleState);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update(this.gameObject);
        states = stateMachine.stateStack.Count;
    }

    private void createIdleState()
    {
        idleState = (fsm, obj) =>
        {
            HashSet<KeyValuePair<string, object>> worldState = dataProvider.getWorldState();
            HashSet<KeyValuePair<string, object>> goal = dataProvider.createGoalState();

            Queue<GOAPAction> plan = planner.plan(gameObject, availableActions, worldState, goal);
            if (plan != null)
            {
                currentActions = plan;
                dataProvider.planFound(goal, plan);

                fsm.popState();
                fsm.pushState(performActionState);
            }
            else
            {
                dataProvider.planFailed(goal);
                fsm.popState();
                fsm.pushState(idleState);
            }
        };
    }

    private void createMoveToState()
    {
        moveToState = (fsm, gameObject) =>
        {
            GOAPAction action = currentActions.Peek();
            if (action.requiresInRange() && action.target == null)
            {
                fsm.popState();
                fsm.popState();
                fsm.pushState(idleState);
                return;
            }

            bool enBool = dataProvider.moveAgent(action);
            if (enBool)
            {
                fsm.popState();
            }

        };
    }

    private void createPerformActionState()
    {

        performActionState = (fsm, obj) =>
        {
            if (!hasActionPlan())
            {
                fsm.popState();
                fsm.pushState(idleState);
                dataProvider.actionsFinished();
                return;
            }

            actionList.Clear();
            GOAPAction action = currentActions.Peek();

            foreach (GOAPAction a in currentActions)
            {
                actionList.Add(a);
            }

            actions = currentActions.Count;
            if (action.isDone())
            {
                currentActions.Dequeue();
            }

            if (hasActionPlan())
            {
                action = currentActions.Peek();
                bool inRange = action.requiresInRange() ? action.isInRange() : true;

                if (inRange)
                {
                    bool success = action.perform(obj);
                    Debug.Log("Current performing action: " + action + " successful? " + success);
                    if (success)
                    {

                        // Hack, can only have one precondition
                        KeyValuePair<string, object> kvp = default(KeyValuePair<string, object>);

                        if (action.Preconditions.Count != 0)
                        {
                            NonPlayerCharacter npc = obj.GetComponent<NonPlayerCharacter>();

                            foreach (KeyValuePair<string, object> keyVal in action.Preconditions)
                            {
                                kvp = keyVal;
                            }

                            npc.worldState.Add(kvp);
                        }

                    }
                    else
                    {
                        fsm.popState();
                        fsm.pushState(idleState);
                        createIdleState();
                        dataProvider.planAborted(action);
                    }
                }
                else
                {
                    fsm.pushState(moveToState);
                }
            }
            else
            {
                fsm.popState();
                fsm.pushState(idleState);
                dataProvider.actionsFinished();
            }
        };
    }
}
*/

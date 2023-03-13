using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTriggerBehaviour : StateMachineBehaviour
{
    public string triggerName;
    public bool updateOnState;
    public bool updateOnStateMachine;
    public bool changeOnEnter, changeOnExit;    //判断是否要修改
    public bool stateOnEnter, stateOnExit;      //判断修改的值为set还是reset
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (updateOnState && changeOnEnter)
        {
            if (stateOnEnter)
            {
                animator.SetTrigger(triggerName);
            }
            else
            {
                animator.ResetTrigger(triggerName);
            }
        }
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (updateOnState && changeOnExit)
        {
            if (stateOnExit)
            {
                animator.SetTrigger(triggerName);
            }
            else
            {
                animator.ResetTrigger(triggerName);
            }
        }
    }

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (updateOnStateMachine && changeOnEnter)
        {
            if (stateOnEnter)
            {
                animator.SetTrigger(triggerName);
            }
            else
            {
                animator.ResetTrigger(triggerName);
            }
        }
    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (updateOnStateMachine && changeOnExit)
        {
            if (stateOnExit)
            {
                animator.SetTrigger(triggerName);
            }
            else
            {
                animator.ResetTrigger(triggerName);
            }
        }
    }
}

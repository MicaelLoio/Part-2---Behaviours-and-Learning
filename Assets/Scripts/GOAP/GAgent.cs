﻿using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class SubGoal
{

    // Dictionary to store our goals
    public Dictionary<string, int> sgoals;
    // Bool to store if goal should be removed
    public bool remove;

    // Constructor
    public SubGoal(string s, int i, bool r) 
    {

        sgoals = new Dictionary<string, int>();
        sgoals.Add(s, i);
        remove = r;
    }
}

public class GAgent : MonoBehaviour 
{ 


    public List<GAction> actions = new List<GAction>();
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();
    public Ginventory inventory = new Ginventory();
    public WorldStates beliefs = new WorldStates();

    GPlanner planner;
    Queue<GAction> actionQueue;
    public GAction currentAction;
    SubGoal currentGoal;

    // Start is called before the first frame update
    protected virtual void Start() 
    {

        GAction[] acts = this.GetComponents<GAction>();
        foreach (GAction a in acts) 
        {

            actions.Add(a);
       
        }

	}
	    bool invoked = false;
        public void CompleteAction()
        {
             currentAction.running = false;
             currentAction.PostPerform();
             invoked = false;
         }

    void LateUpdate() 
    {
        if(currentAction != null && currentAction.running )
        {
                float distanceToTarget = Vector3.Distance(currentAction.target.transform.position, this.transform.position);
                if(currentAction.agent.hasPath && distanceToTarget < 1.0f) //currentAction.agent.remainingDistance < 0.5f)
				{
                    Debug.Log("Distance to Goal: " + currentAction.agent.remainingDistance);
                if (!invoked)
                {
                    //Debug.Log("Complete Invoke in: " + currentAction.duration);
                    Invoke("CompleteAction", currentAction.duration);
                    invoked = true;
                }
            }

            return;
        }

        if(planner == null || actionQueue == null)
        {
            planner = new GPlanner();

            var sortedGoals = from entry in goals orderby entry.Value descending select entry;

            foreach( KeyValuePair < SubGoal, int> sg in sortedGoals )
            {
                actionQueue = planner.plan(actions, sg.Key.sgoals, beliefs);
                if(actionQueue != null ) 
                
                {
                    currentGoal = sg.Key;
                    break;
                }
            }

        }

        if(actionQueue != null && actionQueue.Count == 0)
        {
            if(currentGoal.remove)
            {
                goals.Remove(currentGoal);
            }

            planner = null;
        }


            if (actionQueue !=null && actionQueue.Count > 0)
            {
                     currentAction = actionQueue.Dequeue();
                        if(currentAction.PrePerform())
                         {
                            if(currentAction.target == null && currentAction.targetTag != "")
                                currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                          {
                            if(currentAction.target !=null)
                             {
                                 currentAction.running = true;
                                currentAction.agent.SetDestination(currentAction.target.transform.position);
                             }

                          }

                            
                         }
                        else
                          {
                    actionQueue = null;
                          }

            }
		
	}
}



using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public delegate void Notification();
    public static event Notification NewTurn;

    public delegate void System();

    public static event System Ready;

    public float TimeDelayBetweenTurns = 2f;

    public GameObject[] allEntities;
    private Queue<Entity> _turnOrder = new Queue<Entity>();
    private bool isPlaying;
    public int TurnCount { get; set; }


	// Use this for initialization
	void Awake ()
	{



	}

    void Start()
    {
        FindAllFighters();
        if (allEntities != null)
            SortBySpeed();
        if (Ready != null)
            Ready();
    }

    void OnEnable()
    {
        TurnCount = 1;
        isPlaying = true;
        Entity.EndedTurn += NextTurn;
        SelectorManager.Ready += StartTurn;
        //Entity.Dead += RemoveDead;
        // Player.Dead += StopPlaying;
    }

    void OnDisable()
    {
        Entity.EndedTurn -= NextTurn;
        SelectorManager.Ready -= StartTurn;
       // Entity.Dead -= RemoveDead;
       // Player.Dead -= StopPlaying;

    }


    void StartTurn()
    {
        CurrentTurn().IsTurn = true;
        if (NewTurn != null)
            NewTurn();
        
    }

	// Update is called once per frame
	void Update () {
	
	}

    bool RemoveDead()
    {
        if (!CurrentTurn().IsDead) 
            return true;
        _turnOrder.Dequeue();
        return false;
    }

    void NextTurn()
    {
        StartCoroutine(TurnDelay(TimeDelayBetweenTurns));
    }

    
    void SortBySpeed()
    {    
        _turnOrder.Clear();
        var sortedArray = allEntities.OrderByDescending(go => CompareCondition(go)).ToArray();
        foreach (var obj in sortedArray)
        {
            
            var fighter = obj.GetComponent<Entity>();
            Debug.Log(fighter.Name);
            _turnOrder.Enqueue(fighter);
        }
    }

    void FindAllFighters()
    {
        allEntities = GameObject.FindGameObjectsWithTag("Fighter");

    }

    public Entity CurrentTurn()
    {
        return _turnOrder.Peek();
    }

#region HelperFunctions
    static float CompareCondition(GameObject go)
    {
        float speed = 0;
        if(go.GetComponent<Entity>() != null)
            speed = go.GetComponent<Entity>().Speed;

        return speed;
    }

    IEnumerator TurnDelay(float val)
    {
        yield return new WaitForSeconds(val);
        _turnOrder.Enqueue(_turnOrder.Dequeue());
        while (!RemoveDead()) {}
        
        StartTurn();
    }
#endregion
}

/*
 * Collect all the Entities in the battle. and sort them by speed.
 * 
 */
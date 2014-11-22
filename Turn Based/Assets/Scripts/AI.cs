using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour
{
    public float attackDelay = 10f;
    public float speed = 10f;
    public List<Entity> enemyList = new List<Entity>();

    private Animator _animator;
    private BattleManager _battleManager;
    private Warrior _self;

    public virtual void OnEnable()
    {
        BattleManager.NewTurn += OnTurn;
        Entity.Dead += RemoveDead;

    }

    public virtual void OnDisable()
    {
        BattleManager.NewTurn -= OnTurn;
        Entity.Dead -= RemoveDead;
    }

    public virtual void OnTurn()
    {
        if (_self.IsTurn && enemyList.Count > 0)
        {
            StartCoroutine("AttackRandomTarget");
        }

    }

    IEnumerator AttackRandomTarget()
    {     
        yield return new WaitForSeconds(attackDelay);
        int ranIndex = Random.Range(0, enemyList.Count);
        enemyList[ranIndex].Hit(_self.damage);
        Debug.Log(_self.name + " has attacked " + enemyList[ranIndex].Name + " for " + _self.damage + " damage!");

        Vector3 pointA = transform.position;
        Vector3 pointB = enemyList[ranIndex].transform.position;
        bool reachedTarget = false;
        _animator.Play("attack");
        while (!reachedTarget) {
            yield return StartCoroutine(MoveObject(transform, pointA, pointB, 3.0f));
            yield return StartCoroutine(MoveObject(transform, pointB, pointA, 3.0f));
            reachedTarget = true;
        }
        _animator.Play("idle");
        yield return new WaitForSeconds(.5f);
        _self.EndTurn();

    }
    //http://answers.unity3d.com/questions/14279/make-an-object-move-from-point-a-to-point-b-then-b.html
    IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time)
    {
        float i = 0.0f;
        float rate = speed / time;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
            yield return null;
        }
    }
    void Awake()
    {
        _battleManager = GameObject.FindGameObjectWithTag("Battle Manager").GetComponent<BattleManager>();
        _self = GetComponent<Warrior>();
        _animator = GetComponent<Animator>();
    }
	// Use this for initialization
	void Start () {
	    foreach (var obj in _battleManager.allEntities)
	    {
            //Add to enemy list if entity is not on the player's team.
            var en = obj.GetComponent<Entity>();
            if (en.Team != _self.team)
                enemyList.Add(en);
	    }
	
	}

    void RemoveDead()
    {
        Queue<int> toRemove = new Queue<int>();
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].IsDead)
                toRemove.Enqueue(i);
        }
        foreach (var i in toRemove)
        {
            enemyList.RemoveAt(i);
        }
    }
	
	// Update is called once per frame
	void Update () {

	
	}
}

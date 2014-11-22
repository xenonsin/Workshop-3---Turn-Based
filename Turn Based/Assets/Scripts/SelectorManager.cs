
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BattleManager))]
public class SelectorManager : MonoBehaviour
{
    public GameObject attackArrow;
    public GameObject allyArrow;
    public int playerTeam = 1;


    private BattleManager _battleManager;
    private bool _playerTurn = false;
    private Entity _target;
    private int _index = 0;
    private Entity _turn;

    public List<Entity> enemyList = new List<Entity>();

    void OnEnable()
    {
        BattleManager.NewTurn += OnTurn;
        Entity.Dead += RemoveDead;

    }

    void OnDisable()
    {
        BattleManager.NewTurn -= OnTurn;
        Entity.Dead -= RemoveDead;
    }

	// Use this for initialization
	void Awake ()
	{
	    _battleManager = GetComponent<BattleManager>();

	    
	}

    void Start()
    {
        InstantiateSelectorArrows();
    }

    void InstantiateSelectorArrows()
    {
        foreach (var obj in _battleManager.allEntities)
        {
            Vector3 offset = obj.transform.position;
            offset.y += 1f;
            
            GameObject go = Instantiate(attackArrow, offset, Quaternion.identity) as GameObject;
            go.transform.parent = obj.transform;
            go.SetActive(false);

            GameObject gob = Instantiate(allyArrow, offset, Quaternion.identity) as GameObject;
            gob.transform.parent = obj.transform;
            gob.SetActive(false);

            //Add to enemy list if entity is not on the player's team.
            var en = obj.GetComponent<Entity>();   
            if (en.Team != playerTeam)
                enemyList.Add(en);
        }
    }

	
	// Update is called once per frame
	void Update () {
	    if (_playerTurn)
	    {
	        if (Input.GetKeyDown(KeyCode.LeftArrow))
	        {
	            ChangeTarget(-1);    
	        }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangeTarget(1);
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(AttackTarget());
            }
	    }

	
	}

    IEnumerator AttackTarget()
    {
        _target.Hit(_turn.Damage);
        Debug.Log(_turn.name + " has attacked " + _target.Name + " for " + _turn.Damage + " damage!");

        Vector3 pointA = _turn.transform.position;
        Vector3 pointB = _target.transform.position;
        bool reachedTarget = false;

        while (!reachedTarget)
        {
            yield return StartCoroutine(MoveObject(_turn.transform, pointA, pointB, 3.0f));
            yield return StartCoroutine(MoveObject(_turn.transform, pointB, pointA, 3.0f));
            reachedTarget = true;
        }

        yield return new WaitForSeconds(1f);
        _turn.EndTurn();
    }

    //http://answers.unity3d.com/questions/14279/make-an-object-move-from-point-a-to-point-b-then-b.html
    IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time)
    {
        float i = 0.0f;
        float rate = 10f / time;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
            yield return null;
        }
    }

    void ChangeTarget(int val)
    {
        _index += val;
        if (_index < 0)
            _index = 0;
        else if (_index >= enemyList.Count)
            _index = enemyList.Count - 1 ;
        SetArrow(false);
        _target = enemyList[_index];
        SetArrow(true);
    }

    void SetArrow(bool val)
    {
        var targetArrow = getChildGameObject(_target.gameObject, "attackArrow(Clone)");
        targetArrow.SetActive(val);
    }

    void SetAllyArrow(bool val)
    {
        var targetArrow = getChildGameObject(_turn.gameObject, "allyarrow(Clone)");
        targetArrow.SetActive(val);
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
    void OnTurn()
    {
       _playerTurn = (_battleManager.CurrentTurn().Team == playerTeam);

        if (_turn != null)
        {
            SetAllyArrow(false);
            _turn = _battleManager.CurrentTurn();
            SetAllyArrow(true);
        }
        else
        {
            _turn = _battleManager.CurrentTurn();
            SetAllyArrow(true);
        }



        if (_playerTurn)
            ActivateArrow();
        else if (!_playerTurn && _target != null)
        {
            SetArrow(false);
        }

    }

    void ActivateArrow()
    {
        
        _target = enemyList[0];
        SetArrow(true);
    }

    static public GameObject getChildGameObject(GameObject fromGameObject, string withName)
    {
        //Author: Isaac Dart, June-13.
        Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
        return null;
    }
}

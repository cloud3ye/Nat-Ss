using System;
using System.Collections.Generic;
using Pathfinding;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] public float HealthDepletionRate = 1f;
    [SerializeField] private float HealthChargeRate = 1f;
    public Image Healthbar;
    public float currentHealth;
    public float HealthPercent => currentHealth / maxHealth;

    [Header("Hunger")]
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] public float hungerDepletionRate = .75f;
    public Image Hungerbar;
    public float currentHunger;
    public float HungerPercent => currentHunger / maxHunger;

    
    [Header("Thirst")]
    [SerializeField] private float maxThirst = 100f;
    [SerializeField] public float thirstDepletionRate = 1f;
    public Image Thirstbar;
    public float currentThirst;
    public float ThirstPercent => currentThirst / maxThirst;

    [Header("Reproductive Urge")]
    [SerializeField] private float maxUrge = 100f;
    [SerializeField] private float urgeChargeRate = 1f;
    [SerializeField] private float urgeChargeDelay = 1f;
    public float currentUrge;
    private float currentUrgeDelayCounter;
    public float UrgePercent => currentUrge / maxUrge;

    [Header("Sensory")]
    public bool treeInRange = false;
    public Vector3 treepos;
    public TreeManager tree;
    public bool waterInRange = false;
    public Vector3 waterpos;
    public GameObject creatureprefab;
    public bool creatureInRange = false;
    public Vector3 midPoint;
    public Movement creature2;
    public Reproduction reproduction;
    public bool isReproducing = false;
    public Collider[] targetsInFOV;
    private string status = "SS";
    private string newStatus;

    [Header("Movement")]
    GraphNode randomNode;
    GridGraph grid; 
    public float fov;
    public float height;
    public float moveSpeed;
    bool moving = false;
    float moveRaduis = 2f;
    private float stuckTime = 0f;
    private float timeThreshold = 2f;
    public bool waiting = false;
    public bool endOfPath;


    public TMP_Text tMP_Text;
    public IAstarAI ai;

    void Start()
    {
        Setting();
        reproduction = gameObject.AddComponent<Reproduction>();
        InitialiseStats();
        Visuals(gameObject);

        ai = GetComponent<IAstarAI>();
        grid = AstarPath.active.data.gridGraph;
        AstarPath.active.logPathResults = PathLog.None;

    }



    void Update()
    {
        targetsInFOV = Physics.OverlapSphere(transform.position,fov);
        StatsPerSec();
        HungerAndThirstCheck();
        DeathCheck();
        TreeCheck();
        WaterCheck();
        creatureCheck();
        MoveToEat(moving);
        MoveToDrink(moving);
        MoveToReproduce(moving);
        Wander(waiting);
        CheckEdgeCollision();
        UI();


    }
    private void Setting()
    {
        HealthDepletionRate = PlayerPrefs.GetFloat("HealthDepletionRate");
        hungerDepletionRate = PlayerPrefs.GetFloat("HungerDepletionRate");
        thirstDepletionRate = PlayerPrefs.GetFloat("ThirstDepletionRate");
    }
    private void UI()
    {
        tMP_Text.text = tMP_Text.text.Replace("XX",fov.ToString());
        tMP_Text.text = tMP_Text.text.Replace("YY",height.ToString());
        tMP_Text.text = tMP_Text.text.Replace("ZZ",moveSpeed.ToString());
    }

    private void Visuals(GameObject gameObject)
    {
        Vector3 currentScale = gameObject.transform.localScale;
        Vector3 newPos = gameObject.transform.position;
        if(height>=15)
        {
            currentScale.x = 1.5f;
            currentScale.y = 1.5f;
            currentScale.z = 1.5f;
            gameObject.transform.localScale = currentScale; 
            Collider collider = gameObject.GetComponent<Collider>();
            if(collider != null)
            {
                collider.enabled = false;
                collider.enabled = true;
                gameObject.GetComponent<AIPath>().enabled = false;
                gameObject.GetComponent<AIPath>().enabled = true;
                newPos.y += 1.5f/2 ;
                gameObject.transform.position = newPos;
                
            }
        }
        else if(height>=10)
        {
            currentScale.x = 1f;
            currentScale.y = 1f;
            currentScale.z = 1f;
            gameObject.transform.localScale = currentScale; 
            Collider collider = gameObject.GetComponent<Collider>();
            if(collider != null)
            {
                collider.enabled = false;
                collider.enabled = true;
                gameObject.GetComponent<AIPath>().enabled = false;
                gameObject.GetComponent<AIPath>().enabled = true;
                newPos.y += 1f/2 ;
                gameObject.transform.position = newPos;
            }
        }
        else if(height>=5)
        {
            currentScale.x = 0.75f;
            currentScale.y = 0.75f;
            currentScale.z = 0.75f;
            gameObject.transform.localScale = currentScale; 
            Collider collider = gameObject.GetComponent<Collider>();
            if(collider != null)
            {
                collider.enabled = false;
                collider.enabled = true;
                gameObject.GetComponent<AIPath>().enabled = false;
                gameObject.GetComponent<AIPath>().enabled = true;
                newPos.y += .75f/2 ;
                gameObject.transform.position = newPos;
            }
        }
        else
        {
            currentScale.x = 0.4f;
            currentScale.y = 0.4f;
            currentScale.z = 0.4f;
            gameObject.transform.localScale = currentScale; 
            Collider collider = gameObject.GetComponent<Collider>();
            if(collider != null)
            {
                collider.enabled = false;
                collider.enabled = true;
                gameObject.GetComponent<AIPath>().enabled = false;
                gameObject.GetComponent<AIPath>().enabled = true;
                newPos.y += .4f/2 ;
                gameObject.transform.position = newPos;
            }
        }
    }

    private void InitialiseStats()
    {
        gameObject.GetComponent<AIPath>().maxSpeed = moveSpeed/2;   
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        currentUrge = 0;
    }

    private void StatsPerSec()
    {
        currentHunger -= hungerDepletionRate * Time.deltaTime;
        Hungerbar.fillAmount = HungerPercent;
        currentThirst -= thirstDepletionRate * Time.deltaTime;
        Thirstbar.fillAmount = ThirstPercent;
        currentUrge += urgeChargeRate * Time.deltaTime;
    }

    private void HungerAndThirstCheck()
    {
        if (currentHunger <= 0)
        {
            currentHunger = 0;

            currentHealth -= HealthDepletionRate * Time.deltaTime;
            Healthbar.fillAmount = HealthPercent;
        }
        if (currentThirst < 0)
        {
            currentThirst = 0;
            currentHealth -= HealthDepletionRate * Time.deltaTime;
            Healthbar.fillAmount = HealthPercent;
        }
    }

    private void DeathCheck()
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Death();
        }
    }

    void Death()
    {
        Destroy(gameObject);
    }

    private void TreeCheck()
    {
        if (ai.destination != treepos)
        {
            TreeFinder();
        }
    }

    void TreeFinder()
    {
        
        foreach (Collider c in targetsInFOV)
        {
            if (c.CompareTag("Tree"))
            {
                tree = c.GetComponent<TreeManager>();
                if(height >= tree.HeightReq && tree.currentfoodcount > 0)
                {
                    treeInRange = true;
                    treepos = (Vector3)c.transform.position;
                    break;
                }
                
            }
            else
            {
                treeInRange = false;
            }
        }
    }


    private void WaterCheck()
    {
        if (ai.destination != waterpos)
        {
            WaterFinder();
        }
    }

    void WaterFinder()
    {
        GraphNode node1 =AstarPath.active.GetNearest(transform.position).node;
        List<GraphNode> nodesInFov = new List<GraphNode>();
         foreach (GraphNode node in grid.nodes)
        {
            float distance = Vector3.Distance((Vector3)node.position, (Vector3)node1.position);
            if (distance < fov)
            {
                nodesInFov.Add(node);
            }
        }
        
        if (nodesInFov.Count > 0)
        {

            foreach (GraphNode fovNode in nodesInFov)
            {
                if (!PathUtilities.IsPathPossible(node1, fovNode) && !fovNode.Walkable)
                {
                    waterInRange = true;
                    waterpos = (Vector3)fovNode.position;
                    return;
                }
            }

            waterInRange = false;
            
        }
        else
        {
            waterInRange = false;
        }
    }

    private void creatureCheck()
    {
        endOfPath = ai.reachedEndOfPath;
        if(ai.destination != midPoint)
        {
            CreatureFinder();
        }
    }
    void CreatureFinder()
    {
        foreach (Collider c in targetsInFOV)
        {    
            if (c.CompareTag("Creature") && c.gameObject != gameObject)
            {
                creature2 = c.gameObject.GetComponent<Movement>();
                Vector3 creature2pos = c.transform.position;
                creature2.ai = creature2.gameObject.GetComponent<IAstarAI>();
                if(c.gameObject.GetComponent<Collider>() != null && Array.Exists(creature2.targetsInFOV, x => x.gameObject == gameObject) && Array.Exists(targetsInFOV, x => x.gameObject == c.gameObject))
                {
                    creatureInRange = true;
                    midPoint = (transform.position + creature2pos) /2;
                    break;
                }
                else
                {
                    continue;
                }
                
            }
            else
            {
                creatureInRange = false;
                creature2 = null;
            }
            
        }
    }

    private void MoveToEat(bool moving)
    {
        if (treeInRange && currentHunger < 50 && !moving)
        {
            ai.destination = treepos;
            ai.SearchPath();
            moving = true;
            newStatus = "Going To Eat...";
            tMP_Text.text = tMP_Text.text.Replace(status,newStatus);
            status = newStatus;
            if (ai.reachedEndOfPath && tree != null)
            {
                tree.Eat(gameObject);
                treeInRange = false;
                moving = false;
            }

        }
    }

    private void MoveToDrink(bool moving)
    {
        if (waterInRange && currentThirst < 50 && !moving)
        {
            ai.destination = waterpos;
            ai.SearchPath();
            moving = true;
            newStatus = "Going To Drink...";
            tMP_Text.text = tMP_Text.text.Replace(status,newStatus);
            status = newStatus;
            if (ai.reachedEndOfPath)
            {
                currentThirst = maxThirst;
                moving = false;
            }
        }
    }

    private void MoveToReproduce(bool moving)
    {
        if(creatureInRange && currentUrge > 50f && !moving && !isReproducing && creature2 != null && !creature2.moving && creature2.currentUrge >50f && !creature2.isReproducing && currentHunger > 50 && creature2.currentHunger > 50)
        {
            ai.destination = new Vector3 (midPoint.x - 0.5f,midPoint.y,midPoint.z - 0.5f);
            creature2.ai.destination = new Vector3 (midPoint.x + 0.5f,midPoint.y,midPoint.z + 0.5f);
            ai.SearchPath();
            creature2.ai.SearchPath();
            moving = true;
            waiting = true;
            creature2.waiting = true;
            creature2.moving = true;
            isReproducing = true;
            creature2.isReproducing = true;
            Vector3 childPos = transform.position;
            newStatus = "Going To Reproduce...";
            tMP_Text.text = tMP_Text.text.Replace(status,newStatus);
            status = newStatus;
        }
        if(isReproducing && creature2 != null)
        {
            if(endOfPath && creature2.endOfPath)
            {
                GameObject child = Instantiate(creatureprefab);
                reproduction.Reproduce(gameObject,creature2,child);
                Vector3 currentScale = child.transform.localScale;
                Vector3 newPos = child.transform.position;
                newPos.x += currentScale.x;
                newPos.z += currentScale.z;
                child.transform.position = newPos;
                currentUrge = 0f;
                creature2.currentUrge = 0f;
                waiting = false;
                creature2.waiting = false;
                isReproducing = false;
                creature2.isReproducing = false;
                moving = false;
                creature2.moving = false;
            }
        }
    }

    void Wander(bool waiting)
    {
        if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath) && !waiting)
        {
            RandomiseRandomNode();
        }
    }

    void CheckEdgeCollision()
    {
        if(ai.velocity.sqrMagnitude <= 0.001f)
        {
            stuckTime += Time.deltaTime;
            if(stuckTime >= timeThreshold)
            {
                RandomiseRandomNode();
                stuckTime = 0f;
            }
        }
        else
        {
            stuckTime = 0f;
        }
        
    }


    void RandomiseRandomNode()
    {
        GraphNode node1 = AstarPath.active.GetNearest(transform.position).node;
        List<GraphNode> nodesInFov = new List<GraphNode>();
        foreach (GraphNode node in grid.nodes)
        {
            float distance = Vector3.Distance((Vector3)node.position, (Vector3)node1.position);
            if (distance < fov * moveRaduis && PathUtilities.IsPathPossible(node1, node))
            {
                nodesInFov.Add(node);
            }
        }

        if (nodesInFov.Count > 0)
        {
            randomNode = nodesInFov[UnityEngine.Random.Range(0, nodesInFov.Count)];
            ai.destination = (Vector3)randomNode.position;
            ai.SearchPath();
            newStatus = "Wandering...";
            tMP_Text.text = tMP_Text.text.Replace(status,newStatus);
            status = newStatus;
            
        }
    }
    
}
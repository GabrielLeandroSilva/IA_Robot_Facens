using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    // Obtem a posição do player
    public Transform player;   

    //Obtem a posição do tiro(Bullet)
    public Transform bulletSpawn;

    //Obtem o status do slide no game
    public Slider healthBar;  
    
    //Referencia do tiro como objeto
    public GameObject bulletPrefab;

    //Acessa o navmesh do objeto
    NavMeshAgent agent;
    public Vector3 destination; // The movement destination.
    public Vector3 target;      // The position to aim to.

    //Variavel para vida
    float health = 100.0f;   
    
    //Variavel para a rotação no nav Mesh
    float rotSpeed = 5.0f;

    float visibleRange = 80.0f;
    float shotRange = 40.0f;

    void Start()
    {
        //Inicialização do navMesh
        agent = this.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = shotRange - 5; //for a little buffer
        //Metodo para exibir status da vida
        InvokeRepeating("UpdateHealth",5,0.5f);
    }

    void Update()
    {
        //Exibi a barra de vida sobre o objeto
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        healthBar.value = (int)health;
        healthBar.transform.position = healthBarPos + new Vector3(0,60,0);
    }

    void UpdateHealth()
    {
        //Metodo referente a vida (Regenerar)
       if(health < 100)
        health ++;
    }

    void OnCollisionEnter(Collision col)
    {
        //Metodo ao colidir com objeto com a tag bullet remove vida
        if(col.gameObject.tag == "bullet")
        {
            health -= 10;
        }
    }

    [Task]
    public void PickRandomDestination()
    {
        //Metodo para realizar o uso do plugin Pand para navegação aleatoria
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    public void MoveToDestination()
    {
        //Metodo para realizar o deslocamento com o plugin Panda
        if(Task.isInspected)
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
        if(agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }
}


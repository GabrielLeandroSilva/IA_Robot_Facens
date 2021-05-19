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


    [Task]
    public void PickDestination(int x, int z)
    {
        //Metodo para realizar o uso do plugin Pand para navegação para pontos estabelecidos pelo Patrol
        Vector3 dest = new Vector3(x, 0, z);
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    public void TargetPlayer()
    {
        //Metodo para realizar o uso do plugin Pand focar na posição do player
        target = player.transform.position;
        Task.current.Succeed();
    }

    [Task]
    public bool Fire()
    {
        //Metodo para realizar o uso do plugin Pand para atirar um prefab de bullet
        GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletPrefab.transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);
        return true;
    }

    [Task]
    public void LookAtTarget()
    {
        //Metodo para realizar o uso do plugin Pand para rotacionar em direção do player
        Vector3 direction = target - this.transform.position;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);

        if (Task.isInspected)
            Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction));

        if(Vector3.Angle(this.transform.forward, direction) < 5f)
        {
            Task.current.Succeed();
        }

        
    }


    [Task]
    bool SeePlayer()
    {
        // Metodo para realizar o uso do plugin Pand virar de costas para o player
        Vector3 distance = player.transform.position - this.transform.position;
        RaycastHit hit;
        bool seeWall = false;
        Debug.DrawRay(this.transform.position, distance, Color.red);
        if(Physics.Raycast(this.transform.position, distance, out hit))
        {
            if(hit.collider.gameObject.tag == "wall")
            {
                seeWall = true;
            }
        }

        if (Task.isInspected)
            Task.current.debugInfo = string.Format("wall={0}", seeWall);

        if(distance.magnitude < visibleRange && !seeWall)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    [Task]
    bool Turn(float angle)
    {
        //Metodo para realizar a rotação do gameobject atraves do script Panda
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
        target = p;
        return true;
    }


}


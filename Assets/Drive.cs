using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {

    //Variavel relacionada a velocidade
	float speed = 20.0F;
    //Variavel de rotação
    float rotationSpeed = 120.0F;

    //Obtem o objeto do bullet
    public GameObject bulletPrefab;
    //obtem a posição do bullet
    public Transform bulletSpawn;

    void Update() {
        //Variaveis de deslocamento utilizando inputs pre configurados pela unity
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

        //Deslocamento do player
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        //Ao pressionar espaço realiza o instanciamento de um bullet 
        if(Input.GetKeyDown("space"))
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);
        }
    }
}

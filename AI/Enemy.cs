using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase del enemigo, donde se generan los cromosomas y se les asignan los valores y caracteristicas 
/// que tienen los enemigos.
/// </summary>
public class Enemy : MonoBehaviour {

    /// <summary>
    /// Array con los valores de los cromosomas
    /// </summary>
    public float[] chromosomes;

    /* Tamaño * VelocidadX * VelocidadY * Impulso */

    //[SerializeField] private float aptitud = 0, tiempoNacimiento;
    //[SerializeField] private short generacion = 0;   
    
    /// <summary>
    /// Rigidbody del enemigo que contiene el script
    /// </summary>
    private Rigidbody rb;   
    
    /// <summary>
    /// Vector que guarda la direccion del movimiento del enemigo
    /// </summary>
    Vector3 direccion;

    /// <summary>
    /// Referencia al script de impulso
    /// </summary>
    Impulse impulse;

    /// <summary>
    /// Particulas que se instancian cuando se elimina un enemigo
    /// </summary>
    [SerializeField]
    GameObject explosion;
    

    /// <summary>
    /// Funcion donde se generan los cromosomas de cada enemigo
    /// </summary>
    public void GenerateChromosome()
    {
        chromosomes = new float[Const.minValues.Length];
        for (byte i = 0; i < chromosomes.Length; i++)
        {
            chromosomes[i] = Random.Range(Const.minValues[i], Const.maxValues[i]);
        }
    }    

    // Update is called once per frame
    void Update()
    {
        //Limitamos el movimiento del enemigo en zonas concretas y si llegan al limite se invierte la direccion
        if (transform.position.z > 11)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 11);
            direccion.z *= -1;
        }
        else if (transform.position.z < -11)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -11);
            direccion.z *= -1;
        }

        if (transform.position.x > 5.7f)
        {
            transform.position = new Vector3(5.7f, transform.position.y, transform.position.z);
            direccion.x *= -1;
        }
        else if (transform.position.x < -20)
        {
            transform.position = new Vector3(-20, transform.position.y, transform.position.z);
            direccion.x *= -1;
        }

        rb.velocity = direccion;
    }

    /// <summary>
    /// Funcion create animal para las siguientes generaciones
    /// Llama a la funcion create animal pasando los nuevos valores de cromosomas
    /// </summary>
    /// <param name="newChromosome"></param>
    public void createEnemy(float[] newChromosome)
    {
        chromosomes = newChromosome;
        createEnemy();
    }

    /// <summary>
    /// Funcion para la creacion de los enemigos
    /// Se le asignan las diferentes caracteristicas
    /// </summary>
    public void createEnemy()
    {        
        rb = GetComponent<Rigidbody>();
        impulse = transform.GetChild(0).GetComponent<Impulse>();
        transform.localScale = new Vector3(chromosomes[0], chromosomes[1] , chromosomes[2]);
        rb.velocity = new Vector3(chromosomes[3], 0, chromosomes[4]);
        direccion = rb.velocity;
        impulse.impulse = chromosomes[5];
        impulse.impulseFrequence = chromosomes[6];
       
        //tiempoNacimiento = Time.timeSinceLevelLoad;        
    }

    /// <summary>
    /// Cuando los enemigos colisionan con la pelota se destruyen
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "Bullet")
        {
            GameObject gb = Instantiate(explosion, new Vector3(transform.position.x, explosion.transform.position.y, transform.position.z), explosion.transform.rotation);
            Destroy(gameObject);
            GameController.instances.poblacion.Remove(gameObject.GetComponent<Enemy>());
        }          
    }

}

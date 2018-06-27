using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase donde se gestiona todo el flujo de juego
/// </summary>
public class GameController : MonoBehaviour {

    /// <summary>
    /// Prefab del enemigo que se va a instanciar
    /// </summary>
    [SerializeField] private GameObject enemigo;

    /// <summary>
    /// Lista donde se guardan los enemigos que se encuentran en pantalla
    /// </summary>
    public List<Enemy> poblacion;

    /// <summary>
    /// Lista de spawnPoints donde spawnean los enemigos
    /// </summary>
    public List<Transform> spawnPoints;
        
    /// <summary>
    /// Punto donde spawnean el resto de enemigos que no se encuentran en pantalla
    /// </summary>
    [SerializeField]
    Transform spawnPointPoblacionGrande;

    /// <summary>
    /// Lista de toda la poblacion de enemigos que se generan
    /// </summary>
    [SerializeField]
    public List<Enemy> poblacionGrande;

    /// <summary>
    /// Lista donde guardamos los enemigos que han sobrevivido de cada 5 rondas 
    /// </summary>
    [SerializeField]
    public List<Enemy> poblacionVictoriosa;

    /// <summary>
    /// Instancia del gameController para manejarlo desde otras clases
    /// </summary>
    public static GameController instances;

    /// <summary>
    /// Numero auxiliar para ir cogiendo cada elemento de la poblacion victoriosa
    /// </summary>
    int aux;

    //public Text poblacionval;
    // Use this for initialization
    void Awake()
    {
        if (instances == null)
        {
            instances = this;
        }

        DontDestroyOnLoad(instances);

        poblacion = new List<Enemy>();
        poblacionGrande = new List<Enemy>();

        //Se spawnean todos los enemigos
        SpawnAllEnemies();

        //Se reposicionan los 5 primeros
        Repositionate();
    }

    private void Start()
    {
        aux = -1;
    }

    /// <summary>
    /// Metodo para el spawneo de los 30 primeros enemigos
    /// Se inicializan sus valores llamando a los metodos correspondientes de la clase Enemy
    /// </summary>
    public void SpawnAllEnemies()
    {
        for (int i = 0; i < 50; i++)
        {
            //Aqui se instancian los enemigos y se posicionan en el spawnpoint fuera de pantalla       
            GameObject newEnemigo = Instantiate(enemigo, spawnPointPoblacionGrande.position, Quaternion.identity);

            //Aqui se crea el script que se asigna a cada enemigo
            Enemy script = newEnemigo.GetComponent<Enemy>();
            script.GenerateChromosome();
            script.createEnemy();
            script.enabled = false;
            newEnemigo.transform.GetChild(0).GetComponent<Impulse>().enabled = false;
            poblacionGrande.Add(script);
        }
    }

    /// <summary>
    /// Metodo para el spawneo de los enemigos siguientes
    /// Se inicializan sus valores llamando a los metodos correspondientes de la clase Enemy
    /// Utiliza los valores de cromosomas anteriores
    /// </summary>
    public void SpawnAllEnemies(float[] chromosomes)
    {
        //Aqui se instancian los enemigos y se posicionan en el spawnpoint fuera de pantalla      
        GameObject newEnemigo = Instantiate(enemigo, spawnPointPoblacionGrande.position, Quaternion.identity);

        //Aqui se crea el script que se asigna a cada enemigo
        Enemy script = newEnemigo.GetComponent<Enemy>();
        script.GenerateChromosome();
        script.createEnemy(chromosomes);
        script.enabled = false;
        newEnemigo.transform.GetChild(0).GetComponent<Impulse>().enabled = false;
        poblacionGrande.Add(script);
    }

    /// <summary>
    /// Metodo para la generacion de los siguientes enemigos a partir de los valores que han sobrevivido de la generacion anterior
    /// </summary>
    private void NewEnemigo()
    {
        for (int j = 0; j < 50; j++)
        {
            short[] parents = new short[Const.numberOfparents];
            for (short i = 0; i < parents.Length; i++)
            {
                parents[i] = (short)Random.Range(0, poblacionVictoriosa.Count);// * Const.elegibleParents);
            }

            //recombinacion
            float[] chromosomes = new float[Const.minValues.Length];
            for (byte i = 0; i < chromosomes.Length; i++)
            {
                short parent = (short)Random.Range(0, parents.Length);
                chromosomes[i] = poblacionVictoriosa[parents[parent]].chromosomes[i];
            }

            //mutacion
            for (byte i = 0; i < chromosomes.Length; i++)
            {
                if (Random.Range(0f, 1f) <= Const.mutationRatio)
                    chromosomes[i] = Random.Range(Const.minValues[i], Const.maxValues[i]);
            }


            //crear enemigo
            SpawnAllEnemies(chromosomes);
        }
    }

    /// <summary>
    /// Metodo para guardar al superviviente de cada 5 rondas 
    /// </summary>
    void Remove()
    {
        if (poblacion.Count == 1)
        {
            aux++;

            poblacionVictoriosa.Add(poblacion[0]);
            poblacionVictoriosa[aux].GetComponent<Enemy>().enabled = false;
            poblacionVictoriosa[aux].transform.GetChild(0).GetComponent<Impulse>().enabled = false;
            poblacionVictoriosa[aux].transform.position = spawnPointPoblacionGrande.position;
            poblacion.Remove(poblacion[0]);
        }
    }

    /// <summary>
    /// Metodo para reposicionar a los 5 primeros de la lista de poblacion grande en los spawn points
    /// </summary>
    void Repositionate()
    {
        for (int i = 0; i < Const.generationSize; i++)
        {
            poblacion.Add(poblacionGrande[0]);
            poblacion[i].GetComponent<Enemy>().enabled = true;
            poblacion[i].transform.GetChild(0).GetComponent<Impulse>().enabled = true;

           // Aqui se instancian los enemigos y se posicionan en los spawnPoints
            int aux = Random.Range(0, spawnPoints.Count - 1);
            poblacion[i].transform.position = spawnPoints[aux].position;

            poblacionGrande.Remove(poblacionGrande[0]);           
        }
    }

    // Update is called once per frame
    void Update()
    {      
        //Cuando quede uno se guarda en la lista de victoriosos y se saca de pantalla
        if (poblacion.Count <= 1)
        {
            Remove();

            //Cuando se eliminan los 30 se produce una nueva generacion y se reposicionan
            if (poblacionGrande.Count == 0)
            {
                NewEnemigo();
            }
            Repositionate();
        }
    }
}

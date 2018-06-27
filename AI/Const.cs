using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Archivo donde se guardan las constantes que tiene cada enemigo
/// </summary>
public class Const {

    /* Tamaño * VelocidadX * VelocidadZ * Impulso  * Frecuencia */

    public static float[] minValues = {0.75f, 1f, 0.75f, -10, -10, 10000, 1};

    public static float[] maxValues = {2.5f, 1f, 2.5f, 10, 10, 50000, 6};

    public static short generationSize = 10;

    public static float elegibleParents = 0.1f;
    public static byte numberOfparents = 2;
    public static float mutationRatio = 0.05f;
    
}

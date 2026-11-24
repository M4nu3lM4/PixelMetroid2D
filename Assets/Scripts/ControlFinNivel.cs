using TMPro;
using UnityEngine;

public class ControlFinNivel : MonoBehaviour
{
    [SerializeField] private TMP_Text mensajeFinal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(mensajeFinal == null)
        {
            Debug.LogError("Asigna el TextMeshPro a Mensaje Final");
            return;
        }
        ControlDatosJuego datosJuego = ControlDatosJuego.instancia;

        mensajeFinal.text = datosJuego.HaGanado ? 
            "¡Has ganado \n Puntuación obtenida: " + datosJuego.Puntuacion
            : "Lo siento. Has perdido.";
    }

    
}

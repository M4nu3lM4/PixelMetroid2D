using UnityEngine;

public class ControlDatosJuego : MonoBehaviour
{
    public static ControlDatosJuego instancia{ get; private set; }
    private int puntuacion;
    private bool haGanado;

    public int Puntuacion => puntuacion;

    public bool HaGanado => haGanado;
    private void Awake()
    {
        if(instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    public void reiniciarPartida()
    {
        puntuacion = 0;
        haGanado = false;
    }

    public void guardarResultado(bool gano, int puntos)
    {
        haGanado = gano;
        puntuacion = gano ? puntos : 0;
    }

}

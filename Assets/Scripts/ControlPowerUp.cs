using UnityEngine;

public class ControlPowerUp : MonoBehaviour
{
    [SerializeField, Range(1, 10)] private int cantidad = 2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if(collision.gameObject.TryGetComponent(out MovimientoJugador jugador))
        {
            //Llamar al metodo publico que incrementa la puntuacion
            jugador.IncrementarPuntos(cantidad);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("El objeto player necesita un componente MovimientoJugador");
        }
    }
}

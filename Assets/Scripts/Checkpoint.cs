using UnityEngine;

// Bandera / terminal que cambia el punto de reaparición del jugador al tocarla.
// Necesita un Collider2D con "Is Trigger" activado.
[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Header("Reaparicion")]
    // Opcional: hijo del checkpoint donde aparecerá el jugador (por ejemplo, un poco por encima del suelo).
    // Si se deja vacío, reaparece en la posición del propio checkpoint.
    [SerializeField] private Transform puntoReaparicion;

    [Header("Aspecto")]
    [SerializeField] private SpriteRenderer renderizador;
    [SerializeField] private Sprite spriteActivo;                 // Opcional: sprite de la bandera activada
    [SerializeField] private Color colorInactivo = Color.gray;    // Se usa si no hay spriteActivo
    [SerializeField] private Color colorActivo = Color.white;

    // Checkpoint activo en este momento (compartido por todos), para apagar el anterior
    private static Checkpoint activoActual;

    private bool activado;
    private Sprite spriteInactivo;

    private void Awake()
    {
        if (renderizador == null)
            renderizador = GetComponent<SpriteRenderer>();

        if (renderizador != null)
        {
            spriteInactivo = renderizador.sprite;
            if (spriteActivo == null)
                renderizador.color = colorInactivo;
        }

        // Aseguramos que el collider es un trigger
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D colision)
    {
        if (activado) return;

        MovimientoJugador jugador = colision.GetComponentInParent<MovimientoJugador>();
        if (jugador == null) return;

        // Apagamos el checkpoint anterior para que solo haya uno activo
        if (activoActual != null)
            activoActual.Desactivar();

        jugador.EstablecerPuntoReaparicion(puntoReaparicion != null ? puntoReaparicion : transform);
        Activar();
    }

    private void Activar()
    {
        activado = true;
        activoActual = this;

        if (renderizador == null) return;
        if (spriteActivo != null)
            renderizador.sprite = spriteActivo;
        else
            renderizador.color = colorActivo;
    }

    private void Desactivar()
    {
        activado = false;

        if (renderizador == null) return;
        if (spriteActivo != null)
            renderizador.sprite = spriteInactivo;
        else
            renderizador.color = colorInactivo;
    }

    // Al empezar una nueva partida/escena no debe quedar una referencia vieja
    private void OnDestroy()
    {
        if (activoActual == this)
            activoActual = null;
    }
}

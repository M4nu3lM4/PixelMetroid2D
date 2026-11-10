using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ControlEnemigoHorizontal : MonoBehaviour
{
    public float velocidad = 2f;

    public Vector2 posicionFinal;
    private Vector2 posicionInicial;

    private Rigidbody2D fisica;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private float tolerancia = 0.1f;

    private bool haciaFinal = true;

    public float distancia = 4f;

    private void Awake()
    {
        fisica = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        posicionInicial = fisica.position;
        posicionFinal = posicionInicial + Vector2.left * distancia;
    }

    private void FixedUpdate()
    {
        float objetivoX = haciaFinal ? posicionFinal.x : posicionInicial.x;

        // Al llegar a la esquina, cambia de sentido
        if (Mathf.Abs(fisica.position.x - objetivoX) <= tolerancia)
        {
            haciaFinal = !haciaFinal;
            objetivoX = haciaFinal ? posicionFinal.x : posicionInicial.x;
        }

        // +1 si el objetivo está a la derecha, -1 si está a la izquierda
        float direccion = Mathf.Sign(objetivoX - fisica.position.x);

        // Solo cambio la X; la Y la controla la gravedad
        fisica.linearVelocity = new Vector2(direccion * velocidad, fisica.linearVelocity.y);

        // Girar el sprite (si mira hacia atrás, cambia < por >)
        if (sprite != null)
        {
            sprite.flipX = direccion > 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D colision)
    {
        if (!colision.gameObject.CompareTag("Player")) return;

        if (colision.gameObject.TryGetComponent(out MovimientoJugador jugador))
        {
            //jugador.ReaparecerJugador();
            jugador.QuitarVidas();
        }
        else
        {
            Debug.LogError("El objeto Player necesita ControlJugador en la raiz");
        }

    }
}
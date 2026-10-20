using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody2D))]
public class MovimientoJugadorSaltoMetodo2 : MonoBehaviour
{
    [Header("Movimineto")]
    public float velocity;

    [SerializeField] private InputActionReference accionMover;

    [Header("Salto")]
    [SerializeField] private InputActionReference accionSaltar;

    public float fuerzaImpulso = 5f; //Fuerza aplicada al saltar
    

    [Header("Chequeo de suelo")]
    [SerializeField] private float groundDistance = 0.2f;//Radio de la esfera que va a tener esfera
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private Transform esferaPies; //Objeto vacio en los pies

    private Rigidbody2D fisica;
    private Vector2 entradaMovimiento;
    private bool estaTocandoSuelo;
    private bool saltoPendiente;

    private void Awake()
    {
        fisica = GetComponent<Rigidbody2D>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Capturamos las posiciones de las teclas
        entradaMovimiento = accionMover.action.ReadValue<Vector2>();

        estaTocandoSuelo = Physics2D.OverlapCircle(esferaPies.position, groundDistance, capaSuelo);

        if(accionSaltar.action.WasPerformedThisFrame() && estaTocandoSuelo)
        {
            saltoPendiente = true;
        }

    }

    private void FixedUpdate()
    {
        //Movimiento de X a una velocidad determinada
        fisica.linearVelocity = new Vector2(entradaMovimiento.x * velocity, fisica.linearVelocity.y);

        if (saltoPendiente)
        {
            fisica.AddForce(Vector2.up * fuerzaImpulso, ForceMode2D.Impulse);
        }
        saltoPendiente = false;

    }

    private void OnEnable()
    {
        //Si el movimiento de suelo no esta activado lo activamos
        if ((accionMover == null) || accionMover.action == null)
        {
            Debug.LogError("Asigna mapa de acción Jugador/Mover en accionMover");
            enabled = false;
            return;
        }
        accionMover.action.Enable();

        //Si el movimiento de salto no esta activado lo activamos
        if ((accionSaltar == null) || accionSaltar.action == null)
        {
            Debug.LogError("Asigna mapa de acción Jugador/Saltar en accionSaltar");
            enabled = false;
            return;
        }
        accionSaltar.action.Enable();
    }
    private void OnDisable()
    {
        //Si el movimiento de suelo esta activado lo desactivamos
        if (accionMover != null && accionMover.action != null)
        {
            accionMover.action.Disable();
        }

        //Si el movimiento de salto esta activado lo desactivamos
        if (accionSaltar != null && accionSaltar.action != null)
        {
            accionSaltar.action.Disable();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(esferaPies != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(esferaPies.position, groundDistance);
        }
    }
}

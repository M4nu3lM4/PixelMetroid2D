using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(Rigidbody2D))]
public class MovimientoJugador : MonoBehaviour
{
    [Header("Movimineto")]
    public float velocity;

    [SerializeField] private InputActionReference accionMover;

    [Header("Salto")]
    [SerializeField] private InputActionReference accionSaltar;

    public float fuerzaImpulso = 5f; //Fuerza aplicada al saltar
    [SerializeField] private LayerMask capaSuelo;

    private Boolean saltoPendiente;
    private Rigidbody2D fisica;

    private SpriteRenderer renderizador;

    private Animator animador;
    private Vector2 entradaX;

    private Boolean estaTocandoSuelo;

    private float longitudRaycast = 1.6f;

    private void Awake()
    {
        fisica = GetComponent<Rigidbody2D>();

        renderizador = GetComponent<SpriteRenderer>();

        animador = GetComponent<Animator>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        entradaX = accionMover.action.ReadValue<Vector2>();

        //Creamos el rayo
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);

        //Si esta tocando el suelo hit.collider es distinto de null, si no esta tocando el suelo hit.collider es nulo
        estaTocandoSuelo = hit.collider != null;

        //Creamos el color del rayo dependiendo de si el jugador esta tocando el suelo o no
        Color colorRayo = estaTocandoSuelo ? Color.green : Color.red;

        //Dibujmos el rayo
        Debug.DrawRay(transform.position, Vector2.down * longitudRaycast, colorRayo);

        if (accionSaltar.action.WasPressedThisFrame() && estaTocandoSuelo)
        {
            saltoPendiente = true;
        }

        AnimarJugador();
    }

    private void AnimarJugador()
    {
        if (animador == null)
            return;

        if (!estaTocandoSuelo)
        {
            animador.Play("JugadorSaltando");
        }
        else if (Mathf.Abs(fisica.linearVelocity.x) > 0)
        {
            animador.Play("JugadorCorriendo");
        }
        else
            animador.Play("JugadorParado");
    }

    private void OnEnable()
    {
        //Si el movimiento de suelo no esta activado lo activamos
        if((accionMover == null) || accionMover.action == null)
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

    private void FixedUpdate()
    {
        fisica.linearVelocity = new Vector2(entradaX.x * velocity, fisica.linearVelocity.y);

        //añadimos la fuerza de impulso al salto
        if (saltoPendiente)
        {
            fisica.AddForce(Vector2.up * fuerzaImpulso, ForceMode2D.Impulse);
        }
        saltoPendiente = false;

        OrientarSprite();
    }
    private void OrientarSprite()
    {
        if(renderizador == null)
        return;

        if(fisica.linearVelocity.x > 0)
        {
            renderizador.flipX = false;
        }
        else if(fisica.linearVelocity.x < 0)
        {
            renderizador.flipX = true;
        }
    }
}

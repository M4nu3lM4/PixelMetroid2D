using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using MinAttribute = UnityEngine.MinAttribute;

[RequireComponent (typeof(Rigidbody2D))]
public class MovimientoJugador : MonoBehaviour
{
    // ---------------------------------------------------------------
    // MOVIMIENTO
    // ---------------------------------------------------------------
    [Header("Movimiento")]
    public float velocity; // Velocidad del jugador, se edita en el inspector

    [SerializeField] private InputActionReference accionMover;// Acción de input "Mover" (teclas/mando), asignada en el Inspector
    // ---------------------------------------------------------------
    // SALTO
    // ---------------------------------------------------------------
    [Header("Salto")]
    [SerializeField] private InputActionReference accionSaltar;//Acción de input "saltar"

    public float fuerzaImpulso = 5f; //Fuerza aplicada al saltar
    [SerializeField] private LayerMask capaSuelo;

    // ---------------------------------------------------------------
    // REAPARICIÓN
    // ---------------------------------------------------------------
    [Header("Reaparicion")]
    [SerializeField] private Transform spawnPoint;

    // ---------------------------------------------------------------
    // VARIABLES INTERNAS (referencias y estado)
    // ---------------------------------------------------------------
    private Boolean saltoPendiente;
    private Rigidbody2D fisica;

    private SpriteRenderer renderizador;

    private Animator animador;
    private Vector2 entradaX;

    private Boolean estaTocandoSuelo;

    private float longitudRaycast = 1.6f;
    [SerializeField] private int puntuacion;

    // ---------------------------------------------------------------
    // SALUD
    // ---------------------------------------------------------------
    [Header("Salud")]
    [SerializeField] private int vidas = 3;
    //Metodo get en Unity
    public int Vidas => vidas;

    private Color colorOriginal;

    private bool vulnerable = true;

    [SerializeField, Min(0.01f)] private float segundosInvulnerable = 1f;

    // ---------------------------------------------------------------
    // TIEMPO
    // ---------------------------------------------------------------
    [Header("Tiempo")]
    private float tiempoInicio;
    private float tiempoReal;
    [SerializeField] private int tiempoEmpleado;
    [SerializeField, Min(1)] private int tiempoLimiteNivel = 100;
    [Header("Canvas")]
    [SerializeField] private Canvas canvasInterfaz;
    private ControlHUD hud;

    private ControlDatosJuego controlDatosJuego;

    [Header("Audio")]
    [SerializeField] private AudioClip saltoSonido;
    [SerializeField] private AudioClip vidasSonido;
    private AudioSource efectoSonido;

    // Awake se ejecuta al crear el objeto, antes de Start: aquí se cogen las referencias a los componentes
    private void Awake()
    {
        fisica = GetComponent<Rigidbody2D>();

        renderizador = GetComponent<SpriteRenderer>();

        animador = GetComponent<Animator>();

        

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (renderizador != null)
            colorOriginal = renderizador.color;
        else
            Debug.LogError("El jugador no tiene asignado el objeto Sprite renderer");

        tiempoInicio = Time.time;

        controlDatosJuego = ControlDatosJuego.instancia;
        if (controlDatosJuego == null)
        {
            Debug.LogError("Añade Datos Juego al nivel");
        }
        else
            controlDatosJuego.reiniciarPartida();

        inicializaHud();
    }

    // Detecta si estamos en el suelo y si hay que saltar. Se llama desde Update
    private void SaltoJugador()
    {
        //Creamos el rayo
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);

        //Si esta tocando el suelo hit.collider es distinto de null, si no esta tocando el suelo hit.collider es nulo
        //Solo cuenta como suelo si no estamos subiendo: asi no se puede saltar mientras se atraviesa una plataforma desde abajo
        estaTocandoSuelo = hit.collider != null && fisica.linearVelocity.y <= 0.1f;

        //Creamos el color del rayo dependiendo de si el jugador esta tocando el suelo o no
        Color colorRayo = estaTocandoSuelo ? Color.green : Color.red;

        //Dibujmos el rayo
        Debug.DrawRay(transform.position, Vector2.down * longitudRaycast, colorRayo);

        // Si se acaba de pulsar saltar y estamos en el suelo, dejamos el salto pendiente
        // (el impulso físico se aplica en FixedUpdate, no aquí)
        if (accionSaltar.action.WasPressedThisFrame() && estaTocandoSuelo)
        {
            saltoPendiente = true;
        }
    }



    // Update is called once per frame
    // Aquí va lo que depende de la entrada y la lógica por fotograma
    void Update()
    {
        // Leemos la dirección pulsada (Vector2)
        entradaX = accionMover.action.ReadValue<Vector2>();

        ComprobarFinJuegoTiempo();

        SaltoJugador();
    

        AnimarJugador();
    }

    private void inicializaHud()
    {
        if (canvasInterfaz != null)
        {
            hud = canvasInterfaz.GetComponent<ControlHUD>();
        }
        else
            Debug.LogError("Asigna el objeto Canvas con el componente ControlHUD al atributo Canvas del Jugador.");

        if (hud != null)
        {
            hud.setVidas(vidas);
            hud.setTiempo(tiempoLimiteNivel);
            hud.setPowerUps(GameObject.FindGameObjectsWithTag("PowerUp").Length);
        }
        else
            Debug.LogError("Asigna el objeto Canvas con el componente ControlHUD al atributo Canvas del Jugador.");
    }

    // Calcula el tiempo transcurrido y termina la partida si se pasa del límite
    private void ComprobarFinJuegoTiempo()
    {

        
        tiempoReal = Time.time - tiempoInicio;
        tiempoEmpleado = Mathf.FloorToInt(tiempoReal);// Redondea hacia abajo a segundos enteros
        
        if (tiempoEmpleado >= tiempoLimiteNivel)
        {
            FinJuego();
            return;
        }
        hud.setPowerUps(GameObject.FindGameObjectsWithTag("PowerUp").Length);
        int segundosRestantes = Mathf.CeilToInt(tiempoLimiteNivel - tiempoReal);
        hud.setTiempo(segundosRestantes);
    }

    // Elige la animación según el estado del jugador
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

    // OnEnable se ejecuta al activarse el script: activamos las acciones de input
    private void OnEnable()
    {
        //Si el movimiento de suelo no esta activado lo activamos
        if((accionMover == null) || accionMover.action == null)
        {
            Debug.LogError("Asigna mapa de acción Jugador/Mover en accionMover");
            enabled = false;//Desactivamos el script si falta la refrencia
            return;
        }
        accionMover.action.Enable();

        //Si el movimiento de salto no esta activado lo activamos
        if ((accionSaltar == null) || accionSaltar.action == null)
        {
            Debug.LogError("Asigna mapa de acción Jugador/Saltar en accionSaltar");
            enabled = false;//Desactivamos el script si falta la referencia
            return;
        }
        accionSaltar.action.Enable();
    }

    // OnDisable se ejecuta al desactivarse el script: desactivamos las acciones para no dejarlas activas
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
    // FixedUpdate se ejecuta a intervalos fijos, sincronizado con la física: aquí se mueve el Rigidbody
    private void FixedUpdate()
    {
        MoverJugador();

        OrientarSprite();
    }

    // Aplica el movimiento horizontal y el salto al Rigidbody
    private void MoverJugador()
    {
        // Cambia solo la velocidad en X; la Y se mantiene (la controla la gravedad y el salto)
        fisica.linearVelocity = new Vector2(entradaX.x * velocity, fisica.linearVelocity.y);

        //añadimos la fuerza de impulso al salto
        if (saltoPendiente)
        {
            fisica.AddForce(Vector2.up * fuerzaImpulso, ForceMode2D.Impulse);

            //if (saltoSonido != null)
                //efectoSonido.PlayOneShot(saltoSonido);
        }
        // El salto ya se ha aplicado
        saltoPendiente = false;
    }

    // Gira el sprite según hacia dónde se mueva
    private void OrientarSprite()
    {
        if(renderizador == null)
        return;

        if(fisica.linearVelocity.x > 0)
        {
            renderizador.flipX = false;// Mirando a la derecha
        }
        else if(fisica.linearVelocity.x < 0)
        {
            renderizador.flipX = true;// Mirando a izquierda
        }
    }

    // Lo llamarán otros scripts (por ejemplo, al caer al vacío) para devolver al jugador al spawn
    public void ReaparecerJugador()
    {
        if(spawnPoint == null)
        {
            Debug.LogError("Asigna un spawn al jugador",this);
            enabled = false;
            return;
        }
        fisica.position = (Vector2)spawnPoint.position;// Teletransporta al punto de spawn
        fisica.linearVelocity = Vector2.zero;// Para que no conserve la velocidad de la caída
    }

    // Suma puntos (ignora cantidades 0 o negativas)
    public void IncrementarPuntos(int cantidad)
    {
        if (cantidad <= 0) return;

        puntuacion += cantidad;
    }

    // Resta una vida y activa la invulnerabilidad temporal
    public void QuitarVidas()
    {
        if (!vulnerable) return;// Si está en periodo de invulnerabilidad, no hace nada
        vulnerable = false;
        vidas--;

        

        hud.setVidas(vidas);

        // Feedback visual del daño: el sprite se pone azul
        if (renderizador != null)
        {
            renderizador.color = Color.blue;
        }

        

        if (vidas <= 0)
        {
            FinJuego();
            return;
        }
        // Tras segundosInvulnerable segundos vuelve a ser vulnerable
        Invoke(nameof(HacerVulnerable), segundosInvulnerable);
        //if (saltoSonido != null)
        //efectoSonido.PlayOneShot(saltoSonido);

    }

    // Termina la invulnerabilidad y restaura el color
    private void HacerVulnerable()
    {
        vulnerable = true;
        if (renderizador != null)
            renderizador.color = colorOriginal;
        
    }

    // Fin de partida: por ahora solo desactiva al jugador
    private void FinJuego()
    {
        if(controlDatosJuego == null)
        {
            Debug.LogError("Falta el objeto Control Datos Juego para poder guardar el resultado de la partida",this);
            return;
        }
        controlDatosJuego.guardarResultado(false, 0);
        SceneManager.LoadScene("FinNivel");
    }

    //Paramos el jugador y guardamos los datos de la partida
    public void Victoria()
    {
        puntuacion = vidas * 100 + Mathf.Max(0, tiempoLimiteNivel - tiempoEmpleado);

        saltoPendiente = false;

        fisica.linearVelocity = Vector2.zero;

        controlDatosJuego.guardarResultado(true, puntuacion);

        SceneManager.LoadScene("FinNivel");
    }

    public void EstablecerPuntoReaparicion(Transform nuevoPunto)
    {
        if (nuevoPunto == null) return;
        
            spawnPoint = nuevoPunto;
        
    }
}
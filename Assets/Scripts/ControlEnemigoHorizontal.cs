using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class ControlEnemigoHorizontal : MonoBehaviour
{
    public float velocidad = 2f;

    public Vector2 posicionFinal;
    private Vector2 posicionInicial;
    private Rigidbody2D fisica;

    [SerializeField] private float tolerancia = 0.01f;
    private bool movimientoHorizontal = true;

    private void Awake()
    {
        fisica = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        posicionInicial = fisica.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //Le aplico la velocidad para movimiento horizontal
        fisica.linearVelocity = new Vector2(fisica.position.x * velocidad, fisica.linearVelocity.y);


    }
}

using UnityEngine;

public class ControlJugadorInputSystemOld : MonoBehaviour
{
    public float velocidad;
    private float entradaX;

    private Rigidbody2D fisica;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Obtengo el componente rigidbody del player para modificar posteriormente linearVelocity
        fisica = GetComponent<Rigidbody2D>();
    }







    //update is called once per frame
    //void update()
    //{
    //    vector2 position = transform.position;

    //    position.x = position.x + 0.1f;

    //    transform.position = position;
    //}

    private void FixedUpdate()
    {
        //Obtenemos las teclas de dirección que ha pulsado el usuario
        entradaX = Input.GetAxis("Horizontal");
        if(fisica != null)
        {
            fisica.linearVelocity = new Vector2(entradaX * velocidad, fisica.linearVelocity.y);
        }
        Debug.LogError("El GameObjct no tiene asignado Rigidboy2D que es necesario para moverlo");
    }
}

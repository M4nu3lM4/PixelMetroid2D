using UnityEngine;
using TMPro;
public class ControlHUD : MonoBehaviour
{
    public TMP_Text vidasTxt;
    public TMP_Text powerUpsTxt;
    public TMP_Text tiempoTxt;

    private void Awake()
    {
        if (vidasTxt == null || powerUpsTxt == null || tiempoTxt == null)
        {
            Debug.LogError("Asigna los textos TMP del HUD al componente ControlHUD");
        }
    }

    public void setVidas(int cantidad)
    {
        if(vidasTxt != null)
        {
            vidasTxt.text = "Vidas: " + cantidad;
        }
    }

    public void setPowerUps(int cantidad)
    {
        if (powerUpsTxt != null)
        {
            powerUpsTxt.text = "Power Ups: " + cantidad;
        }
    }

    public void setTiempo(int segundosrestantes)
    {
        if (tiempoTxt == null) return;

        int minutos = segundosrestantes / 60;
        int segundos = segundosrestantes & 60;

        tiempoTxt.text = minutos.ToString("00") + ":" + segundos.ToString("00");
    }



}

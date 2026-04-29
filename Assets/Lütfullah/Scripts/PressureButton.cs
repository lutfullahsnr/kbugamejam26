using UnityEngine;
using DG.Tweening; 

public class PressureButton : MonoBehaviour
{
    [Header("Buton Ayarları")]
    [Tooltip("Butonun aşağı çökecek olan görsel objesi (Parent objeyi değil, içindeki Sprite'ı seç)")]
    public Transform buttonVisual; 
    public float buttonPressDistance = 0.05f; 
    public float buttonAnimDuration = 0.2f;   

    [Header("Asansör Ayarları")]
    public Transform elevator; 
    public Transform elevatorTarget;
    public float elevatorMoveDuration = 1.5f; 
    private Vector3 buttonStartPos;
    private Vector3 elevatorStartPos;
    private int charactersOnButton = 0; 

    void Start()
    {
        if (buttonVisual != null) buttonStartPos = buttonVisual.position;
        if (elevator != null) elevatorStartPos = elevator.position;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Character"))
        {
            charactersOnButton++;
            if (charactersOnButton == 1)
            {
                ActivateSystem();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Character") || other.CompareTag("Player"))
        {
            charactersOnButton--;

            if (charactersOnButton <= 0)
            {
                charactersOnButton = 0; 
                DeactivateSystem();
            }
        }
    }

    private void ActivateSystem()
    {
        buttonVisual.DOKill();
        buttonVisual.DOMoveY(buttonStartPos.y - buttonPressDistance, buttonAnimDuration);

        if (elevator != null && elevatorTarget != null)
        {
            elevator.DOKill();
            elevator.DOMove(elevatorTarget.position, elevatorMoveDuration).SetEase(Ease.InOutSine);
        }
    }

    private void DeactivateSystem()
    {
        buttonVisual.DOKill();
        buttonVisual.DOMoveY(buttonStartPos.y, buttonAnimDuration);

        if (elevator != null)
        {
            elevator.DOKill();
            elevator.DOMove(elevatorStartPos, elevatorMoveDuration).SetEase(Ease.InOutSine);
        }
    }
}
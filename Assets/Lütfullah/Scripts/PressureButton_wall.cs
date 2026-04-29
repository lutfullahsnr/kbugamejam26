using UnityEngine;
using DG.Tweening; 

public class PressureButton_wall : MonoBehaviour
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
    [Header("Gizli Duvar Ayarları")]
    [Tooltip("Tuşa basılınca X ekseninde küçülerek kaybolacak duvar")]
    public Transform secretWall; 
    public float wallHideDuration = 0.5f;
    private Vector3 buttonStartPos;
    private Vector3 elevatorStartPos;
    private Vector3 wallStartScale; 
    private int charactersOnButton = 0; 

    void Start()
    {
        if (buttonVisual != null) buttonStartPos = buttonVisual.position;
        if (elevator != null) elevatorStartPos = elevator.position;
        
        if (secretWall != null) wallStartScale = secretWall.localScale; 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Character") || other.CompareTag("Player"))
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
        if (buttonVisual != null)
        {
            buttonVisual.DOKill();
            buttonVisual.DOMoveY(buttonStartPos.y - buttonPressDistance, buttonAnimDuration);
        }

        if (elevator != null && elevatorTarget != null)
        {
            elevator.DOKill();
            elevator.DOMove(elevatorTarget.position, elevatorMoveDuration).SetEase(Ease.InOutSine);
        }

        if (secretWall != null)
        {
            secretWall.DOKill();
            secretWall.DOScaleX(0f, wallHideDuration).SetEase(Ease.InOutSine);
        }
    }

    private void DeactivateSystem()
    {
        if (buttonVisual != null)
        {
            buttonVisual.DOKill();
            buttonVisual.DOMoveY(buttonStartPos.y, buttonAnimDuration);
        }

        if (elevator != null)
        {
            elevator.DOKill();
            elevator.DOMove(elevatorStartPos, elevatorMoveDuration).SetEase(Ease.InOutSine);
        }

        if (secretWall != null)
        {
            secretWall.DOKill();
            secretWall.DOScaleX(wallStartScale.x, wallHideDuration).SetEase(Ease.InOutSine);
        }
    }
}
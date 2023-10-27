using UnityEngine;

public class BalanceIndicator : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Whether to base the indicator's rotation on the board rotation (checked) or the player's head rotation (unchecked)")]
    public bool UseBoardRotation = true;

    [SerializeField]
    [Tooltip("Degrees that the player must rotate left or right when the minecart is turning")]
    private float turnAngle = 20;

    [SerializeField]
    [Tooltip("Spring constant for turning indicators. Higher values make the balance line rotations more snappy")]
    private float turnSpring = 100;

    [SerializeField]
    [Tooltip("Damping constant for turning indicators. Higher values generally make the balance line rotations smoother and slower")]
    private float turnDamper = 20;

    [SerializeField]
    [Tooltip("Threshold in degrees that the board/player rotation must be from the current minecart turn angle for the indicator to lock and turn green")]
    private float lockRotationDifference = 6;

    [Header("References")]
    [SerializeField]
    private Minecart cart;

    [SerializeField]
    private Animator uiAnimator;

    [SerializeField]
    private Transform targetPivot;

    [SerializeField]
    private Transform boardUnlockedPivot;

    [SerializeField]
    private Transform boardPivot;

    [SerializeField]
    private UdpSocket server;

    [SerializeField]
    private GameObject headBox;

    // Internal state
    private float currentCartRot = 0;
    private float currentCartVel = 0;
    private float currentBoardRot = 0;
    private float currentBoardVel = 0;
    private bool locked = false;

    private void LateUpdate()
    {
        // Always keep the balance indicator level when the minecart tilts to one side
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        // Get board and cart rotations
        float cartRot = GetCartRotation();
        float boardRot = GetBoardRotation();

        // Lock or unlock the cart rotation as needed
        // This is for a satisfying effect when the board rotation matches the minecart rotation
        bool newLocked = Mathf.Abs(Mathf.DeltaAngle(cartRot, boardRot)) <= lockRotationDifference;
        if (newLocked != locked)
        {
            locked = newLocked;
            uiAnimator.SetBool("Locked", locked);
        }

        // Update the board and cart indicator rotations (using spring damper!)
        SpringDamper(cartRot, ref currentCartRot, ref currentCartVel);
        SpringDamper(boardRot, ref currentBoardRot, ref currentBoardVel);
        targetPivot.localEulerAngles = new Vector3(0, 0, currentCartRot);
        boardUnlockedPivot.localEulerAngles = new Vector3(0, 0, currentBoardRot);
        if (!locked)
            boardPivot.localEulerAngles = new Vector3(0, 0, currentBoardRot);
        else
            boardPivot.localEulerAngles = new Vector3(0, 0, currentCartRot);
    }

    private float GetCartRotation()
    {
        return cart.turningLeft ? turnAngle : cart.turningRight ? -turnAngle : 0;
    }
    private float GetBoardRotation()
    {
        if (UseBoardRotation)
            return server.BoardRotation;
        else
            return headBox.transform.eulerAngles.z;
    }

    private void SpringDamper(float goalRot, ref float rot, ref float vel)
    {
        // Spring damper functions are too fun not to use randomly
        // (And they provide a satisfying smoothing effect for the UI elements)
        float dt = Time.deltaTime / 10;
        for (int i = 0; i < 10; i++)
        {
            rot += vel * dt;
            vel += Mathf.DeltaAngle(rot, goalRot) * turnSpring * dt;
            vel -= vel * turnDamper * dt;
        }
    }
}
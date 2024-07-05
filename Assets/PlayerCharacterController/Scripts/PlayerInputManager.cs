using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;

    public bool aim;

    private void Update()
    {
        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        move = Vector2.ClampMagnitude(move, 1f);

        look = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        jump = Input.GetKeyDown(KeyCode.Space);

        sprint = Input.GetKey(KeyCode.LeftShift);

        aim = Input.GetKey(KeyCode.Mouse1);
    }
}

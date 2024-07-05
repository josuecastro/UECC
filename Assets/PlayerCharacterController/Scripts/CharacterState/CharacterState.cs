using UnityEngine;

public class CharacterState : MonoBehaviour
{
    public static CharacterState instance;

    public BodyPosition currentBodyPosition;

    public BodyPosition previewBodyPosition;

    public static CharacterState Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<CharacterState>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("CharacterState");
                    instance = singletonObject.AddComponent<CharacterState>();
                }
            }
            return instance;
        }
    }


    public void SetCurrentBodyPosition(BodyPosition newBodyPosition)
    {
        previewBodyPosition = currentBodyPosition;
        currentBodyPosition = newBodyPosition;
    }
}

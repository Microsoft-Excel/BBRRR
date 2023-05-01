using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JeffController : MonoBehaviour
{

    [SerializeField] float gravity = 9.8f;

    private Vector2 velocity = Vector2.zero;

    private CharacterController bruh;

    // Start is called before the first frame update
    void Start()
    {
        bruh = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        // Gravity
        velocity += new Vector2(0, -1 * gravity * Time.deltaTime);

        bruh.Move(velocity * Time.deltaTime);
    }
}

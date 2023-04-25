using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 13f;
    private Vector2 inputVector = new Vector2(0, 0);
    private Vector3 moveVector = Vector3.zero;
    private Vector3 lastMove = Vector3.zero;

    private void Update()
    {
        Movement();
        Interact();
    }

    private void Interact()
    {
        float interactionDistance = 1.5f;
        if (moveVector != Vector3.zero)
        {
            lastMove = moveVector;
        }

        if (Physics.Raycast(transform.position, lastMove, out RaycastHit raycastHit, interactionDistance))
        {
            if (raycastHit.transform.TryGetComponent(out Cupboard cupboard))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    cupboard.InteractCupboard();
                }
            }
        }
    }

    private void Movement()
    {
        inputVector = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputVector.y = +1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector.y = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x = +1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x = -1;
        }

        inputVector = inputVector.normalized;

        moveVector = new Vector3(inputVector.x, 0f, inputVector.y);

        float playerHeight = .7f;
        float playerWidth = .9f;

        bool movePossible = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
                                                playerWidth, moveVector, Time.deltaTime * moveSpeed);

        if (!movePossible)
        {
            //axis x after stuck
            Vector3 moveAxisX = new Vector3(inputVector.x, 0f, 0f);
            moveAxisX = moveAxisX.normalized;

            movePossible = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
                                                playerWidth, moveAxisX, Time.deltaTime * moveSpeed);
            //if yes we will slide x axis
            if (movePossible)
            {
                moveVector = moveAxisX;
            }
            //axis z after stuck
            else
            {
                Vector3 moveAxisZ = new Vector3(0f, 0f, inputVector.y);
                moveAxisZ = moveAxisZ.normalized;

                movePossible = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
                                                    playerWidth, moveAxisZ, Time.deltaTime * moveSpeed);
                //if yes we will slide z axis
                if (movePossible)
                {
                    moveVector = moveAxisZ;
                }
            }

        }
        //if we stuck here our moveVector changed in moveAxis(x/z)
        if (movePossible)
        {
            transform.position += moveVector * Time.deltaTime * moveSpeed;
        }

        transform.forward = Vector3.Lerp(transform.forward, moveVector, Time.deltaTime * 15);// число-відповідає за пришвидшення повороту
    }
}



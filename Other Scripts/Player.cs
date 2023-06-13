using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, I_PropParent
{
    public static void ResetStaticData()
    {
        OnPlayerSpawned = null;
    }
    public static Player LocalInstance { get; private set; }

    //to know when player spawn so for exampl highlights will show
    //Static btw cause All instances of the class MUST share the same event handlers.
    public static event EventHandler OnPlayerSpawned;
    //for pause menu
    public event EventHandler OnEscapePressed;
    public event EventHandler OnReadyPressed;

    //the way in multiplayer set instances. Cause Awake or Start are not usable in that case
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
        OnPlayerSpawned?.Invoke(this, EventArgs.Empty);


        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetwworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetwworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        //Destroy props if player disconnected
        if (clientId == OwnerClientId && HasProp())
        {
            Props.DespawnProp(GetProp());
        }
    }

    public EventHandler<OnHighlightCupboardEventArgs> OnHighlightCupboard;
    public class OnHighlightCupboardEventArgs : EventArgs
    {
        public InheritCupboard selectedCupboard;
    }
    //layerMask that player could interact with only markeed objects as cupboards *note it is also must be done in unity ui
    [SerializeField] private LayerMask cupboardsLayerMask;
    [SerializeField] private Transform handPoint;
    //this for changed color was in game
    [SerializeField] private PlayerVisual playerVisual;
    private float moveSpeed = 13f;
    private Vector2 inputVector = new Vector2(0, 0);
    private Vector3 moveVector = Vector3.zero;
    private Vector3 lastMove = Vector3.zero;
    private InheritCupboard selectedCupboard;
    private Props prop;


    private void Start()
    {
        PlayerData playerData = GameNetworkConnector.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(GameNetworkConnector.Instance.GetPlayerColor(playerData.colorId));
    }
    private void Update()
    {
        //cannot move others players cause u are not their owner
        if (!IsOwner) { return; }

        Movement();
        Interact();
    }

    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapePressed?.Invoke(this, EventArgs.Empty);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnReadyPressed?.Invoke(this, EventArgs.Empty);
        }

        float interactionDistance = 2f;
        //lastMove needed if our player stand still and we want that interaction would be possible
        if (moveVector != Vector3.zero)
        {
            lastMove = moveVector;
        }

        if (Physics.Raycast(transform.position, lastMove, out RaycastHit raycastHit, interactionDistance, cupboardsLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out InheritCupboard basicCupboard))
            {
                //interaction after key input
                if (Input.GetKeyDown(KeyCode.E))
                {
                    basicCupboard.InteractCupboard(this);
                }

                //interaction after key input
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    basicCupboard.InteractCutting(this);
                }

                //highlight
                if (basicCupboard != selectedCupboard)
                {
                    SetSelectedCupboard(basicCupboard);

                }
            }
            else
            {
                // Check if the player is leaving the selected cupboard area
                if (selectedCupboard != null)
                {
                    SetSelectedCupboard(null);
                }
            }
        }
        else
        {
            // Check if the player is leaving the selected cupboard area
            if (selectedCupboard != null)
            {
                SetSelectedCupboard(null);
            }
        }
    }

    private void Movement()
    {
        inputVector = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            //added that player cant move in pause, start game or game over
            if (!GameManager_.Instance.IsPlaying())
                return;
            inputVector.y = +1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            //added that player cant move in pause, start game or game over
            if (!GameManager_.Instance.IsPlaying())
                return;
            inputVector.y = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            //added that player cant move in pause, start game or game over
            if (!GameManager_.Instance.IsPlaying())
                return;
            inputVector.x = +1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            //added that player cant move in pause, start game or game over
            if (!GameManager_.Instance.IsPlaying())
                return;
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

            movePossible = moveVector.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
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

                movePossible = moveVector.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight,
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

    private void SetSelectedCupboard(InheritCupboard selectedCupboard)
    {
        this.selectedCupboard = selectedCupboard;
        OnHighlightCupboard?.Invoke(this, new OnHighlightCupboardEventArgs
        {
            selectedCupboard = selectedCupboard
        });
    }

    public Transform GetPropTransform()
    {
        return handPoint;
    }
    public void SetPropParent(Props prop)
    {
        this.prop = prop;
    }
    public Props GetProp()
    {
        return prop;
    }
    public bool HasProp()
    {
        return prop != null;
    }
    public void NullProp()
    {
        prop = null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
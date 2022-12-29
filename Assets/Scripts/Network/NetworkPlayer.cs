using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public TextMeshProUGUI playerNickName;
    public static NetworkPlayer local { get; set; }
    public Transform playerModel;
    [Networked(OnChanged = nameof(OnNickNameChanged))]
    public NetworkString<_16> nickName { get; set; }
    [Networked(OnChanged = nameof(OnPopUpDialouge))]
    public NetworkString<_16> dialogue { get; set; }
    bool isPublicJoinMessageSent = false;
    NetworkInGameMessages networkInGameMessages;
    public LocalCameraHandler localCameraHandler;
    public GameObject localUI;
    public GameObject chatEntry;
    private TMP_InputField chatInput;
    public TextMeshProUGUI playerDialogue;
    EventSystem eventSystem;
    void Awake()
    {
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        chatInput = chatEntry.GetComponent<TMP_InputField>();
        eventSystem = GetComponentInChildren<EventSystem>();
    }
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            local = this;
            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));
            RPC_SetNickName(PlayerPrefs.GetString("PlayerNickName"));
            Debug.Log("Spawned local player");
        }
        else
        {
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;
            EventSystem eventSystems = GetComponentInChildren<EventSystem>();
            eventSystems.enabled = false;
            localUI.SetActive(false);
            Debug.Log("Spawned remote player");
        }
        transform.name = $"P_{Object.Id}";
    }

    void Update()
    {
        var PlayerNameCanvases = GameObject.FindGameObjectsWithTag("PlayerNameCanvas");
        if (PlayerNameCanvases.Length > 0 && Camera.current != null)
        {
            foreach (var PlayerNameCanvas in PlayerNameCanvases)
            {
                PlayerNameCanvas.transform.rotation = Quaternion.LookRotation(PlayerNameCanvas.transform.position - Camera.current.transform.position);
            }
        }

        var PlayerDialogue = GameObject.FindGameObjectsWithTag("Dialogue");
        if (PlayerDialogue.Length > 0 && Camera.current != null)
        {
            foreach (var Dialogue in PlayerDialogue)
            {
                Dialogue.transform.rotation = Quaternion.LookRotation(Dialogue.transform.position - Camera.current.transform.position);
            }
        }
        if (Object.HasInputAuthority)
        {
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                chatInput.enabled = true;
                Debug.Log("Tab pressed");
                Debug.Log($"Selected game object: {chatInput.transform.root.gameObject.name}");
                eventSystem.SetSelectedGameObject(chatInput.gameObject, null);
                chatInput.OnPointerClick(new PointerEventData(eventSystem));
            }
            if (Input.GetKeyDown(KeyCode.Return) && chatInput.text != "")
            {
                Debug.Log(chatInput.text);
                playerDialogue.text = dialogue.ToString();
                RPC_Chat(PlayerPrefs.GetString("PlayerNickName"), chatInput.text);
                chatInput.text = "";
                chatInput.enabled = false;
            }
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Object.HasStateAuthority)
        {
            networkInGameMessages.SendInGameMessage(nickName.ToString(), "left the game");
        }
        if (player == Object.InputAuthority)
        {
            Destroy(gameObject);
        }
    }

    static void OnNickNameChanged(Changed<NetworkPlayer> change)
    {
        Debug.Log($"{Time.time} OnNickNameChanged {change.Behaviour.nickName}");
        change.Behaviour.OnNickNameChanged();
    }

    static void OnPopUpDialouge(Changed<NetworkPlayer> change)
    {
        Debug.Log($"{Time.time} OnPopUpMessageChanged {change.Behaviour.dialogue}");
        change.Behaviour.OnPopUpDialouge();
    }

    private void OnNickNameChanged()
    {
        Debug.Log($"NickName changed for player to {nickName} for player {gameObject.name}");
        playerNickName.text = nickName.ToString();
    }

    private void OnPopUpDialouge()
    {
        Debug.Log($"Dialogue changed for player to {dialogue} for player {gameObject.name}");
        playerDialogue.text = dialogue.ToString();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickName(string nickName, RpcInfo info = default)
    {
        Debug.Log($"[RPC] SetNickName {nickName}");
        this.nickName = nickName;
        if (!isPublicJoinMessageSent)
        {
            networkInGameMessages.SendInGameMessage(nickName, "joined the game");
            isPublicJoinMessageSent = true;
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_Chat(string nickName, string chatTxt, RpcInfo info = default)
    {
        Debug.Log($"[RPC] Chat {nickName}: {chatTxt}");
        this.dialogue = chatTxt;
        networkInGameMessages.SendInGameMessage(nickName, chatTxt);
        isPublicJoinMessageSent = true;
    }
}

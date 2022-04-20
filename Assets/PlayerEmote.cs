using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Workbench.ProjectDilemma
{
  public class PlayerEmote : MonoBehaviourPun
  {
    [SerializeField] Animator animator;
    public List<EmoteData> ownedEmotes = new List<EmoteData>();
    public EmoteData emoteSelected;

    public SelectionMenu selectionMenu;
    public CursorManager cursorManager;
    private CanvasGroup canvasGroup;

    private void Start()
    {
      canvasGroup = selectionMenu.GetComponent<CanvasGroup>();
      if (photonView.IsMine)
      {
        foreach (EmoteData emote in ownedEmotes)
        {
          if (emote.Icon)
          {
            var ico = Instantiate(emote.Icon, selectionMenu.transform);
            ico.container = emote;
            if (emote.ico) ico.image.sprite = emote.ico;
          }
        }
      }
    }


    public void Pick()
    {
      canvasGroup.alpha = 1; canvasGroup.blocksRaycasts = true; canvasGroup.interactable = true;
      cursorManager.SetLockMode(CursorLockMode.Confined);
      cursorManager.SetVisibility(true);
    }

    public void Emote()
    {
      canvasGroup.alpha = 0; canvasGroup.blocksRaycasts = false; canvasGroup.interactable = false;
      cursorManager.SetLockMode(CursorLockMode.Locked);
      cursorManager.SetVisibility(false);

      if (selectionMenu.LastSelectedObject != null)
      {
        var container = selectionMenu.LastSelectedObject.GetComponent<SelectionMenuContainer>();
        if (container)
        {
          if (container.container is EmoteData)
          {
            emoteSelected = container.container as EmoteData;
            StartCoroutine(AnimationDisablesInput(emoteSelected.ApproxDuration));
            if (PhotonNetwork.IsConnected)
            {
              int viewID = GameMechanic.Instance.localPlayerSpot.GetComponent<PhotonView>().ViewID;
              RPCManager.Instance.photonView.RPC("RPC_UserEmote", RpcTarget.AllViaServer, viewID, MasterData.Instance.GetAnimationIndex(emoteSelected));
            }
            else
            {
              SYNC_Animation(emoteSelected);
            }
          }
        }
      }
    }

    IEnumerator AnimationDisablesInput(float duration)
    {
      if (!PlayerInputManager.Instance.inputConditions.Contains(InputCondition.EMOTE_PLAYING))
      {
        PlayerInputManager.Instance.inputConditions.Add(InputCondition.EMOTE_PLAYING);
      }
      else
      {
        yield break;
      }
      yield return new WaitForSeconds(duration);
      if (PlayerInputManager.Instance.inputConditions.Contains(InputCondition.EMOTE_PLAYING))
      {
        PlayerInputManager.Instance.inputConditions.Remove(InputCondition.EMOTE_PLAYING);
      }
    }

    #region Called by RPC
    public void SYNC_Animation(EmoteData emote)
    {
      animator.SetTrigger(emote.Parameter);
    }
    #endregion
  }
}

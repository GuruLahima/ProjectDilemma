using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Workbench.ProjectDilemma
{
  public class PlayerEmote : InputBaseComponent
  {
    [SerializeField] Animator animator;
    public List<EmoteData> ownedEmotes = new List<EmoteData>();
    public EmoteData emoteSelected;

    public RadialChooseMenu radialMenu;
    public CursorManager cursorManager;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
      canvasGroup = radialMenu.GetComponent<CanvasGroup>();
    }

    public override void Init()
    {
      foreach (EmoteData emote in ownedEmotes)
      {
        if (emote.Icon)
        {
          var ico = Instantiate(emote.Icon, radialMenu.transform);
          ico.container = emote;
          if (emote.ico) ico.image.sprite = emote.ico;
        }
      }
    }


    public void Pick()
    {
      if (OnCooldown) return;

      ActiveState = true;
      radialMenu.Activate();
      canvasGroup.alpha = 1;
      cursorManager.SetLockMode(CursorLockMode.Confined);
      cursorManager.SetVisibility(true);
    }

    public void Emote()
    {
      if (OnCooldown) return;

      AddCooldown();

      ActiveState = false;
      radialMenu.Deactivate();
      canvasGroup.alpha = 0;
      cursorManager.SetLockMode(CursorLockMode.Locked);
      cursorManager.SetVisibility(false);

      if (radialMenu.LastSelectedObject != null)
      {
        var container = radialMenu.LastSelectedObject.GetComponent<SelectionMenuContainer>();
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
      if (!PlayerInputManager.Instance.inputConditions.Contains(ICKeys.EMOTE_PLAYING))
      {
        PlayerInputManager.Instance.inputConditions.Add(ICKeys.EMOTE_PLAYING);
      }
      else
      {
        yield break;
      }
      yield return new WaitForSeconds(duration);
      if (PlayerInputManager.Instance.inputConditions.Contains(ICKeys.EMOTE_PLAYING))
      {
        PlayerInputManager.Instance.inputConditions.Remove(ICKeys.EMOTE_PLAYING);
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

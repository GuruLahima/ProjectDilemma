using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using GuruLaghima;
using System.Linq;
using UnityEngine.Events;

namespace Workbench.ProjectDilemma
{
  public class EmoteActivator : BaseActivatorComponent
  {
    #region Exposed Private Fields
    [SerializeField] private Animator animator;
    [SerializeField] private RadialChooseMenu radialMenu;
    [SerializeField] private UnityEvent AnimationStart;
    [SerializeField] private UnityEvent AnimationEnd;
    #endregion

    #region Private Fields
    private List<EmoteData> ownedEmotes = new List<EmoteData>();
    private EmoteData emoteSelected;
    #endregion

    #region Public Methods
    public override void Init()
    {
      ownedEmotes = InventoryData.Instance.emotes.Where((obj) => { return obj.Owned && obj.Equipped; }).ToList();
      // foreach (EmoteData emote in ownedEmotes)
      // {
      //   if (emote.Icon)
      //   {
      //     var ico = Instantiate(emote.Icon, radialMenu.transform);
      //     ico.container = emote;
      //     if (emote.ico) ico.image.sprite = emote.ico;
      //   }
      // }
    }
    public void Pick()
    {
      if (OnCooldown) return;


      if (!ActiveState)
      {
        //we call this only once per activation (to avoid unnecessary tasks)
        radialMenu.RegenerateSnapPoints();
      }
      ActiveState = true;
      radialMenu.Activate();
      CursorManager.SetLockMode(CursorLockMode.Confined);
      CursorManager.SetVisibility(false);
    }
    public void Emote()
    {
      if (OnCooldown) return;
      //we are adding cooldown where the animation is played
      // this way we take the duration of the animation
      AddCooldown();

      ActiveState = false;
      radialMenu.Deactivate();
      CursorManager.SetLockMode(CursorLockMode.Locked);
      CursorManager.SetVisibility(false);

      if (radialMenu.LastSelectedObject != null)
      {
        var container = radialMenu.LastSelectedObject.GetComponent<SelectionMenuContainer>();
        if (container)
        {
          if (container.container is EmoteData)
          {
            emoteSelected = container.container as EmoteData;
            //AddCooldown(emoteSelected.ApproxDuration); // <== cooldown is added here
            //StartCoroutine(AnimationDisablesInput(emoteSelected.ApproxDuration));
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
    #endregion

    #region Private Methods
    IEnumerator AnimationDisablesInput(float duration)
    {
      AnimationStart?.Invoke();

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
      AnimationEnd?.Invoke();

    }
    #endregion

    #region Called by RPC
    public void SYNC_Animation(EmoteData emote)
    {
      if (this.GetComponent<PlayerSpot>() == GameMechanic.Instance.localPlayerSpot)
        AnimationStart?.Invoke();

      if (FXManager.Instance)
      {
        foreach (FXWrapper wrap in emote.FXOnCast)
        {
          FXManager.Instance.GenerateFX(wrap, transform);
        }
      }
      animator.SetTrigger(emote.Parameter);
      StartCoroutine(SYNCED_AnimationEnd(emote));

    }

    private IEnumerator SYNCED_AnimationEnd(EmoteData emote)
    {

      yield return new WaitForSeconds(emote.ApproxDuration);

      if (this.GetComponent<PlayerSpot>() == GameMechanic.Instance.localPlayerSpot)
        AnimationEnd?.Invoke();

      if (emote.FXOnEnd.Count > 0)
      {
        if (FXManager.Instance)
        {
          foreach (FXWrapper wrap in emote.FXOnEnd)
          {
            FXManager.Instance.GenerateFX(wrap, transform);
          }
        }
      }
    }
    #endregion
  }
}

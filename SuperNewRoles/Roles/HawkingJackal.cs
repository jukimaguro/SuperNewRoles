using HarmonyLib;
using Hazel;
using SuperNewRoles.Buttons;
using SuperNewRoles.CustomRPC;
using SuperNewRoles.Patches;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SuperNewRoles.Roles
{
    class HawkingJackal
    {
        public static void resetCoolDown()
        {
            HudManagerStartPatch.JackalKillButton.MaxTimer = RoleClass.HawkingJackal.KillCoolDown;
            HudManagerStartPatch.JackalKillButton.Timer = RoleClass.HawkingJackal.KillCoolDown;
        }
        public static void EndMeeting()
        {
            resetCoolDown();
        }
        public static void setPlayerOutline(PlayerControl target, Color color)
        {
            if (target == null || target.MyRend == null) return;

            target.MyRend.material.SetFloat("_Outline", 1f);
            target.MyRend.material.SetColor("_OutlineColor", color);
        }
        public class HawkingJackalFixedPatch
        {
            public static PlayerControl HawkingJackalsetTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false, List<PlayerControl> untargetablePlayers = null, PlayerControl targetingPlayer = null)
            {
                PlayerControl result = null;
                float num = GameOptionsData.KillDistances[Mathf.Clamp(PlayerControl.GameOptions.KillDistance, 0, 2)];
                if (!ShipStatus.Instance) return result;
                if (targetingPlayer == null) targetingPlayer = PlayerControl.LocalPlayer;
                if (targetingPlayer.Data.IsDead || targetingPlayer.inVent) return result;

                if (untargetablePlayers == null)
                {
                    untargetablePlayers = new List<PlayerControl>();
                }

                Vector2 truePosition = targetingPlayer.GetTruePosition();
                Il2CppSystem.Collections.Generic.List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
                for (int i = 0; i < allPlayers.Count; i++)
                {
                    GameData.PlayerInfo playerInfo = allPlayers[i];
                    if (!playerInfo.Disconnected && playerInfo.PlayerId != targetingPlayer.PlayerId && playerInfo.Object.isAlive() && (!RoleClass.HawkingJackal.HawkingJackalPlayer.IsCheckListPlayerControl(playerInfo.Object)))
                    {
                        PlayerControl @object = playerInfo.Object;
                        if (untargetablePlayers.Any(x => x == @object))
                        {
                            // if that player is not targetable: skip check
                            continue;
                        }

                        if (@object && (!@object.inVent || targetPlayersInVents))
                        {
                            Vector2 vector = @object.GetTruePosition() - truePosition;
                            float magnitude = vector.magnitude;
                            if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
                            {
                                result = @object;
                                num = magnitude;
                            }
                        }
                    }
                }
                return result;
            }
        }
        public static void TimerEnd()
        {
            
            if (PlayerControl.LocalPlayer.isRole(CustomRPC.RoleId.HawkingJackal))
            {
                MapBehaviour.Instance.Close();
                HudManager.Instance.KillButton.gameObject.SetActive(true);
                HudManager.Instance.ReportButton.gameObject.SetActive(true);
                HudManager.Instance.SabotageButton.gameObject.SetActive(true);
            }
            
        }
        public class FixedUpdate
        {
            public static void Postfix()
            {
                if (RoleClass.HawkingJackal.Timer >= 0.1 && !RoleClass.IsMeeting)
                {
                    Camera.main.orthographicSize = RoleClass.HawkingJackal.CameraDefault * 3f;
                    HudManager.Instance.UICamera.orthographicSize = RoleClass.HawkingJackal.Default * 3f;

                }
                else
                {
                    Camera.main.orthographicSize = RoleClass.HawkingJackal.CameraDefault;
                    HudManager.Instance.UICamera.orthographicSize = RoleClass.HawkingJackal.Default;
                }
                if (RoleClass.HawkingJackal.timer1 >= 0.1 && !RoleClass.IsMeeting)
                {
                    var TimeSpanDate = new TimeSpan(0, 0, 0, (int)10);
                    RoleClass.HawkingJackal.timer1 = (float)((Roles.RoleClass.HawkingJackal.Timer2 + TimeSpanDate) - DateTime.Now).TotalSeconds;
                    PlayerControl.LocalPlayer.transform.localPosition = RoleClass.HawkingJackal.Postion;
                    SuperNewRolesPlugin.Logger.LogInfo(RoleClass.HawkingJackal.timer1);
                }
            }
        }

    }
 }
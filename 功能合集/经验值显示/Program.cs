﻿using System;
using LeagueSharp;
using LeagueSharp.Common;
using System.Collections.Generic;
using System.Diagnostics;
using SharpDX;

using Color = System.Drawing.Color;

namespace RoyalAssistant
{
    class Program
    {
        static Menu menu;
        static int[] SRExpCumulative = { 0, 280, 660, 1140, 1720, 2400, 3180, 4060, 5040, 6120, 7300, 8580, 9960, 11440, 13020, 14700, 16480, 18360 };
        static bool bought = false;
        static System.Timers.Timer globalCooldown = new System.Timers.Timer();

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        static void OnGameLoad(EventArgs args)
        {
            LoadMenu();
            
            if (Game.MapId != (GameMapId)11)
            {
                Game.PrintChat("RoyalAssistant: only SR support implemented!");
				return;
            }

            Game.OnGameUpdate += OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameEnd += OnGameEnd;
            globalCooldown.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerProcs);
            Game.PrintChat("RoyalAssistant Loaded! By Doramio");
        }

        static void OnUpdate(EventArgs args)
        {
            if (Utility.InShopRange() && menu.Item("ward").GetValue<bool>() && !HasWard())
                if (menu.Item("buyward").GetValue<KeyBind>().Active && !bought)
                {
                    ObjectManager.Player.BuyItem(ItemId.Stealth_Ward);
                    bought = true;
                }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (Utility.InShopRange() && menu.Item("ward").GetValue<bool>() && !HasWard())
            {
                Drawing.DrawText(menu.Item("center").GetValue<bool>() ? Drawing.Width / 2 - 40 : 200,
                                  menu.Item("center").GetValue<bool>() ? Drawing.Height / 2 - 60 : 400, GetColor(),
                                  "愚蠢的人类,买眼可以拯救你! (" + (char)menu.Item("buyward").GetValue<KeyBind>().Key + ")");
                if(bought) Utility.DelayAction.Add(10, () => bought = false);
            }

            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
                if (hero.Level != 18 && hero.IsVisible && !hero.IsDead)
                {
                    int XOffset;
                    int YOffset;
                    int textXOffset;
                    int textYOffset;
                    int width;
                    if (hero.IsMe && menu.Item("showSelf").GetValue<bool>())
                    {
                        XOffset = 8;
                        YOffset = 2;
                        width = 132;
                        textXOffset = 6;
                        textYOffset = -14;
                    }
                    else if (hero.IsAlly && !hero.IsMe && menu.Item("showAllies").GetValue<bool>() || hero.IsEnemy && menu.Item("showEnemies").GetValue<bool>())
                    {
                        if ((hero.IsAlly && menu.Item("showAlliesT").GetValue<bool>() || hero.IsEnemy && menu.Item("showEnemiesT").GetValue<bool>()))
                        {
                            XOffset = 10;
                            YOffset = 42;
                            width = 130;
                            textXOffset = 6;
                            textYOffset = 3;
                        }
                        else
                        {
                            XOffset = 8;
                            YOffset = 13;
                            width = 132;
                            textXOffset = 6;
                            textYOffset = -2;
                        }
						if(hero.IsAlly)
						{
							YOffset -= 2;
							textYOffset -= 2;
						}
                    }
                    else return;
                    Drawing.DrawLine(
                        new Vector2(hero.HPBarPosition.X + XOffset, hero.HPBarPosition.Y + YOffset),
                        new Vector2(hero.HPBarPosition.X + XOffset + width * ((180 + 100 * hero.Level + hero.Experience - SRExpCumulative[hero.Level]) / (180 + 100 * hero.Level)),
                            hero.HPBarPosition.Y + YOffset), 3, Color.Gold);
                    if (menu.Item("text").GetValue<bool>()) Drawing.DrawText(hero.HPBarPosition.X + textXOffset, hero.HPBarPosition.Y + textYOffset, Color.PaleGoldenrod, (int)(180 + 100 * hero.Level + hero.Experience - SRExpCumulative[hero.Level]) + "/" + (180 + 100 * hero.Level));
                }

        }

        static void OnGameEnd(EventArgs args)
        {
            //Cause utility won't work after game ends ( -_-)
            globalCooldown.Interval = menu.Item("delay").GetValue<Slider>().Value;
            globalCooldown.Start();
        }

        static void OnTimerProcs(object sender, System.Timers.ElapsedEventArgs e)
        {
            globalCooldown.Stop();
            globalCooldown.Dispose();
            Process.GetProcessesByName("League of Legends")[0].Kill();
        }
       

        static bool HasWard()
        {
            foreach (InventorySlot slot in ObjectManager.Player.InventoryItems)
                if (slot.Name.ToLower().Contains("ward") && !slot.Name.ToLower().Contains("trinket"))
                    return true;
            return false;
        }
        
        static Color GetColor()
        {
            switch ((int)((Game.Time % 1) * 10))//SHITTY CODE! :D
            {
                case 0:
                    return Color.IndianRed;
                    break;
                case 1:
                    return Color.LightGoldenrodYellow;
                    break;
                case 2:
                    return Color.Goldenrod;
                    break;
                case 3:
                    return Color.Green;
                    break;
                case 4:
                    return Color.Blue;
                    break;
                case 5:
                    return Color.Violet;
                    break;
                case 6:
                    return Color.DeepPink;
                    break;
                case 7:
                    return Color.DeepSkyBlue;
                    break;
                case 8:
                    return Color.White;
                    break;
                case 9:
                    return Color.Cyan;
                    break;
                default:
                    return Color.ForestGreen;
                    break;
            }
            return Color.Black;
        }

        static void LoadMenu()
        {
            // Initialize the menu
            menu = new Menu("RoyalAssistant", "Lv.⑨", true);
            menu.AddItem(new MenuItem("showSelf", "显示自己经验").SetValue(false));
            menu.AddItem(new MenuItem("showAllies", "显示队友经验").SetValue(true));
            menu.AddItem(new MenuItem("showEnemies", "显示敌人经验").SetValue(true));
            menu.AddItem(new MenuItem("text", "绘制经验值计数").SetValue(true));
            menu.AddItem(new MenuItem("1", "                  检测设置:"));
            //menu.AddItem(new MenuItem("showSelfT", "Tracker showing self").SetValue(false));
            menu.AddItem(new MenuItem("showAlliesT", "队友显示检测").SetValue(true));
            menu.AddItem(new MenuItem("showEnemiesT", "敌人显示检测").SetValue(true));
            menu.AddItem(new MenuItem("2", "                        助手:"));
            menu.AddItem(new MenuItem("end", "退出游戏关闭").SetValue(true));
            menu.AddItem(new MenuItem("delay", "自定义延迟关闭 LoL").SetValue(new Slider(500, 0, 1500)));
            menu.AddItem(new MenuItem("ward", "显示 \"买眼\" 提醒").SetValue(true));
            menu.AddItem(new MenuItem("center", "^ 把提示放在中间 ^").SetValue(false));
            menu.AddItem(new MenuItem("buyward", "自动购买眼睛").SetValue(new KeyBind('U', KeyBindType.Press)));
            menu.AddToMainMenu();
            Console.WriteLine("Menu finalized");
        }
    }
}

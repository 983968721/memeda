using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace TAC_Kalista
{
    class MenuHandler
    {
        public static Menu Config;
        internal static Orbwalking.Orbwalker orb;
        public static void init()
        {
            Config = new Menu("复仇之矛", "Kalista", true);

            var targetselectormenu = new Menu("目标选择", "Common_TargetSelector");
            SimpleTs.AddToMenu(targetselectormenu);
            Config.AddSubMenu(targetselectormenu);

            Menu orbwalker = new Menu("走砍", "orbwalker");
            orb = new Orbwalking.Orbwalker(orbwalker);
            Config.AddSubMenu(orbwalker);

            Config.AddSubMenu(new Menu("连招", "ac"));
            
            Config.SubMenu("ac").AddSubMenu(new Menu("技能使用","skillUsage"));
            Config.SubMenu("ac").SubMenu("skillUsage").AddItem(new MenuItem("UseQAC", "使用 Q").SetValue(true));
            Config.SubMenu("ac").SubMenu("skillUsage").AddItem(new MenuItem("UseEAC", "使用 E").SetValue(true));
            
            Config.SubMenu("ac").AddSubMenu(new Menu("技能设置","skillConfiguration"));
            Config.SubMenu("ac").SubMenu("skillConfiguration").AddItem(new MenuItem("UseQACM", "用Q时范围 ").SetValue(new StringList(new[] { "Far", "Medium", "Close" }, 2)));
            Config.SubMenu("ac").SubMenu("skillConfiguration").AddItem(new MenuItem("E4K", "使用E抢人头").SetValue(true));
            Config.SubMenu("ac").SubMenu("skillConfiguration").AddItem(new MenuItem("UseEACSlow", "使用E减缓目标").SetValue(false));
            Config.SubMenu("ac").SubMenu("skillConfiguration").AddItem(new MenuItem("UseEACSlowT", "缓慢的目标，如果敌人< =").SetValue(new Slider(1, 1, 5)));
            Config.SubMenu("ac").SubMenu("skillConfiguration").AddItem(new MenuItem("minE", "当被动叠到X层时使用E技能").SetValue(new Slider(1, 1, 20)));
            Config.SubMenu("ac").SubMenu("skillConfiguration").AddItem(new MenuItem("minEE", "打开E技能在X层被动").SetValue(false));
            
            Config.SubMenu("ac").AddSubMenu(new Menu("项目设置","itemsAC"));
            Config.SubMenu("ac").SubMenu("itemsAC").AddItem(new MenuItem("useItems", "使用物品").SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Toggle)));

            Config.SubMenu("ac").SubMenu("itemsAC").AddItem(new MenuItem("allIn", "在所有模式下").SetValue(new KeyBind("U".ToCharArray()[0], KeyBindType.Toggle)));
//            Config.SubMenu("ac").SubMenu("itemsAC").AddItem(new MenuItem("allInAt", "Auto All in when X hero").SetValue(new Slider(2, 1, 5)));
            
            Config.SubMenu("ac").SubMenu("itemsAC").AddItem(new MenuItem("BOTRK", "使用破败").SetValue(true));
            Config.SubMenu("ac").SubMenu("itemsAC").AddItem(new MenuItem("GHOSTBLADE", "使用鬼刀").SetValue(true));
            Config.SubMenu("ac").SubMenu("itemsAC").AddItem(new MenuItem("SWORD", "使用神圣之剑").SetValue(true));

            Config.SubMenu("ac").SubMenu("itemsAC").AddSubMenu(new Menu("自动解控", "QSS"));
            Config.SubMenu("ac").SubMenu("itemsAC").SubMenu("QSS").AddItem(new MenuItem("AnyStun", "眩晕").SetValue(true));
            Config.SubMenu("ac").SubMenu("itemsAC").SubMenu("QSS").AddItem(new MenuItem("AnySnare", "陷阱").SetValue(true));
            Config.SubMenu("ac").SubMenu("itemsAC").SubMenu("QSS").AddItem(new MenuItem("AnyTaunt", "嘲讽").SetValue(true));
            foreach (var t in ItemHandler.BuffList)
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.IsEnemy))
                {
                    if (t.ChampionName == enemy.ChampionName)
                        Config.SubMenu("ac").SubMenu("itemsAC").SubMenu("QSS").AddItem(new MenuItem(t.BuffName, t.DisplayName).SetValue(t.DefaultValue));
                }
            }

            Config.AddSubMenu(new Menu("杂项", "misc"));
            Config.SubMenu("misc").AddItem(new MenuItem("saveSould", "保存灵魂绑定").SetValue(true));
            Config.SubMenu("misc").AddItem(new MenuItem("soulHP", "拯救灵魂的HP ％").SetValue(new Slider(15,1,100)));
            Config.SubMenu("misc").AddItem(new MenuItem("soulEnemyCount", "和X周围的敌人").SetValue(new Slider(3, 1, 5)));
            Config.SubMenu("misc").AddItem(new MenuItem("antiGap", "抗缝隙关闭").SetValue(false));
            Config.SubMenu("misc").AddItem(new MenuItem("antiGapRange", "防突进范围").SetValue(new Slider(300, 300, 400)));
            Config.SubMenu("misc").AddItem(new MenuItem("antiGapPrevent", "防止反差距组合模式").SetValue(true));

            Config.AddSubMenu(new Menu("骚扰设置", "harass"));
            Config.SubMenu("harass").AddItem(new MenuItem("harassQ", "使用Q").SetValue(true));
            Config.SubMenu("harass").AddItem(new MenuItem("stackE", "使用e").SetValue(new Slider(1, 1, 10)));
            Config.SubMenu("harass").AddItem(new MenuItem("manaPercent", "法力％").SetValue(new Slider(40, 1, 100)));

            Config.AddSubMenu(new Menu("清兵", "wc"));
            Config.SubMenu("wc").AddItem(new MenuItem("wcQ", "使用 Q").SetValue(true));
            Config.SubMenu("wc").AddItem(new MenuItem("wcE", "使用 E").SetValue(true));
            Config.SubMenu("wc").AddItem(new MenuItem("enableClear", "利用E被动快速清线").SetValue(false));
            
            Config.AddSubMenu(new Menu("惩戒设置", "smite"));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Baron", "大龙").SetValue(true));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Dragon", "小龙").SetValue(true));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Gromp", "Gromp Enabled").SetValue(false));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Murkwolf", "Murkwolf Enabled").SetValue(false));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Krug", "Krug Enabled").SetValue(false));
            Config.SubMenu("smite").AddItem(new MenuItem("SRU_Razorbeak", "Razorbeak Enabled").SetValue(false));
            Config.SubMenu("smite").AddItem(new MenuItem("Sru_Crab", "Crab Enabled").SetValue(false));
            Config.SubMenu("smite").AddItem(new MenuItem("smite", "自动启用惩戒").SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Toggle)));

            Config.AddSubMenu(new Menu("墙跳设置", "wh"));
            Config.SubMenu("wh").AddItem(new MenuItem("JumpTo", "墙跳按键 (HOLD)").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            Config.AddSubMenu(new Menu("显示范围", "Drawings"));
            Config.SubMenu("Drawings").AddSubMenu(new Menu("Ranges", "range"));

            Config.SubMenu("Drawings").SubMenu("range").AddItem(new MenuItem("QRange", "Q range").SetValue(new Circle(true, Color.FromArgb(100, Color.Red))));
            Config.SubMenu("Drawings").SubMenu("range").AddItem(new MenuItem("WRange", "W range").SetValue(new Circle(false, Color.FromArgb(100, Color.Coral))));
            Config.SubMenu("Drawings").SubMenu("range").AddItem(new MenuItem("ERange", "E range").SetValue(new Circle(true, Color.FromArgb(100, Color.BlueViolet))));
            Config.SubMenu("Drawings").SubMenu("range").AddItem(new MenuItem("drawESlow", "E slow range").SetValue(true));
            Config.SubMenu("Drawings").SubMenu("range").AddItem(new MenuItem("RRange", "R range").SetValue(new Circle(false, Color.FromArgb(100, Color.Blue))));
            Config.SubMenu("Drawings").AddItem(new MenuItem("drawHp", "绘制可造成伤害在目标血量条上")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("drawStacks", "绘制总被动层数")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("enableDrawings", "Enable all drawings").SetValue(true));          

            Config.AddItem(new MenuItem("Packets", "封包").SetValue(true));

            Config.AddItem(new MenuItem("debug", "调试").SetValue(false));
            
            Config.AddToMainMenu();

        }
    }
}

using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod.Menu;
using UnityEngine;

namespace AnotherLocation.Rando
{
    public class ConnectionMenu
    {
        internal static ConnectionMenu Instance { get; private set; }

        private readonly SmallButton pageRootButton;
        private readonly MenuPage accessPage;
        private readonly MenuElementFactory<RandoSettings> topLevelFactory;

        public static void Hook()
        {
            RandomizerMenuAPI.AddMenuPage(ConstructMenu, HandleButton);
            MenuChangerMod.OnExitMainMenu += () => Instance = null;
        }

        private static bool HandleButton(MenuPage landingPage, out SmallButton button)
        {
            button = Instance.pageRootButton;
            button.Text.color = AnotherLocation.GlobalSettings.BugPrince
                ? Colors.TRUE_COLOR
                : Colors.DEFAULT_COLOR;
            return true;
        }

        private static void ConstructMenu(MenuPage landingPage)
        {
            Instance = new ConnectionMenu(landingPage);
        }

        private ConnectionMenu(MenuPage landingPage)
        {
            accessPage = new MenuPage("Another Location", landingPage);
            topLevelFactory = new MenuElementFactory<RandoSettings>(accessPage, RandoMenuProxy.RS);

            _ = new VerticalItemPanel(
                accessPage,
                new Vector2(0, 200),
                48f,
                true,
                topLevelFactory.Elements
            );

            pageRootButton = new SmallButton(landingPage, "Another Location");
            pageRootButton.AddHideAndShowEvent(landingPage, accessPage);

            landingPage.BeforeShow += () =>
            {
                pageRootButton.Text.color = AnotherLocation.GlobalSettings.BugPrince
                    ? Colors.TRUE_COLOR
                    : Colors.DEFAULT_COLOR;
            };

            accessPage.BeforeHide += () =>
            {
                var settings = new RandoSettings
                {
                    BugPrince = (bool)topLevelFactory.ElementLookup[nameof(RandoSettings.BugPrince)].Value
                };

                RandoMenuProxy.RS = settings;
                topLevelFactory.SetMenuValues(settings);
                AnotherLocation.GlobalSettings.BugPrince = settings.BugPrince;
            };
        }

        public void Apply(RandoSettings settings)
        {
            topLevelFactory.SetMenuValues(settings);
        }

        public void Disable()
        { //changes the "Bug Prince"(only) setting,but not a global "enable" value(abscent here)
            var elem = topLevelFactory.ElementLookup[nameof(RandoSettings.BugPrince)];
            elem.SetValue(false);
        }
    }
}
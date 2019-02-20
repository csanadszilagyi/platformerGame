using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using platformerGame.Utilities;

namespace platformerGame.App
{
    abstract class BaseGuiItem
    {
        public delegate void clickCallBack(MouseButtonEventArgs e);

        public clickCallBack OnClick;

        protected GameState parent = null;

        public AABB Bounds { get; set; }
        public bool Active { get; set; }
        public bool MouseHover { get; set; }
        protected RenderStates rstates;

        public BaseGuiItem(GameState parent, AABB bounds)
        {
            this.parent = parent;
            this.Bounds = bounds;
            this.OnClick = null;
            this.Active = false;
            this.MouseHover = false;
            this.rstates = new RenderStates(BlendMode.Alpha);
        }

        public GameState Parent
        {
            get { return this.parent; }
        }

        public void Click(MouseButtonEventArgs e)
        {
            this.OnClick?.Invoke(e);
        }

        abstract public void Update(float step_time);
        abstract public void Render(RenderTarget destination);
    }

    class Button : BaseGuiItem
    {
        
        public string Title { get; set; }

        RectangleShape rectShape;
        Text label;
        Vector2f labelOrigPos = new Vector2f();

        const uint FONT_SIZE = 16;

        Color fillColor = Color.Transparent;
        Color textColor = Color.White;

        public Button(GameState parent, AABB bounds, string title) : base(parent, bounds)
        {
            this.Title = title;
            this.rectShape = new RectangleShape();
            rectShape.Position = Bounds.topLeft;
            rectShape.Size = Bounds.dims;
            rectShape.OutlineColor = Color.White;
            rectShape.OutlineThickness = 2.0f;
            rectShape.FillColor = fillColor;

            label = new Text(this.Title, AssetManager.GetFont("pf_tempesta_seven"), FONT_SIZE);
            labelOrigPos.X = Bounds.center.X - (float)(label.GetGlobalBounds().Width / 2.0f);
            labelOrigPos.Y = Bounds.center.Y - (float)(FONT_SIZE / 2.0f);
            label.FillColor = textColor;

            label.Position = new Vector2f(labelOrigPos.X, labelOrigPos.Y);
        }



        public override void Render(RenderTarget destination)
        {
            this.rectShape.FillColor = this.fillColor;
            this.label.FillColor = textColor;

            destination.Draw(this.rectShape, this.rstates);
            destination.Draw(this.label);
        }

        public override void Update(float step_time)
        {
            
            this.MouseHover = cCollision.IsPointInsideBox(parent.GetMousePos(), Bounds);
            if(MouseHover)
            {
                this.fillColor = Color.White;
                this.textColor = Color.Black;
            }
            else
            {
                this.fillColor = Color.Black;
                this.textColor = Color.White;
            }
        }
    }

    class MenuScreen
    {
        protected List<BaseGuiItem> guiItems;

        public MenuScreen()
        {
            this.guiItems = new List<BaseGuiItem>();
        }

        public void Add(BaseGuiItem[] items)
        {
            this.guiItems.AddRange(items);
        }

        public void Remove(BaseGuiItem item)
        {
            this.guiItems.Remove(item);
        }

        public void Update(float step_time)
        {
            foreach(var item in this.guiItems)
            {
                item.Update(step_time);
            }
        }

        public void Render(RenderTarget destination)
        {
            foreach (var item in this.guiItems)
            {
                item.Render(destination);
            }
        }

        public List<BaseGuiItem> Items
        {
            get { return this.guiItems; }
        }
    }

    class MainMenu : GameState
    {
        Dictionary<string, MenuScreen> menus;
        MenuScreen currentMenu = null;

        public MainMenu(SfmlApp app_ref) : base(app_ref)
        {
            this.menus = new Dictionary<string, MenuScreen>()
            {
                {"home", new MenuScreen() }
            };

            // this.menus.Add("home", new MenuScreen());
        }

        
        public void connectItems(string menu, BaseGuiItem[] items)
        {

            MenuScreen ms;

            System.Diagnostics.Debug.WriteLine(string.Format("if előtt"));

            if (menus.TryGetValue(menu, out ms))
            {
                System.Diagnostics.Debug.WriteLine(string.Format("if-ben"));
                ms.Add(items);
                return;
            }

            System.Diagnostics.Debug.WriteLine(string.Format("if után - no menu found"));
            
            ms = new MenuScreen();
            ms.Add(items);
            menus.Add(menu, ms);
        }

        public void SwitchMenu(string name)
        {
            /*
            if(name == "back")
            {
                return;
            }
            */

            MenuScreen s;
            if(menus.TryGetValue(name, out s))
            {
                this.currentMenu = s;
            }
        }

        public void Create()
        {
            var b = this.camera.Bounds;
            int buttonWidth = 180;
            int buttonHeight = 40;

            // centering the button
            float left = b.halfDims.X - buttonWidth / 2.0f;


            var thisState = this;

            System.Diagnostics.Debug.WriteLine(string.Format("has-home (before try): {0}", menus.ContainsKey("home")));
            // valami nem ok itt, amikor visszalépünk főmenübe lefegy a játék...
            try
            {
                this.connectItems("home", new Button[2] {
                new Button(thisState, new AABB(left, 400, buttonWidth, buttonHeight), "Play") {

                               OnClick = (MouseButtonEventArgs e) =>
                               {
                                   // this.SwitchMenu("options");
                                   appControllerRef.StartGame();
                               }
                },
                new Button(thisState, new AABB(left, 480, buttonWidth, buttonHeight), "Exit") {

                               OnClick = (MouseButtonEventArgs e) =>
                               {
                                   Exit();
                                   appControllerRef.CloseApp();
                               }
                }

            });
            }
            catch(Exception e)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(string.Format("has home (in exception): {0}", menus.ContainsKey("home")));
                System.Diagnostics.Debug.WriteLine(e.Message + e.Source + e.StackTrace);

#endif
                Exit();
                appControllerRef.CloseApp();
            }

            /*
            this.connectItems("options", new[] {
                new Button() { Title = "ok" },
                new Button() { Title = "back" }

            });

            this.connectItems("credits", new[] {
                new Button() { Title = "ok" },
                new Button() { Title = "back" }

            });
            */
        }

        public override void Enter()
        {
            camera = new Camera(new View(new Vector2f(appControllerRef.WindowSize.X / 2.0f, appControllerRef.WindowSize.Y / 2.0f), appControllerRef.WindowSize));
            camera.Zoom = 1.0f;
            this.Create();
            this.SwitchMenu("home");
        }

        public override void Exit()
        {
            this.menus.Clear();
        }

        public override void HandleKeyPress(KeyEventArgs e)
        {
            
        }

        public override void HandleSingleMouseClick(MouseButtonEventArgs e)
        {
            var buttons = currentMenu.Items.OfType<Button>();
            Vector2f mousePos = new Vector2f(e.X, e.Y);
            foreach (var button in buttons)
            {
                if (cCollision.IsPointInsideBox(mousePos, button.Bounds))
                {
                    button.Click(e);
                    return;
                }
            }
        }

        public override void UpdateFixed(float step_time)
        {
            currentMenu?.Update(step_time);
        }

        public override void UpdateVariable(float step_time = 1)
        {

        }

        public override void Render(RenderTarget destination, float alpha)
        {
            camera.DeployOn(destination, alpha);
            currentMenu?.Render(destination);
        }

       
    }
}

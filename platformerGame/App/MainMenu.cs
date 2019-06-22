using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using platformerGame.Utilities;

using platformerGame.GameObjects;
using platformerGame.Containers;

namespace platformerGame.App
{
    abstract class BaseGuiItem : GridOccupant
    {
        public delegate void clickCallBack(MouseButtonEventArgs e);

        public clickCallBack OnClickFunc;

        protected GameState parent = null;

        public bool Active { get; set; }
        public bool MouseMoved { get; set; }
        public bool MouseClicked { get; set; }

        protected RenderStates renderStates;

        public BaseGuiItem(GameState parent, AABB bounds) : base()
        {
            this.parent = parent;
            this.Bounds = bounds;
            this.OnClickFunc = null;
            this.Active = true;
            this.renderStates = new RenderStates(BlendMode.Alpha);

            this.GridPosition = EntityGrid<BaseGuiItem>.calcGridPos(this.Bounds.center);

            this.Reset();
        }

        public virtual void Reset()
        {
            this.MouseMoved = false;
            this.MouseClicked = false;
        }

        public GameState Parent
        {
            get { return this.parent; }
        }

        public virtual void StartClick(MouseButtonEventArgs e)
        {
            this.MouseClicked = true;
        }

        // The real "Click" function only occurs when released the button, but effect starts when button was pressed.
        public virtual void OnClick(MouseButtonEventArgs e)
        {
            this.OnClickFunc?.Invoke(e);
            this.MouseClicked = false;
        }

        public abstract void OnMouseMove(MouseMoveEventArgs e);
        public abstract void OnMouseLeave(MouseMoveEventArgs e);

        public override bool isActive()
        {
            return this.Active;
        }

        abstract public void Render(RenderTarget destination);

        public virtual bool IsReady()
        {
            return this.MouseClicked == false;
        }
    }

    class CheckBox : BaseGuiItem
    {
        public bool Checked { get; set; }

        public CheckBox(GameState parent, AABB bounds, string title) : base(parent, bounds)
        {
            Checked = false;
        }

        public override void Reset()
        {
            base.Reset();
 

        }


        public override void StartClick(MouseButtonEventArgs e)
        {
            base.StartClick(e);
            
        }

        public override void OnClick(MouseButtonEventArgs e)
        {
            base.OnClick(e);
        }

        public override void OnMouseMove(MouseMoveEventArgs e)
        {
            this.MouseMoved = true;
        }

        public override void OnMouseLeave(MouseMoveEventArgs e)
        {
            if (this.MouseMoved)
            {
                if (this.MouseClicked)
                {

                    this.MouseClicked = false;
                }

                this.MouseMoved = false;
            }
        }

        public override void Render(RenderTarget destination)
        {
            /*
            this.rectShape.FillColor = currentFillColor;
            this.label.FillColor = currentTextColor;

            destination.Draw(this.rectShape, this.renderStates);
            destination.Draw(this.label);
            */
        }

        public override void Update(float step_time)
        {

        }

    }

    class Button : BaseGuiItem
    {
        const int TEXT_OFFSET = 2;

        public string Title { get; set; }

        RectangleShape rectShape;
        protected Text label;
        Vector2f labelOrigPos = new Vector2f();

        const uint FONT_SIZE = 16;

        readonly Color FILL_COLOR = Utils.BLUE;
        readonly Color TEXT_COLOR = Color.White;

        Color currentFillColor;
        Color currentTextColor;

        float alpha = 255.0f;

        public Button(GameState parent, AABB bounds, string title) : base(parent, bounds)
        {
            currentFillColor = Color.Transparent;
            currentTextColor = TEXT_COLOR;

            Title = title;
            rectShape = new RectangleShape();
            rectShape.Position = Bounds.topLeft;
            rectShape.Size = Bounds.dims;
            rectShape.OutlineColor = FILL_COLOR;
            rectShape.OutlineThickness = 2.0f;
            rectShape.FillColor = currentFillColor;

            label = new Text(this.Title, parent.Assets.GetFont("pf_tempesta_seven"), FONT_SIZE);
            labelOrigPos.X = Bounds.center.X - (float)(label.GetGlobalBounds().Width / 2.0f);
            labelOrigPos.Y = Bounds.center.Y - (float)(FONT_SIZE / 2.0f);
            label.FillColor = currentTextColor;

            this.resetTextposition();
        }

        public override void Reset()
        {
            base.Reset();
            this.currentFillColor = Color.Transparent;

        }
        private void resetTextposition()
        {
            label.Position = new Vector2f(labelOrigPos.X, labelOrigPos.Y);
        }

        public override void StartClick(MouseButtonEventArgs e)
        {
            base.StartClick(e);
            this.label.Position = new Vector2f(label.Position.X, label.Position.Y + TEXT_OFFSET);
        }

        public override void OnClick(MouseButtonEventArgs e)
        {
            base.OnClick(e);
            this.resetTextposition();
        }

        public override void OnMouseMove(MouseMoveEventArgs e)
        {
            this.MouseMoved = true;
            this.currentFillColor = FILL_COLOR;
        }

        public override void OnMouseLeave(MouseMoveEventArgs e)
        {
            if(this.MouseMoved)
            {
                this.currentFillColor = Color.Transparent;

                if(this.MouseClicked)
                {
                    this.resetTextposition();
                    this.MouseClicked = false;
                }
                
                this.MouseMoved = false;
            }
        }

        public override void Render(RenderTarget destination)
        {
            this.rectShape.FillColor = currentFillColor;
            this.label.FillColor = currentTextColor;

            destination.Draw(this.rectShape, this.renderStates);
            destination.Draw(this.label);
        }

        public override void Update(float step_time)
        {
            
        }


    }

    class MenuScreen
    {
        // protected List<BaseGuiItem> guiItems;
        AABB ViewBounds { get; set; }
        protected EntityGrid<BaseGuiItem> guiItemsGrid;

        public MenuScreen(AABB view_bounds)
        {
            ViewBounds = view_bounds;
            
            this.guiItemsGrid = new EntityGrid<BaseGuiItem>(ViewBounds);
            // System.Diagnostics.Debug.WriteLine(ViewBounds.dims.X);
        }

        public void Add(BaseGuiItem item)
        {
            this.guiItemsGrid.AddEntity(item);
        }

        public void Add(BaseGuiItem[] items)
        {
            this.guiItemsGrid.AddEntities(items);
        }

        public void Remove(BaseGuiItem item)
        {
            this.guiItemsGrid.RemoveEntity(item);
        }

        public void Update(float step_time)
        {
            this.guiItemsGrid.Update(step_time);
        }

        public void ClearItems()
        {
            this.guiItemsGrid.RemoveAll();
        }

        public void Render(RenderTarget destination)
        {
            var visibles = this.guiItemsGrid.filterVisibles(this.ViewBounds).ToList();
            foreach (var item in visibles)
            {
                item.Render(destination);
            }
        }

        public void ResetItems()
        {
            this.guiItemsGrid.GetAllEntities().ForEach(item =>
            {
                item.Reset();
            });
        }
        
        public EntityGrid<BaseGuiItem> ItemGrid
        {
            get { return this.guiItemsGrid; }
        }
    }

    class MainMenu : GameState
    {
        Dictionary<string, MenuScreen> menus;
        MenuScreen currentMenu = null;
  
        public MainMenu(SfmlApp app_ref) : base(app_ref)
        {
            menus = new Dictionary<string, MenuScreen>();
        }

        public void connectItems(string menu_name, BaseGuiItem[] items)
        {

            MenuScreen ms;

            System.Diagnostics.Debug.WriteLine("CONNECT-ITEMS entered");

            

            
            if (menus.TryGetValue(menu_name, out ms))
            {
                System.Diagnostics.Debug.WriteLine(string.Format("IN IF - menuscreen: {0}", ms));
                ms?.Add(items);

                // if(ms == null) while (true) { }

                return;
            }
            

            /*
            catch (Exception ex)
            {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(string.Format("has home (in exception): {0}", menus.ContainsKey("home")));
                    System.Diagnostics.Debug.WriteLine(ex.Message + ex.Source + ex.StackTrace);

#endif
                //throw ex;
                while (true) { }
                //Exit();
                //appControllerRef.CloseApp();
            }
*/
            System.Diagnostics.Debug.WriteLine("if után - no menu found");
            
            ms = new MenuScreen(this.appControllerRef.WindowArea);
            ms.Add(items);
            menus.Add(menu_name, ms);
        }

        public void SwitchMenu(string name)
        {

            MenuScreen s = null;
            if(menus.TryGetValue(name, out s))
            {
                this.currentMenu?.ResetItems();
                this.currentMenu = s;
            }
        }

        public void Create()
        {
            // menus = new Dictionary<string, MenuScreen>();
            System.Diagnostics.Debug.WriteLine("ADD előtt");
            this.menus.Add("home", new MenuScreen(this.appControllerRef.WindowArea));
            this.menus.Add("options", new MenuScreen(this.appControllerRef.WindowArea));
            System.Diagnostics.Debug.WriteLine("ADD után");

            var screenBounds = this.camera.Bounds;
            int buttonWidth = 180;
            int buttonHeight = 40;

            // centering the button
            float left = screenBounds.halfDims.X - buttonWidth / 2.0f;

            // System.Diagnostics.Debug.WriteLine(string.Format("has-home (before try): {0}", menus.ContainsKey("home")));
            // valami nem ok itt, amikor visszalépünk főmenübe lefagy a játék...

            System.Diagnostics.Debug.WriteLine("CONNECT - options előtt");
            this.connectItems("options", new Button[1] {
                new Button(this, new AABB(left, 400, buttonWidth, buttonHeight), "Back") {

                                    OnClickFunc = (MouseButtonEventArgs e) =>
                                    {
                                        this.SwitchMenu("home");
                                    }
                 }
            });

            System.Diagnostics.Debug.WriteLine("CONNECT - home előtt");

            this.connectItems("home", new Button[3] {
                    new Button(this, new AABB(left, 400, buttonWidth, buttonHeight), "Play") {

                                    OnClickFunc = (MouseButtonEventArgs e) =>
                                    {
                                        appControllerRef.StartGame();
                                    }
                    },
                    new Button(this, new AABB(left, 460, buttonWidth, buttonHeight), "Options") {

                                    OnClickFunc = (MouseButtonEventArgs e) =>
                                    {
                                        this.SwitchMenu("options");
                                    }
                    },
                    new Button(this, new AABB(left, 520, buttonWidth, buttonHeight), "Exit") {

                                    OnClickFunc = (MouseButtonEventArgs e) =>
                                    {
                                        Exit();
                                        appControllerRef.CloseApp();
                                    }
                    }
                }
            );
            System.Diagnostics.Debug.WriteLine("CONNECT után");

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
            resourceAssets.LoadResources(Constants.FONT_NAMES);
            camera = new Camera(new View(new Vector2f(appControllerRef.WindowSize.X / 2.0f, appControllerRef.WindowSize.Y / 2.0f), appControllerRef.WindowSize));
            camera.Zoom = 1.0f;
            this.Create();
            this.SwitchMenu("home");
        }

        public override void Exit()
        {
            this.currentMenu = null;
            foreach (var item in menus)
            {
                item.Value.ClearItems();
            }
            this.menus.Clear();
            this.resourceAssets.ClearResources();
        }

        public override void HandleKeyPressed(KeyEventArgs e)
        {
            
        }

        public override void HandleKeyReleased(KeyEventArgs e)
        {

        }

        public override void HandleTextEntered(TextEventArgs e)
        {

        }

        public override void HandleMouseButtonPressed(MouseButtonEventArgs e)
        {
            Vector2f mousePos = new Vector2f(e.X, e.Y);

            var grid = currentMenu.ItemGrid;
            var possibleHandlers = grid.getEntitiesInRadius(mousePos, 10.0f);

            foreach (var guiItem in possibleHandlers)
            {
                if (cCollision.IsPointInsideBox(mousePos, guiItem.Bounds))
                {
                    guiItem.StartClick(e);
                    break;
                }
            }
        }

        // only occurs if fully clicked
        public override void HandleMouseButtonReleased(MouseButtonEventArgs e)
        {
            Vector2f mousePos = new Vector2f(e.X, e.Y);

            var grid = currentMenu.ItemGrid;
            var possibleHandlers = grid.getEntitiesInRadius(mousePos, 10.0f);

            foreach (var guiItem in possibleHandlers)
            {
                if (cCollision.IsPointInsideBox(mousePos, guiItem.Bounds))
                {
                    guiItem.OnClick(e);
                    break;
                }
            }
        }

        public override void HandleMouseMoved(MouseMoveEventArgs e)
        {
            Vector2f mousePos = new Vector2f(e.X, e.Y);
            EntityGrid<BaseGuiItem> grid = null;
            List<BaseGuiItem> possibleHandlers = null;
            try
            {
                grid = currentMenu.ItemGrid;
                possibleHandlers = grid.getEntitiesInRadius(mousePos, 10.0f);

                foreach (var guiItem in possibleHandlers)
                {
                    if (cCollision.IsPointInsideBox(mousePos, guiItem.Bounds))
                    {
                        guiItem.OnMouseMove(e);
                        break;
                    }
                    else
                    {
                        guiItem.OnMouseLeave(e);
                    }
                }
            }
            catch(Exception ex)
            {

            #if DEBUG
                System.Diagnostics.Debug.WriteLine(this.menus != null ? "menus OK" : "menus NULL");
                System.Diagnostics.Debug.WriteLine(currentMenu != null ? "currentMenu OK" : "currentMenu NULL");
                System.Diagnostics.Debug.WriteLine(grid != null ? "grid OK" : "grid NULL");
                System.Diagnostics.Debug.WriteLine(ex.Message);
            #endif
                
                while (true)
                {

                }
                

                // throw ex;
            }

                // grid?.getEntitiesNearby(mousePos);

            
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

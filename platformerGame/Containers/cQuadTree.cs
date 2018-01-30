using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Graphics;
using SFML.System;

namespace platformerGame
{
    class cQuadTree<T> where T : cQuadTreeOccupant
    {
        public const int MAX_OBJECTS = 10;
        public const int MAX_LEVEL = 4;

        List<T> entities;

        static int numLevels = 0;
        int level;
        cAABB bounds;

        RectangleShape shape;
        Text label;
        //cQuadTree*					m_pParent;

        cQuadTree<T> pNW; //north-west : left-top
        cQuadTree<T> pNE; //north-east : right-top
        cQuadTree<T> pSW; //south-west : left-bottom
        cQuadTree<T> pSE; //south-east : right-bottom

        //sfml text
        //Text m_Text;

        public cQuadTree(int _level, cAABB _bounds)
        {
            
            this.level = _level;
            this.bounds = _bounds;

            this.shape = new RectangleShape(bounds.dims);
            this.shape.Position = bounds.topLeft;
            this.shape.OutlineColor = Color.Red;
            this.shape.FillColor = Color.Transparent;
            this.shape.OutlineThickness = 1.0f;

            entities = new List<T>();

            label = new Text();

            float xPos = bounds.rightBottom.X - level * 50;
            label.Position = new Vector2f(xPos, bounds.rightBottom.Y - 30);

            label.Font = cAssetManager.GetFont("BGOTHL");
            label.CharacterSize = 24;
            label.Color = Color.White;
            label.Style = Text.Styles.Bold;

            SplitToRects();
        }

        public cAABB Bounds
        {
            get { return bounds; }
            //set { bounds = value; }
        }

        public void SplitToRects()
        {
            if (level >= MAX_LEVEL)
            {
                return;
            }

            numLevels++;

            float subWidth = bounds.halfDims.X;
            float subHeight = bounds.halfDims.Y;
            float x = bounds.topLeft.X;
            float y = bounds.topLeft.Y;

            pNW = new cQuadTree<T>(level + 1, new cAABB(new Vector2f(x, y), new Vector2f(subWidth, subHeight)));
            pNE = new cQuadTree<T>(level + 1, new cAABB(new Vector2f(x + subWidth, y), new Vector2f(subWidth, subHeight)));
            pSW = new cQuadTree<T>(level + 1, new cAABB(new Vector2f(x, y + subHeight), new Vector2f(subWidth, subHeight)));
            pSE = new cQuadTree<T>(level + 1, new cAABB(new Vector2f(x + subWidth, y + subHeight), new Vector2f(subWidth, subHeight)));

        }

        public void Clear()
        {
            if (level == MAX_LEVEL)
            {
                entities.Clear();
                return;
            }
            else
            //if(this.level + 1 <= numLevels)
            {
                pNW.Clear();
                pNE.Clear();
                pSW.Clear();
                pSE.Clear();
            }

            if (entities.Count > 0)
            {
                entities.Clear();
            }
        }

        public bool Contains(cQuadTree<T> child, T entity)
        {
            //bool b = (cCollision.OverlapAABB(child.Bounds, entity.Bounds));
            //bool b = pChild->GetBounds().AsFloatRect().intersects(pEntity->GetBoundingBox().AsFloatRect());
            return cCollision.OverlapAABB(child.Bounds, entity.Bounds);
        }

        public void AddEntity(T entity)
        {
            /*
            if (entities.Count >= MAX_OBJECTS)
            {
                SplitToRects();
            }
            */

            if (level >= MAX_LEVEL)
            {
                entities.Add(entity);
                return;
            }


            if (Contains(pNW, entity))
            {
                pNW.AddEntity(entity);
                //return;
            }

            if (Contains(pNE, entity))
            {
                pNE.AddEntity(entity);
                //return;
            }

            if (Contains(pSW, entity))
            {
                pSW.AddEntity(entity);
                //return;
            }

            if (Contains(pSE, entity))
            {
                pSE.AddEntity(entity);
                //return;
            }

            /*
            if (Contains(this, entity)) //contains
            {
                //logger << "Entity added to tree" << "";
                entities.Add(entity);
                //m_Text.setString( StringOf<uint>(m_Entities.size()) );
            }
            */

            
        }
        public List<T> GetEntitiesAtPos(Vector2f pos)
        {
            if (level == MAX_LEVEL)
                return entities;

            List<T> returnEntities = new List<T>();
            List<T> childReturnEntities = new List<T>();

            FloatRect boundsRect = bounds.AsFloatRect();

            if (entities.Count > 0)
            {
                returnEntities = entities;
            }

            if (pos.X > boundsRect.Left + boundsRect.Width / 2.0f && pos.X < boundsRect.Left + boundsRect.Width)
            {
                if (pos.Y > boundsRect.Top + boundsRect.Height / 2.0f && pos.Y < boundsRect.Top + boundsRect.Height)
                {
                    childReturnEntities = pSE.GetEntitiesAtPos(pos);

                    returnEntities.AddRange(childReturnEntities);

                    return returnEntities;
                }
                else
                if (pos.Y > boundsRect.Top && pos.Y <= boundsRect.Top + boundsRect.Height / 2.0f)
                {
                    childReturnEntities = pNE.GetEntitiesAtPos(pos);
                    returnEntities.AddRange(childReturnEntities);
                    return returnEntities;
                }
            }
            else
            if (pos.X > boundsRect.Left && pos.X <= boundsRect.Left + boundsRect.Width / 2.0f)
            {
                if (pos.Y > boundsRect.Top + boundsRect.Height / 2.0f && pos.Y < boundsRect.Top + boundsRect.Height)
                {
                    childReturnEntities = pSW.GetEntitiesAtPos(pos);
                    returnEntities.AddRange(childReturnEntities);
                    return returnEntities;
                }
                else
                if (pos.Y > boundsRect.Top && pos.Y <= boundsRect.Top + boundsRect.Height / 2.0f)
                {
                    childReturnEntities = pNW.GetEntitiesAtPos(pos);
                    returnEntities.AddRange(childReturnEntities);
                    return returnEntities;
                }
            }

            return returnEntities;
        }

        public void DrawBounds(RenderTarget target)
        {
            label.DisplayedString = this.entities.Count.ToString();
            target.Draw(this.shape, new RenderStates(BlendMode.Alpha));
            target.Draw(label);
            
            if(this.level < MAX_LEVEL)
            {
                pNW.DrawBounds(target);
                pNE.DrawBounds(target);
                pSW.DrawBounds(target);
                pSE.DrawBounds(target);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SFML.System;
using tileLoader;

using platformerGame.Utilities;

namespace platformerGame
{
    class cMapData
    {
        List<cTile> m_Tiles;

        int m_NumOfTiles;
        int m_Width;
        int m_Height;

        public cMapData()
        {
            m_Tiles = new List<cTile>();
        }

        public void Clear()
        {
            m_Tiles.Clear();
        }
        public List<cTile> Tiles
        {
            get { return m_Tiles; }
        }
        public int NumOfTiles
        {
            get { return m_NumOfTiles; }
        }

        public int Width
        {
            get { return m_Width; }
        }

        public int Height
        {
            get { return m_Height; }
        }

        public bool IsObstacleAtPos(int x, int y)
        {
            return !m_Tiles[GetIndexByXY(x, y)].IsWalkAble();
        }

        public bool IsObstacleAtPos(Vector2i pos)
        {
            return !m_Tiles[GetIndexByXY(pos.X, pos.Y)].IsWalkAble();
        }

        public TileType GetTypeAtPos(Vector2i pos)
        {
            return m_Tiles[GetIndexByXY(pos.X, pos.Y)].Type;
        }
        public int GetIndexByXY(int x, int y)
        {
            int index = y * m_Width + x;
            return index;
        }

        public cTile GetTileAtXY(int x, int y)
        {
            int index = GetIndexByXY(x, y);
            //return m_Tiles[index];
            if (index >= 0 && index < NumOfTiles)
            {
                return m_Tiles[index];
            }
            else
            {

                return null; // new cTile();
                
                //throw new IndexOutOfRangeException();
            }
        }

        public cTile GetTileAtIndex(int index)
        {
            if (index >= 0 && index < NumOfTiles)
            {
                return m_Tiles[index];
            }
            else
            {
                int index2 = cAppMath.Clamp<int>(index, 0, NumOfTiles - 1);
                return m_Tiles[index2];
            }
        }

        Vector2i TilePosByIndex(int index)
        {
            Vector2i pos = new Vector2i();

            pos.Y = index / m_Width; //or: index / m_Height;
            pos.X = index % m_Width; //or: index -  (pos.y * m_Width);

            return pos;
        }
        public void Create(int w, int h)
        {
            m_Width = w;
            m_Height = h;
            m_NumOfTiles = m_Width * m_Height;

            for (int i = 0; i < m_NumOfTiles; i++)
            {
               /* if(cAppRandom.GetRandomFloat() < 0.4f)
                    m_Tiles.Add(new cTile(i + 1, TileType.WALL));
                else*/
                    m_Tiles.Add(new cTile(i + 1, TileType.EMPTY));
            }


        }

        public void LoadFromTMX(string file_name)
        {
            var map = new TmxMap("first.tmx");
            TmxLayerTile[] tmxTiles = map.Layers[0].Tiles.ToArray();
            foreach (var tmxTile in tmxTiles)
            {
                //tmxTile.
               // m_Tiles.Add(new cTile())
            }
        }
        /// <summary>
        /// Fájlból is betudja olvasni a pályát
        /// 1. sor: pálya szélessége (x ~ width ~ oszlopok száma)
        /// 2. sor: pálya magassága (y ~ height ~ sorok száma)
        /// </summary>
        /// <param name="file_name">A fájl neve</param>
        public void LoadFromFile(string file_name)
        {
            m_Tiles.Clear();

            StreamReader sr = new StreamReader(file_name);

            m_Width = int.Parse(sr.ReadLine());
            m_Height = int.Parse(sr.ReadLine());
            m_NumOfTiles = m_Width * m_Height;
            int sor = 0;

            while (!sr.EndOfStream)
            {
                string[] splitted = sr.ReadLine().Split(' ');

                for(int i = 0; i < splitted.Length; i++)
                {
                    TileType typeCode = (TileType)int.Parse(splitted[i]);
                    m_Tiles.Add( new cTile(sor * m_Height + i, typeCode) );
                }

                sor++;
            }
            sr.Close();
        }
    }
}

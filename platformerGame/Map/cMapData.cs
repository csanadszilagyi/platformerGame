using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using SFML.System;
using tileLoader;

using platformerGame.Utilities;
using SFML.Graphics;
using System.Text.RegularExpressions;

namespace platformerGame.Map
{
    class TileLayers
    {
        public const int   WALLS = 0,
                              BACKGROUND_1 = 1,
                              BACKGROUND_2 = 2;
    }

    class cMapData
    {
        Dictionary<int, List<cTile>> layers;

        int numOfTiles;
        int width;
        int height;

        Texture tilesetTexture;
        cAABB levelStartRegion;
        cAABB levelEndRegion;

        public cMapData()
        {
            layers = new Dictionary<int, List<cTile>>();
            tilesetTexture = null;
            levelStartRegion = new cAABB();
            levelEndRegion = new cAABB();
        }

        public void Clear()
        {
            layers.Clear();
        }
        public List<cTile> Tiles
        {
            get { return layers[TileLayers.WALLS]; }
        }
        public int NumOfTiles
        {
            get { return numOfTiles; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public Texture TilesetTexture
        {
            get
            {
                return tilesetTexture;
            }
        }

        public cAABB LevelStartRegion
        {
            get
            {
                return levelStartRegion;
            }
        }

        public cAABB LevelEndRegion
        {
            get
            {
                return levelEndRegion;
            }
        }

        public bool IsObstacleAtPos(int x, int y)
        {
            return !layers[TileLayers.WALLS][GetIndexByXY(x, y)].IsWalkAble();
        }

        public bool IsObstacleAtPos(Vector2i pos)
        {
            return !layers[TileLayers.WALLS][GetIndexByXY(pos.X, pos.Y)].IsWalkAble();
        }

        public TileType GetTypeAtPos(Vector2i pos)
        {
            return layers[TileLayers.WALLS][GetIndexByXY(pos.X, pos.Y)].Type;
        }
        public int GetIndexByXY(int x, int y)
        {
            int index = y * width + x;
            return index;
        }

        public cTile[] getAllTileAtXY(int x, int y)
        {
            int index = GetIndexByXY(x, y);

            List<cTile> ret = new List<cTile>();

            if (index >= 0 && index < NumOfTiles)
            {
                foreach (var layerTiles in layers)
                {
                        ret.Add(layerTiles.Value[index]);
                }

                ret.Reverse();
                return ret.ToArray();
                
            }

            return null;
        }

        public cTile GetTileAtXY(int x, int y, int layer = TileLayers.WALLS)
        {
            int index = GetIndexByXY(x, y);
            //return m_Tiles[index];
            if (index >= 0 && index < NumOfTiles)
            {
                return layers[layer][index];
            }
            else
            {

                return null; // new cTile();
                
                //throw new IndexOutOfRangeException();
            }
        }

        public cTile GetTileAtIndex(int index, int layer = TileLayers.WALLS)
        {
            if (index >= 0 && index < NumOfTiles)
            {
                return layers[layer][index];
            }
            else
            {
                int index2 = cAppMath.Clamp<int>(index, 0, NumOfTiles - 1);
                return layers[layer][index2];
            }
        }

        Vector2i TilePosByIndex(int index)
        {
            Vector2i pos = new Vector2i();

            pos.Y = index / width; //or: index / m_Height;
            pos.X = index % width; //or: index -  (pos.y * m_Width);

            return pos;
        }
        public void Create(int w, int h)
        {
            width = w;
            height = h;
            numOfTiles = width * height;
            layers[TileLayers.WALLS] = new List<cTile>();
            for (int i = 0; i < numOfTiles; i++)
            {
               /* if(cAppRandom.GetRandomFloat() < 0.4f)
                    m_Tiles.Add(new cTile(i + 1, TileType.WALL));
                else*/
                    layers[TileLayers.WALLS].Add(new cTile(i + 1, TileType.EMPTY));
            }


        }

        private cAABB getTextureRect(int type_id, int tiles_in_row)
        {
            int posY = type_id / tiles_in_row; //or: index / m_Height;
            int posX = type_id % tiles_in_row; //or: index -  (pos.y * m_Width);

            return new cAABB(posX* Constants.TILE_SIZE, posY* Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
        }

        private void loadBackgrounds(TmxMap map, Dictionary<int, TmxTilesetTile> tile_types, int first_gid, int tiles_in_row)
        {
            List<string> backgroundNames = new List<string>();

            string pattern = @"\bbackground";

            foreach (var layer in map.Layers)
            {
                if( Regex.IsMatch(layer.Name, pattern))
                {
                    backgroundNames.Add(layer.Name);
                }
            }

            foreach(string bgName in backgroundNames)
            {
                TmxLayerTile[] tmxTiles = map.Layers[bgName].Tiles.ToArray();


                int tileID = 0;
                foreach (var tmxTile in tmxTiles)
                {
                    int gid = tmxTile.Gid;
                    int type;

                    int typeID = gid - first_gid;

                    cTile gameTile = null;

                    try
                    {
                        if (typeID < 0)
                        {
                            gameTile = new cTile(tileID, TileType.EMPTY);

                            // does not matter, because we just skip the drwaing of empty tiles...
                            gameTile.PosOnTexture = new cAABB(0, 0, Constants.TILE_SIZE, Constants.TILE_SIZE);
                        }
                        else
                        if (int.TryParse(tile_types[typeID].Type, out type))
                        {

                            gameTile = new cTile(tileID, TileType.EMPTY);
                            gameTile.PosOnTexture = getTextureRect(typeID, tiles_in_row);

                        }

                        layers[TileLayers.BACKGROUND_1].Add(gameTile);
                        tileID++;
                    }
                    catch (Exception e)
                    {

                    }

                }
            }

        }

        private void loadWalls(TmxMap map, Dictionary<int, TmxTilesetTile> tile_types, int first_gid, int tiles_in_row)
        {
            TmxLayerTile[] tmxTiles = map.Layers["walls"].Tiles.ToArray();

            layers[TileLayers.WALLS] = new List<cTile>();

            int tileID = 0;
            foreach (var tmxTile in tmxTiles)
            {
                int gid = tmxTile.Gid;
                int type;

                int typeID = gid - first_gid;

                cTile gameTile = null;

                try
                {
                    if (typeID < 0)
                    {
                        gameTile = new cTile(tileID, TileType.EMPTY);

                        // does not matter, because we just skip the drwaing of empty tiles...
                        gameTile.PosOnTexture = new cAABB(0, 0, Constants.TILE_SIZE, Constants.TILE_SIZE);
                    }
                    else
                    if (int.TryParse(tile_types[typeID].Type, out type))
                    {

                        gameTile = new cTile(tileID, (TileType)type);
                        gameTile.PosOnTexture = getTextureRect(typeID, tiles_in_row);


                    #if DEBUG
                            System.Diagnostics.Debug.WriteLine(
                                string.Format("{0} : {1}", gameTile.PosOnTexture.topLeft.X, gameTile.PosOnTexture.topLeft.Y));
                        }
                    #endif


                    layers[TileLayers.WALLS].Add(gameTile);
                    tileID++;
                }
                catch (Exception e)
                {

                }

            }
        }

        public void LoadFromTMX(string tmx_file_name)
        {
            var map = new TmxMap(tmx_file_name);
            var tileSet = map.Tilesets[0];
            var tileTypes = tileSet.Tiles;

            width = map.Width;
            height = map.Height;
            numOfTiles = width * height;

            tilesetTexture = cAssetManager.LoadAndReturnTexture(tileSet.Image.Source);

            int firstGid = map.Tilesets[0].FirstGid;
            int imgWidth = (int)tileSet.Image.Width;
            int imgHeight = (int)tileSet.Image.Height;

            Constants.TILE_SIZE = tileSet.TileWidth; // suppose width and heigt are equals

            int tilesInRow = imgWidth / Constants.TILE_SIZE;

            loadWalls(map, tileTypes, firstGid, tilesInRow);
            //loadBackgrounds(map, tileTypes, firstGid, tilesInRow);

            TmxObject startObject = map.ObjectGroups["SpawnPoints"].Objects["LevelStart"];
            TmxObject endObject = map.ObjectGroups["SpawnPoints"].Objects["LevelEnd"];

            this.levelStartRegion = new cAABB(new Vector2f((float)startObject.X, (float)startObject.Y),
                                              new Vector2f((float)startObject.Width, (float)startObject.Height));

            this.levelEndRegion = new cAABB(new Vector2f((float)endObject.X, (float)endObject.Y),
                                              new Vector2f((float)endObject.Width, (float)endObject.Height));
        }


        /// <summary>
        /// Fájlból is betudja olvasni a pályát
        /// 1. sor: pálya szélessége (x ~ width ~ oszlopok száma)
        /// 2. sor: pálya magassága (y ~ height ~ sorok száma)
        /// </summary>
        /// <param name="file_name">A fájl neve</param>
        public void LoadFromFile(string file_name)
        {
            layers.Clear();

            StreamReader sr = new StreamReader(file_name);

            width = int.Parse(sr.ReadLine());
            height = int.Parse(sr.ReadLine());
            numOfTiles = width * height;
            int sor = 0;

            while (!sr.EndOfStream)
            {
                string[] splitted = sr.ReadLine().Split(' ');

                for(int i = 0; i < splitted.Length; i++)
                {
                    TileType typeCode = (TileType)int.Parse(splitted[i]);
                    layers[TileLayers.WALLS].Add( new cTile(sor * height + i, typeCode) );
                }

                sor++;
            }
            sr.Close();
        }
    }
}

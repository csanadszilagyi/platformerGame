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
    class TileLayerTypes
    {
        public const int    BACKGROUND_1    = 0,
                            BACKGROUND_2    = 1,
                            WALLS           = 2;
                            
    }

    class cMapData
    {
        Dictionary<int, List<cTile>> layers;

        int numOfTiles;
        int width;
        int height;

        Texture tilesetTexture;
        AABB levelStartRegion;
        AABB levelEndRegion;


        TmxMap map;

        public cMapData()
        {
            layers = new Dictionary<int, List<cTile>>();
            tilesetTexture = null;
            levelStartRegion = new AABB();
            levelEndRegion = new AABB();
        }

        public void Clear()
        {
            layers.Clear();
        }
        public List<cTile> Tiles
        {
            get { return layers[TileLayerTypes.WALLS]; }
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

        public AABB LevelStartRegion
        {
            get
            {
                return levelStartRegion;
            }
        }

        public AABB LevelEndRegion
        {
            get
            {
                return levelEndRegion;
            }
        }

        public bool IsObstacleAtPos(int x, int y)
        {
            return !layers[TileLayerTypes.WALLS][GetIndexByXY(x, y)].IsWalkAble();
        }

        public bool IsObstacleAtPos(Vector2i pos)
        {
            return !layers[TileLayerTypes.WALLS][GetIndexByXY(pos.X, pos.Y)].IsWalkAble();
        }

        public TileType GetTypeAtPos(Vector2i pos)
        {
            return layers[TileLayerTypes.WALLS][GetIndexByXY(pos.X, pos.Y)].Type;
        }
        public int GetIndexByXY(int x, int y)
        {
            int index = y * width + x;
            return index;
        }

        public cTile[] getDrawAbleTiles(int x, int y)
        {
            int index = GetIndexByXY(x, y);
            //this.layers[TileLayers.WALLS].Where(t => t.Type != TileType.EMPTY).ToList();

            List<cTile> ret = new List<cTile>();

            if (index >= 0 && index < NumOfTiles)
            {
                foreach (var layerTiles in layers)
                {
                        ret.Add(layerTiles.Value[index]);
                }

                //ret.Reverse();
                return ret.ToArray();
                
            }

            return null;
        }

        public cTile GetTileAtXY(int x, int y, int layer = TileLayerTypes.WALLS)
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

        public cTile GetTileAtIndex(int index, int layer = TileLayerTypes.WALLS)
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
            layers[TileLayerTypes.WALLS] = new List<cTile>();
            for (int i = 0; i < numOfTiles; i++)
            {
               /* if(cAppRandom.GetRandomFloat() < 0.4f)
                    m_Tiles.Add(new cTile(i + 1, TileType.WALL));
                else*/
                    layers[TileLayerTypes.WALLS].Add(new cTile(i + 1, TileType.EMPTY));
            }


        }

        private AABB getTextureRect(int index, int tiles_in_row)
        {
            int posY = index / tiles_in_row; //or: index / m_Height;
            int posX = index % tiles_in_row; //or: index -  (pos.y * m_Width);

            return new AABB(posX* Constants.TILE_SIZE, posY* Constants.TILE_SIZE, Constants.TILE_SIZE, Constants.TILE_SIZE);
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
                            gameTile.PosOnTexture = new AABB(0, 0, Constants.TILE_SIZE, Constants.TILE_SIZE);
                        }
                        else
                        if (int.TryParse(tile_types[typeID].Type, out type))
                        {

                            gameTile = new cTile(tileID, TileType.EMPTY);
                            gameTile.PosOnTexture = getTextureRect(typeID, tiles_in_row);

                        }

                        layers[TileLayerTypes.BACKGROUND_1].Add(gameTile);
                        tileID++;
                    }
                    catch (Exception e)
                    {

                    }

                }
            }

        }

        private void loadLayer(Dictionary<int, TmxTilesetTile> tile_types, string tmx_layer, int into_layer, int first_gid, int tiles_in_row)
        {
            TmxLayerTile[] tmxTiles = this.map.Layers[tmx_layer].Tiles.ToArray();

            layers[into_layer] = new List<cTile>();

            int tileID = 0;
            foreach (var tmxTile in tmxTiles)
            {
               
                int globalID = tmxTile.Gid;

                cTile gameTile = null;
                try
                {
                    if (globalID <= 0)
                    {
                        // do nothing, it is an empty tile
                        gameTile = new cTile(tileID, TileType.EMPTY);
                    }
                    else
                    {
                        int frameTile = globalID - 1; // substract one is very important!

                        int type = Convert.ToInt32(tile_types[frameTile].Type);
                        gameTile = new cTile(tileID, (TileType)type);
                        gameTile.GlobalId = globalID;

                        gameTile.PosOnTexture = getTextureRect(frameTile, tiles_in_row);
                       

                        #if DEBUG
                            System.Diagnostics.Debug.WriteLine(
                                    string.Format("{0} : {1}", gameTile.PosOnTexture.topLeft.X, gameTile.PosOnTexture.topLeft.Y));
                        #endif
                    }

                    layers[into_layer].Add(gameTile);
                    //simple tile id counting...
                    tileID++;


                }
                catch (Exception e)
                {

                }

            }
        }

        public TmxMap GetTmxMap()
        {
            return this.map;
        }

        public void LoadFromTMX(string tmx_file_name)
        {
            this.map = new TmxMap(tmx_file_name);
            var tileSet = map.Tilesets[0];
            var tileTypes = tileSet.Tiles;

            width = map.Width;
            height = map.Height;
            numOfTiles = width * height;

            // works also
            string fileName = AssetManager.TEX_RESOURCE_PATH + AssetManager.TILESET_PATH + tileSet.Image.Source.Split('\\').Last();
            
            this.tilesetTexture = AssetManager.LoadAndReturnTexture(tileSet.Image.Source); // fileName);

#if DEBUG

            System.Diagnostics.Debug.WriteLine(
                string.Format("splitted fileName: {0}", fileName));

            System.Diagnostics.Debug.WriteLine(
                string.Format("tmx directory: {0}", tileSet.TmxDirectory));
            System.Diagnostics.Debug.WriteLine(
               string.Format("tileset texture: {0}", tileSet.Image.Source));

#endif

            int firstGid = map.Tilesets[0].FirstGid;
            int imgWidth = (int)tileSet.Image.Width;
            int imgHeight = (int)tileSet.Image.Height;

            Constants.TILE_SIZE = tileSet.TileWidth; // suppose width and height are equals

            int tilesInRow = imgWidth / Constants.TILE_SIZE;

            loadLayer(tileTypes, "walls", TileLayerTypes.WALLS, firstGid, tilesInRow);
            loadLayer(tileTypes, "background1", TileLayerTypes.BACKGROUND_1, firstGid, tilesInRow);
            loadLayer(tileTypes, "background2", TileLayerTypes.BACKGROUND_2, firstGid, tilesInRow);

            TmxObject startObject = map.ObjectGroups["SpawnPoints"].Objects["LevelStart"];
            TmxObject endObject = map.ObjectGroups["SpawnPoints"].Objects["LevelEnd"];

            this.levelStartRegion = new AABB(new Vector2f((float)startObject.X, (float)startObject.Y),
                                              new Vector2f((float)startObject.Width, (float)startObject.Height));

            this.levelEndRegion = new AABB(new Vector2f((float)endObject.X, (float)endObject.Y),
                                              new Vector2f((float)endObject.Width, (float)endObject.Height));
        }


        /// <summary>
        /// Fájlból is be tudja olvasni a pályát
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

            layers[TileLayerTypes.WALLS] = new List<cTile>();

            while (!sr.EndOfStream)
            {
                string[] splitted = sr.ReadLine().Split(' ');

                for(int i = 0; i < splitted.Length; i++)
                {
                    TileType typeCode = (TileType)int.Parse(splitted[i]);
                    layers[TileLayerTypes.WALLS].Add( new cTile(sor * height + i, typeCode) );
                }

                sor++;
            }
            sr.Close();
        }
    }
}

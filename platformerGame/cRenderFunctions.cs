using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

using platformerGame.Utilities;

namespace platformerGame
{
    class cRenderFunctions
    {
        //private static Sprite sprite = new Sprite();
        private static Sprite sprite = new Sprite();
        static cRenderFunctions()
        {

        }
        public static void DrawSprite(RenderTarget destination,
                                      Vector2f location,
								      Texture texture,
								      IntRect sub_rect,
                                      Color coloration,
								      float rotation,
                                      Vector2f scale,
                                      BlendMode blend_mode,
                                      Shader shader )
        {

            
            sprite.Texture = texture;
            sprite.Origin = new Vector2f(sprite.TextureRect.Width / 2.0f, sprite.TextureRect.Height / 2.0f);
            sprite.Scale = scale;
	        sprite.Position = location;

	        
	        //sprite.TextureRect = sub_rect;
	        sprite.Color = coloration;
	        sprite.Rotation = rotation; //rotation-90.0f
            
            RenderStates rs = new RenderStates(blend_mode); // Transform.Identity, null, shader);

            
            
	        destination.Draw(sprite, rs);

           
        }

        public static void DrawTextureSimple(RenderTarget destination,
                                       Vector2f location,
                                       Texture texture,
                                       IntRect sub_rect,
                                       Color coloration,
                                       BlendMode blend_mode
                                       )
        {

            Transform translationT = Transform.Identity;
            translationT.Translate(location);

            //Setup the render state.
            float left = sub_rect.Left;
            float right = left + sub_rect.Width;
            float top = sub_rect.Top;
            float bottom = top + sub_rect.Height;

            float widthBeforeTransform = sub_rect.Width;
            float heightBeforeTransform = sub_rect.Height;

            RenderStates states = new RenderStates(blend_mode, translationT, texture, null);


            //Setup the vertices and their attributes.

            VertexArray vertices = new VertexArray(PrimitiveType.Quads);


            vertices.Append(new Vertex(new Vector2f(0.0f, 0.0f), coloration, new Vector2f(left, top)));
            vertices.Append(new Vertex(new Vector2f(widthBeforeTransform, 0), coloration, new Vector2f(right, top)));
            vertices.Append(new Vertex(new Vector2f(widthBeforeTransform, heightBeforeTransform), coloration, new Vector2f(right, bottom)));
            vertices.Append(new Vertex(new Vector2f(0, heightBeforeTransform), coloration, new Vector2f(left, bottom)));




            destination.Draw(vertices, states);
        }
        public static void DrawTexture(RenderTarget destination,
    							       Vector2f location,
    							       Texture texture,
                                       IntRect sub_rect,
    							       Color coloration,
                                       float rotation,
                                       float scale,
                                       bool flip_horizontally,
                                       bool flip_vertically,
    							       BlendMode blend_mode,
    							       Shader shader)
        {

            if (sub_rect.Width == 0 || sub_rect.Height == 0)
            {
                sub_rect.Top = 0;
                sub_rect.Left = 0;
                sub_rect.Width = (int)texture.Size.X;
                sub_rect.Height = (int)texture.Size.Y;
            }

            //Set the position in space.
            Transform translationT = Transform.Identity;
            
            translationT.Translate(location);

            //Set the rotation (rotated around the center, since this Transform wasn't moved).
            Transform rotationT = Transform.Identity;
            rotationT.Rotate(rotation);

            Transform scaleT = Transform.Identity;
            scaleT.Scale(scale, scale);

            //Setup the render state.
            float left = sub_rect.Left;
            float right = left + sub_rect.Width;
            float top = sub_rect.Top;
            float bottom = top + sub_rect.Height;

            float widthBeforeTransform = sub_rect.Width;
            float heightBeforeTransform = sub_rect.Height;

            RenderStates states = new RenderStates(blend_mode, translationT * rotationT * scaleT, texture, shader);
            
           

            //Setup the vertices and their attributes.
            
            VertexArray vertices = new VertexArray(PrimitiveType.Quads);
            /*
            Vertex vtx = vertices[0];
            vtx.Position = new Vector2f(0, 0);
            vtx.Color = coloration;
            vtx.TexCoords = new Vector2f(left, top);
            vertices[0] = vtx;

            vtx = vertices[1];
            vtx.Position = new Vector2f(0, heightBeforeTransform);
            vtx.Color = coloration;
            vtx.TexCoords = new Vector2f(left, bottom);
            vertices[1] = vtx;

             vtx = vertices[2];
            vtx.Position = new Vector2f(widthBeforeTransform, heightBeforeTransform);
            vtx.Color = coloration;
            vtx.TexCoords = new Vector2f(right, bottom);
            vertices[2] = vtx;

            vtx = vertices[3];
            vtx.Position = new Vector2f(widthBeforeTransform, 0);
            vtx.Color = coloration;
            vtx.TexCoords = new Vector2f(right, top);
            vertices[3] = vtx;
            */

            if(flip_vertically)
            {
                Utils.Swap<float>(ref top, ref bottom);
            }

            if (flip_horizontally)
            {
                Utils.Swap<float>(ref right, ref left);
            }

            vertices.Append(new Vertex(new Vector2f(0.0f, 0.0f), coloration, new Vector2f(left, top)));
            vertices.Append(new Vertex(new Vector2f(widthBeforeTransform, 0), coloration, new Vector2f(right, top)));
            vertices.Append(new Vertex(new Vector2f(widthBeforeTransform, heightBeforeTransform), coloration, new Vector2f(right, bottom)));
            vertices.Append(new Vertex(new Vector2f(0, heightBeforeTransform), coloration, new Vector2f(left, bottom)));
            
            


            //The transparency:
            /*
            vertices[0].Color = coloration;
            vertices[1].Color = coloration;
            vertices[2].Color = coloration;
            vertices[3].Color = coloration;
            
            //The pre-transform position and size:
           

            vertices[0].Position = new Vector2f(0, 0);
            vertices[1].Position = new Vector2f(0, heightBeforeTransform);
            vertices[2].Position = new Vector2f(widthBeforeTransform, heightBeforeTransform);
            vertices[3].Position = new Vector2f(widthBeforeTransform, 0);
            
            //Calculate the texture coordinates:
            

            //If we're mirroring, swap the texture coordinates vertically and/or horizontally.
            /*
            if (flip_vertically) std::swap(top, bottom);
            if (flip_horizontally) std::swap(left, right);
            */

            //Set the texture coordinates:
            /*
            vertices[0].TexCoords = new Vector2f(left, top);
            vertices[1].TexCoords = new Vector2f(left, bottom);
            vertices[2].TexCoords = new Vector2f(right, bottom);
            vertices[3].TexCoords = new Vector2f(right, top);
            */
            //Use the RenderTarget to draw the vertices using the RenderStates we set up.

            destination.Draw(vertices, states);

            //vertices.Draw(destination, states);
        }

        public static void DrawTextureUseCenter(RenderTarget destination,
                                       Vector2f location,
                                       Texture texture,
                                       IntRect sub_rect,
                                       Color coloration,
                                       float rotation,
                                       float scale,
                                       bool flip_horizontally,
                                       bool flip_vertically,
                                       BlendMode blend_mode,
                                       Shader shader)
        {

            if (sub_rect.Width == 0 || sub_rect.Height == 0)
            {
                sub_rect.Top = 0;
                sub_rect.Left = 0;
                sub_rect.Width = (int)texture.Size.X;
                sub_rect.Height = (int)texture.Size.Y;
            }


            //Set the position in space.
            Transform translationT = Transform.Identity;

            translationT.Translate(location); //location

            //Set the rotation (rotated around the center, since this Transform wasn't moved).
            Transform rotationT = Transform.Identity;
            rotationT.Rotate(rotation);

            Transform scaleT = Transform.Identity;
            scaleT.Scale(scale, scale);

            //Setup the render state.
            float left = sub_rect.Left;
            float right = left + sub_rect.Width;
            float top = sub_rect.Top;
            float bottom = top + sub_rect.Height;

            float widthBeforeTransform = sub_rect.Width;
            float heightBeforeTransform = sub_rect.Height;

            RenderStates states = new RenderStates(blend_mode, translationT * rotationT * scaleT, texture, shader);



            //Setup the vertices and their attributes.

            VertexArray vertices = new VertexArray(PrimitiveType.Quads);


            if (flip_vertically)
            {
                Utils.Swap<float>(ref top, ref bottom);
            }

            if (flip_horizontally)
            {
                Utils.Swap<float>(ref right, ref left);
            }

            // centered
            
            vertices.Append(new Vertex(new Vector2f(-widthBeforeTransform/2, -heightBeforeTransform/2), coloration, new Vector2f(left, top)));
            vertices.Append(new Vertex(new Vector2f(widthBeforeTransform/2, -heightBeforeTransform / 2), coloration, new Vector2f(right, top)));
            vertices.Append(new Vertex(new Vector2f(widthBeforeTransform/2, heightBeforeTransform/2), coloration, new Vector2f(right, bottom)));
            vertices.Append(new Vertex(new Vector2f(-widthBeforeTransform / 2, heightBeforeTransform/2), coloration, new Vector2f(left, bottom)));
            /*
            vertices.Append(new Vertex(new Vector2f(0.0f, 0.0f), coloration, new Vector2f(left, top)));
            vertices.Append(new Vertex(new Vector2f(widthBeforeTransform, 0), coloration, new Vector2f(right, top)));
            vertices.Append(new Vertex(new Vector2f(widthBeforeTransform, heightBeforeTransform), coloration, new Vector2f(right, bottom)));
            vertices.Append(new Vertex(new Vector2f(0, heightBeforeTransform), coloration, new Vector2f(left, bottom)));
            */
            //Use the RenderTarget to draw the vertices using the RenderStates we set up.

            destination.Draw(vertices, states);

        }


        public static void DrawDirLightByDVec(RenderTarget destination,

                                     Vector2f light_pos,
									 double light_radius,
                                     Vector2f light_dir,
									 double light_spread_angle,
                                     Color Color,
									 BlendMode blend_mode,
                                     Shader shader )
{

        double lightSubdivisionSize = cAppMath.PI / 24.0;

        VertexArray va = new VertexArray(PrimitiveType.TriangleFan);

        Transform t = Transform.Identity;
        RenderStates states = new RenderStates(blend_mode, t, null, shader);
		
	    double light_dir_angle = cAppMath.GetAngleOfVector(light_dir);
            //Math->GetAngleBetwenVecs(light_pos, light_dir) - Math->PI/2.0;

        va.Append(new Vertex(light_pos, Color, new Vector2f()) );
      
		// Set the edge color for rest of shape
	    int numSubdivisions = (int)(light_spread_angle / lightSubdivisionSize);
        double startAngle = light_dir_angle - light_spread_angle / 2.0f;
      
	    for(int currentSubDivision = 0; currentSubDivision <= numSubdivisions; currentSubDivision++)
	    {
		    double angle = startAngle + currentSubDivision * lightSubdivisionSize;
           
		    va.Append(new Vertex(new Vector2f((float)(light_radius * Math.Cos(angle) + light_pos.X), (float)(light_radius * Math.Sin(angle) + light_pos.Y)), Color));
	    }

        destination.Draw(va, states);

//=========================================================================================
//soft portion-----------------------------------------------------------------------------
//		sf::VertexArray finVA;
//		sf::RenderStates finStates;
//		finStates.blendMode = sf::BlendMultiply;
//		finStates.shader = shader;
//		finStates.texture = &myLightFinTexture;
//
//		sf::Color color(120, 120, 120, 0);
//
//		double softSpreadAngle = Math->PI / 35.0;
//
//		double umbraAngle1 = light_dir_angle - light_spread_angle / 2.0f;
//		double penumbraAngle1 = umbraAngle1 + softSpreadAngle;
//		sf::Vector2f penumbra_pos = sf::Vector2f(light_radius * cosf(penumbraAngle1), light_radius * sinf(penumbraAngle1));
//		sf::Vector2f umbra_pos = sf::Vector2f(light_radius * cosf(umbraAngle1), light_radius * sinf(umbraAngle1));
//		sf::Vector2f rootPos = light_pos;
///*
//		sf::Vector2f tc1 = sf::Vector2f(myLightFinTexture.getSize().x, 0);
//		sf::Vector2f tc2 = sf::Vector2f(myLightFinTexture.getSize().x, myLightFinTexture.getSize().y);
//		sf::Vector2f tc3 = sf::Vector2f(0 , myLightFinTexture.getSize().y);
//*/
//		sf::Vector2f tc1 = sf::Vector2f(0, 0);
//		sf::Vector2f tc2 = sf::Vector2f(myLightFinTexture.getSize().x, 0);
//		sf::Vector2f tc3 = sf::Vector2f(0 , myLightFinTexture.getSize().y);
//
//		sf::Vector2f p1 = sf::Vector2f(rootPos.x, rootPos.y);
//		sf::Vector2f p2 = sf::Vector2f(rootPos.x + penumbra_pos.x, rootPos.y + penumbra_pos.y);
//		sf::Vector2f p3 = sf::Vector2f(rootPos.x + umbra_pos.x, rootPos.y + umbra_pos.y);
//
//		finVA.append(sf::Vertex(p1, color, tc1));
//		finVA.append(sf::Vertex(p2, color, tc2));
//		finVA.append(sf::Vertex(p3, color, tc3));
//
//		destination.draw(&finVA[0], finVA.getVertexCount(), sf::PrimitiveType::Triangles, finStates);
//=====================================================================================================
		//finVA.clear();

		//double umbraAngle2 = light_dir_angle + light_spread_angle / 2.0f;
		//double penumbraAngle2 = umbraAngle2 - softSpreadAngle;
		//sf::Vector2f penumbra_pos_2 = sf::Vector2f(light_radius * cosf(penumbraAngle2), light_radius * sinf(penumbraAngle2));
		//sf::Vector2f umbra_pos_2 = sf::Vector2f(light_radius * cosf(umbraAngle2), light_radius * sinf(umbraAngle2));
		//sf::Vector2f rootPos_2 = light_pos;

		//tc1 = sf::Vector2f(0, 0);
		//tc2 = sf::Vector2f(myLightFinTexture.getSize().x, 0);
		//tc3 = sf::Vector2f(0 , myLightFinTexture.getSize().y);

		//p1 = sf::Vector2f(rootPos_2.x, rootPos_2.y);
		//p2 = sf::Vector2f(rootPos_2.x + penumbra_pos_2.x, rootPos_2.y + penumbra_pos_2.y);
		//p3 = sf::Vector2f(rootPos_2.x + umbra_pos_2.x, rootPos_2.y + umbra_pos_2.y);

		//finVA.append(sf::Vertex(p1, color, tc1));
		//finVA.append(sf::Vertex(p2, color, tc2));
		//finVA.append(sf::Vertex(p3, color, tc3));

		//destination.draw(&finVA[0], finVA.getVertexCount(), sf::PrimitiveType::Triangles, finStates);
//===================================================================================================
//====================================================================================================
}

        public static void DrawRectangleShape(RenderTarget destination,
                                              cAABB bounds,
                                              Color color,
                                              BlendMode blend_mode,
                                              double orientation = 0.0)
        {
            RenderStates states = new RenderStates(blend_mode);
            RectangleShape rs = new RectangleShape(bounds.dims);
            rs.Rotation = (float)orientation;
            rs.Position = bounds.topLeft;
            rs.FillColor = color;
            destination.Draw(rs, states);
        }
        public static void DrawLine(RenderTarget destination,
                                       Vector2f pos_start,
                                       Vector2f pos_end,
                                       Color color,
                                       BlendMode blend_mode)
        {
            RenderStates states = new RenderStates(blend_mode);
            VertexArray vertices = new VertexArray(PrimitiveType.Lines);
            vertices.Append(new Vertex(pos_start, color));
            vertices.Append(new Vertex(pos_end, color));
            destination.Draw(vertices, states);
        }

        public static void DrawLine(RenderTarget destination,
                                       Vector2f pos_start,
                                       Vector2f pos_end,
                                       float thickness,
                                       Color color,
                                       BlendMode blend_mode)
        {
            sfLine line = new sfLine(pos_start, pos_end, color, thickness);
            line.Draw(destination, new RenderStates(blend_mode));
            //destination.Draw(vertices, states);
        }
    }
}

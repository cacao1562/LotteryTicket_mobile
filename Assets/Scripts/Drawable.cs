using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
// using UnityEditor;
    // 1. Attach this to a read/write enabled sprite image
    // 2. Set the drawing_layers  to use in the raycast
    // 3. Attach a 2D collider (like a Box Collider 2D) to this sprite
    // 4. Hold down left mouse to draw on this texture!
    public class Drawable : MonoBehaviour
    {
        // PEN COLOUR
        public Color Pen_Colour = new Color(0,0,0,0);     // Change these to change the default drawing settings
        // PEN WIDTH (actually, it's a radius, in pixels)
        public int Pen_Width = 20;

        public LayerMask Drawing_Layers;

        public bool Reset_Canvas_On_Play = true;
        // The colour the canvas is reset to each time
        public Color Reset_Colour = new Color(0, 0, 0, 255);  // By default, reset the canvas to be transparent

        // MUST HAVE READ/WRITE enabled set in the file editor of Unity
        Sprite drawable_sprite;
        Texture2D drawable_texture;

        Vector2 previous_drag_position;
        Color[] clean_colours_array;
        Color transparent;
        Color32[] cur_colors;
        bool mouse_was_previously_held_down = false;
        bool no_drawing_on_current_drag = false;

        private Texture2D cloneT;
        public SocketManager socketManager;
        public GameObject particle;
        public GameObject coin;
        private int count = 0;
        private SpriteRenderer spRenderer;
        private int sp_width;
        public GameObject nextPopup;
        public GameObject buttonParticle;
        

        Texture2D duplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }


        // Texture2D render(Texture2D tex) {
        //     var temp = RenderTexture.GetTemporary(tex.width, tex.height);
        //     Graphics.Blit(tex, temp);
        //     // ReadPixelsで直前のレンダリング結果を読み込める
        //     var copy = new Texture2D(tex.width, tex.height);
        //     copy.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        //     RenderTexture.ReleaseTemporary(temp);
        //     return copy;
        // }
        // Texture2D duplicateTexture2(Texture2D source)
        //     {
        //         byte[] pix = source.GetRawTextureData();
        //         Texture2D readableText = new Texture2D(source.width, source.height, source.format, false);
        //         readableText.LoadRawTextureData(pix);
        //         readableText.Apply();
        //         return readableText;
        //     }

        Texture2D img;
        Texture2D reimg;
        string path;
        int w , h;
        void Awake()
        {
             Screen.sleepTimeout = SleepTimeout.NeverSleep;
             path = PlayerPrefs.GetString("path");
             w = PlayerPrefs.GetInt("width");
             h = PlayerPrefs.GetInt("height");
        }
        void Start()
        {
            
           
            img = NativeGallery.LoadImageAtPath(path ,-1);
            reimg = duplicateTexture(img);
            // // reimg.Compress(false);
            Rect rect = new Rect(0,0,reimg.width, reimg.height);
            Sprite sss = Sprite.Create(reimg, rect , new Vector2(0.5f, 0.5f));
        
            // Sprite ss = GameObject.Find("sp").GetComponent<SpriteRenderer>().sprite;
            GetComponent<SpriteRenderer>().sprite = sss;
            
            spRenderer = GetComponent<SpriteRenderer>();

            // this.GetComponent<SpriteRenderer>().sprite = sss;
            drawable_sprite = this.GetComponent<SpriteRenderer>().sprite;
            // drawable_sprite = sss;
            // sp_width =  (int)drawable_sprite.rect.width;
           
            drawable_texture =  drawable_sprite.texture;
            
            GetComponent<BoxCollider2D>().size = new Vector2(w / 100 , h / 100 );
            // cloneT = Instantiate(drawable_texture);
            

            // Initialize clean pixels to use
            // clean_colours_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];
            // for (int x = 0; x < clean_colours_array.Length; x++)
            //     clean_colours_array[x] = Reset_Colour;
            
            // Sprite s = Resources.Load<Sprite>("Logo/gsmatt2"); //원래 이미지
            

            sp_width = (int)drawable_sprite.rect.width; //(int)drawable_sprite.rect.width;
            
            
            // Initialize clean pixels to use
            clean_colours_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];
            clean_colours_array = drawable_texture.GetPixels();
            // Color32[] c = cloneT.GetPixels32();    
            // cloneT.SetPixels32(c);
            // cloneT.Apply();

            // Should we reset our canvas image when we hit play in the editor?
            if (Reset_Canvas_On_Play) {
                 ResetCanvas();
            }   
               
        }


        // void Update2()
        // {
        //     if (count >= 50000000) {
        //         count = 0;
               
        //         StartCoroutine( fadeOut() );
        //     }
           
           
        //     // Is the user holding down the left mouse button?
        //     bool mouse_held_down = Input.GetMouseButton(0);
        //     if (mouse_held_down && !no_drawing_on_current_drag)
        //     {
        //          Color32[] cc = drawable_texture.GetPixels32();
        //             if(cc != null) {
        //                 for(int i=0; i<cc.Length; i++) {
                        
        //                     if (cc[i].a == 0) {
        //                         // Debug.Log("0000000");
        //                         count++;
        //                     }
        //                 }
        //             }

        //         particle.SetActive(true);
        //         coin.SetActive(true);
              
                
        //         // Convert mouse coordinates to world coordinates
        //         Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //         particle.transform.position = new Vector3(mouse_world_position.x, mouse_world_position.y, -0.5f) ;
        //         coin.transform.position = new Vector3(mouse_world_position.x + 0.4f, mouse_world_position.y+0.1f, -0.6f) ;
        //         // Debug.Log("mouse P = " + mouse_world_position);
        //         socketManager.sendDrawing( mouse_world_position );
        //     }else {
        //         particle.SetActive(false);
        //         coin.SetActive(false);
        //     }
        // }

        public bool fadding = false;
        void Update()
        {
            // if (count >= 50008000) {
            //     count = 0;
            //     fadding = true;
            //     StartCoroutine( fadeOut() );
            // }
            
            if (fadding) {
                return;
            }
           
            // Is the user holding down the left mouse button?
            bool mouse_held_down = Input.GetMouseButton(0);
            if (mouse_held_down && !no_drawing_on_current_drag)
            {
                //  Color32[] cc = drawable_texture.GetPixels32();
                //     if(cc != null) {
                //         for(int i=0; i<cc.Length; i++) {
                        
                //             if (cc[i].a == 0) {
                //                 // Debug.Log("0000000");
                //                 count++;
                //             }
                //         }
                //     }

                particle.SetActive(true);
                coin.SetActive(true);
                // particle.transform.position = Input.mousePosition;
                
                // Convert mouse coordinates to world coordinates
                Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);   
                particle.transform.position = new Vector3(mouse_world_position.x, mouse_world_position.y, -0.5f) ;
                coin.transform.position = new Vector3(mouse_world_position.x + 0.4f, mouse_world_position.y+0.1f, -0.6f) ;
                // Debug.Log("mouse P = " + mouse_world_position);
                socketManager.sendDrawing( mouse_world_position );
                // Check if the current mouse position overlaps our image
                Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
                
                if (hit != null && hit.transform != null)
                    // We're over the texture we're drawing on! Change them pixel colours
                    ChangeColourAtPoint(mouse_world_position);
                else
                {
                    // We're not over our destination texture
                    previous_drag_position = Vector2.zero;
                    if (!mouse_was_previously_held_down)
                    {
                        // This is a new drag where the user is left clicking off the canvas
                        // Ensure no drawing happens until a new drag is started
                        no_drawing_on_current_drag = true;
                    }
                }
            }
            // Mouse is released
            else if (!mouse_held_down)
            {
                previous_drag_position = Vector2.zero;
                no_drawing_on_current_drag = false;
            }else {
                particle.SetActive(false);
            }
            mouse_was_previously_held_down = mouse_held_down;
        }

        
        private Vector2 v2BeforeDraw;

        // Pass in a point in WORLD coordinates
        // Changes the surrounding pixels of the world_point to the static pen_colour
        public void ChangeColourAtPoint(Vector2 world_point)
        {
            // Change coordinates to local coordinates of this image
            Vector3 local_pos = transform.InverseTransformPoint(world_point);

            // Change these to coordinates of pixels
            float pixelWidth = drawable_sprite.rect.width;
            float pixelHeight = drawable_sprite.rect.height;
            float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x;

            // Need to center our coordinates
            float centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
            float centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;

            // Round current mouse position to nearest pixel
            Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

            cur_colors = drawable_texture.GetPixels32();
            // Debug.Log("cur color length = " + cur_colors.Length );
            if (previous_drag_position == Vector2.zero)
            {
                // If this is the first time we've ever dragged on this image, simply colour the pixels at our mouse position
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
            }
            else
            {
                // Colour in a line from where we were on the last update call
                if(Vector2.Distance(v2BeforeDraw, previous_drag_position) > 10){
                    
                    v2BeforeDraw = previous_drag_position;
                    ColourBetween(previous_drag_position, pixel_pos);

                }
                
            }
            ApplyMarkedPixelChanges();

            //Debug.Log("Dimensions: " + pixelWidth + "," + pixelHeight + ". Units to pixels: " + unitsToPixels + ". Pixel pos: " + pixel_pos);
            previous_drag_position = pixel_pos;
        }


        // Set the colour of pixels in a straight line from start_point all the way to end_point, to ensure everything inbetween is coloured
        public void ColourBetween(Vector2 start_point, Vector2 end_point)
        {
            // Get the distance from start to finish
            float distance = Vector2.Distance(start_point, end_point);
            Vector2 direction = (start_point - end_point).normalized;

            Vector2 cur_position = start_point;

            // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
            float lerp_steps = 1 / distance;

            for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
            {
                cur_position = Vector2.Lerp(start_point, end_point, lerp);
                MarkPixelsToColour(cur_position, Pen_Width, Pen_Colour);
            }
        }




      
        public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            int extra_radius = Mathf.Min(0, pen_thickness - 2);

            Vector2 center_p = new Vector2(center_x, center_y);
            int left_x = center_x - pen_thickness;
            int right_x = center_x + pen_thickness;
            int left_y = center_y - pen_thickness;
            int right_y = center_y + pen_thickness;

            for (int x = left_x; x <= right_x; x+=4)
            {
                // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
                if (x >= sp_width || x < 0) {

                    continue;  
                }     
                    
                for (int y = left_y; y <= right_y; y+=22)
                {
                    
                    

                    if (x >= sp_width || x < 0) {

                        continue;  
                    }     
                  
                    if(Vector2.Distance(new Vector2(x, y), center_p ) < pen_thickness){

                         MarkPixelToChange(x, y, color_of_pen);
                    }

                }
            }

        }

        public void MarkPixelToChange(int x, int y, Color color)
        {
            
            // Need to transform x and y coordinates to flat coordinates of array
            int array_pos = y * (int)drawable_sprite.rect.width + x;

            // if (cur_colors[array_pos] == color) {
            //     Debug.Log(" already color ");
            //     return;
            // }
            // Check if this is a valid position
            if (array_pos > (cur_colors.Length - 1) || array_pos < 0)
                return;

            
            cur_colors[array_pos] = color;
        }
        public void ApplyMarkedPixelChanges()
        {
            drawable_texture.SetPixels32(cur_colors);
            drawable_texture.Apply();
        }


        // Directly colours pixels. This method is slower than using MarkPixelsToColour then using ApplyMarkedPixelChanges
        // SetPixels32 is far faster than SetPixel
        // Colours both the center pixel, and a number of pixels around the center pixel based on pen_thickness (pen radius)
        public void ColourPixels(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    drawable_texture.SetPixel(x, y, color_of_pen);
                }
            }

            drawable_texture.Apply();
        }


        // Changes every pixel to be the reset colour
        public void ResetCanvas()
        {
            drawable_texture.SetPixels(clean_colours_array);
            drawable_texture.Apply();
        }

        public void clickButton() {

            // Texture2D sp =  this.GetComponent<SpriteRenderer>().sprite.texture;
            // sp = cloneT;
            // sp.Apply();
            
           
            buttonParticle.SetActive(false);
            nextPopup.SetActive(false);
            count = 0;
            fadding = false;
            spRenderer.color = new Color(255,255,255,255);
            socketManager.sendReset();
            drawable_texture.SetPixels( clean_colours_array );
            drawable_texture.Apply();
            ResetCanvas();
        }


       	private float animTime = 2.0f;
	    private float time = 0f;
        IEnumerator fadeOut() {

            Color color = spRenderer.color;
            time = 0f;
            color.a = Mathf.Lerp(1f, 0f, time);

            while ( color.a > 0f ) {
                
                time += Time.deltaTime / animTime;
                color.a = Mathf.Lerp(1f, 0f, time);
                spRenderer.color = color;

                yield return null;
            }
            // gameObject.SetActive(false);
	    }

        public void startFade() {

            StartCoroutine (fadeOut());
            count = 0;
            fadding = true;
        }

    

    }

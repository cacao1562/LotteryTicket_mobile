using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Select : MonoBehaviour {

	public GameObject plane;
	public SpriteRenderer sp;
	public Sprite mSprite;
	public Text ttt;

	// void Awake()
	// {
	// 	DontDestroyOnLoad(this);
	// }
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// public void clickButton() {
	// 	NativeGallery.GetImageFromGallery();
	// 	NativeGallery.GetImageFromGallery( NativeGallery.MediaPickCallback callback, string title = "", string mime = "image/*", int maxSize = -1 );
	// }

	public void PickImage( int maxSize )
	{
		NativeGallery.Permission permission = NativeGallery.GetImageFromGallery( ( path ) =>
		{
			Debug.Log( "Image path: " + path );
			ttt.text = path;
		

			if( path != null )
			{

				
				
				// Create Texture from selected image
				Texture2D texture = NativeGallery.LoadImageAtPath( path, maxSize );
				// #if UNITY_EDITOR
				// AssetDatabase.CreateAsset(texture, "Assets/Resources/Logo/gg.png");
				// string assetPath = AssetDatabase.GetAssetPath(texture);
				// var tImporter = AssetImporter.GetAtPath( assetPath ) as TextureImporter;
				// if ( tImporter != null ) {
				// 	// tImporter.textureType = TextureImporterType.Advanced;
				// 	tImporter.isReadable = true;

				// 	AssetDatabase.ImportAsset( assetPath );
				// 	AssetDatabase.Refresh();
				// }
				// #endif
				

				// Rect rect = new Rect(0,0, texture.width, texture.height);
				// mSprite = Sprite.Create(texture, rect , new Vector2(0.5f, 0.5f));
				// sp.sprite = mSprite;
				PlayerPrefs.SetString("path", path);
				PlayerPrefs.SetInt("width", texture.width);
				PlayerPrefs.SetInt("height", texture.height );
				PlayerPrefs.Save();
				// var sss = Sprite.Create(texture, rect , new Vector2(0.5f, 0.5f));
				
				SceneManager.LoadScene("main");

				if( texture == null )
				{
					Debug.Log( "Couldn't load texture from " + path );
					return;
				}

				// Assign texture to a temporary quad and destroy it after 5 seconds
				// GameObject quad = GameObject.CreatePrimitive( PrimitiveType.Quad );
				// quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
				// quad.transform.forward = Camera.main.transform.forward;
				// quad.transform.localScale = new Vector3( 1f, texture.height / (float) texture.width, 1f );
				
				// Material material = quad.GetComponent<Renderer>().material;
				// if( !material.shader.isSupported ) // happens when Standard shader is not included in the build
				// 	material.shader = Shader.Find( "Legacy Shaders/Diffuse" );

				// material.mainTexture = texture;
				// plane.GetComponent<MeshRenderer>().material = material;	
				// Destroy( quad, 5f );

				// If a procedural texture is not destroyed manually, 
				// it will only be freed after a scene change
				// Destroy( texture, 5f );
			}
		}, "Select a PNG image", "image/png", maxSize );

		Debug.Log( "Permission result: " + permission );
	}



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System;
using System.Reflection;
using UnityEditor;

public class AddPanelsList : MonoBehaviour
{
    public string folderPath;
    public GameObject scrollView;
    public GameObject buttonPrefab;
    public GameObject planes;

    // Start is called before the first frame update
    void Start()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>(folderPath);

        // For every image in the folder path
        foreach (var texture in textures) {
            Debug.Log("Width: " + texture.width + ", Height: " + texture.height);
            int width;
            int height;
            // Get original image size
            GetImageSize(texture, out width, out height);

            float scaleFactor = 0.00001f;
            // Constrain the dimensions
            float imageWidth = width * scaleFactor;
            float imageHeight = height * scaleFactor;
            Debug.Log("Width: " + width + ", " + "Height: " + height);

            float textureWidth = texture.width;
            float textureHeight = texture.height;

            // Create a new button
            GameObject buttonGameObject = Instantiate(buttonPrefab);
            buttonGameObject.transform.SetParent(scrollView.transform, false);

            // Assign the click function
            Button button = buttonGameObject.GetComponent<Button>();
            button.onClick.AddListener(() => OnPanelClick(texture, imageWidth, imageHeight));

            // Add the image to the button
            Image imageComponent = buttonGameObject.GetComponent<Image>();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            imageComponent.sprite = sprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPanelClick(Texture2D texture, float imageWidth, float imageHeight)
    {
        foreach (Transform plane in planes.transform)
        {
            // Debug.Log("Width:" + plane.transform.localScale.x + ", Height:" + plane.transform.localScale.z);

            // Get the dimensions of the wall

            // Create a new material for the wall
            Material material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            material.mainTexture = texture;
            // Set RenderFace to Both
            material.SetFloat("_Cull", (float) CullMode.Off);
            
            // Obtain the plane's dimensions
            float planeWidth = plane.transform.localScale.x;
            float planeHeight = plane.transform.localScale.z;

            // Set the tiling values
            float tileX = planeWidth / imageWidth;
            float tileY = planeHeight / imageHeight;
            material.mainTextureScale = new Vector2(tileX, tileY);

            // Apply the material to the plane
            MeshRenderer planeRenderer = plane.gameObject.GetComponent<MeshRenderer>();
            planeRenderer.material = material;
        }
    }

    // Get original size of image
    public static bool GetImageSize(Texture2D asset, out int width, out int height)
    {
        if (asset != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
    
            if (importer != null)
            {
                object[] args = new object[2] { 0, 0 };
                MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);
    
                width = (int)args[0];
                height = (int)args[1];
    
                return true;
            }
        }
    
        height = width = 0;
        return false;
    }
}

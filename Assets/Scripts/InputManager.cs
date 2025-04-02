using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public Transform player;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameManager.Instance.smoothTerrains();
            
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.Instance.flatShadTerrains();
            
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameManager.Instance.resetTerrains();
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            LevelManager.setNextStrategy();
            GameManager.Instance.resetTerrains();
            
        }

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            CubesData.dim++;
            CubesData.rotation = player.rotation;
            CubesData.cameraRotation = Camera.main.transform.rotation;
            CubesData.position = player.position;
            SceneManager.LoadScene(0);
        }
        
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            CubesData.dim--;
            CubesData.rotation = player.rotation;
            CubesData.cameraRotation = Camera.main.transform.rotation;
            
            CubesData.position = player.position;
            CubesData.dim = Mathf.Max(1, CubesData.dim - 1);
            SceneManager.LoadScene(0);
        }
    }
}

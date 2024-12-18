using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Level
{
    public List<Transform> levelParts;
}
public class GenerateLevels : MonoBehaviour
{
    public int currentLevelPart = 0;
    public int currentLevel = 2;
    public List<Level> levels;
    [SerializeField] private Transform player;
    [SerializeField] private float distanceForCreation;
    [SerializeField] private float distanceForDeletion;
    private List<Transform> generatedLevels = new List<Transform>();
    private Transform previousObject;
    private Transform start;
    private Transform end;

    void Start()
    {
        OnLevelStart();    
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            DeleteGeneratedLevels();

            if (Vector3.Distance(previousObject.position, player.position) < distanceForCreation)
            {
                if (levels[currentLevel].levelParts.Count != currentLevelPart)
                    GenerateLevel();

            }
        }
               
    }

    public void OnLevelStart()
    {
        start = levels[currentLevel].levelParts[currentLevelPart].Find("Start");
        end = levels[currentLevel].levelParts[currentLevelPart].Find("End");
        previousObject = Instantiate(levels[currentLevel].levelParts[currentLevelPart], levels[currentLevel].levelParts[currentLevelPart].position, Quaternion.identity, this.transform);
        generatedLevels.Add(previousObject);
        currentLevelPart++;
    }

    public void DeleteGeneratedLevels(bool noCondition = false)
    {
        if (!noCondition)
        {
            foreach (Transform e in generatedLevels.ToList())
            {

                if (Vector3.Distance(player.position, e.position) > distanceForDeletion && e.position.x < player.position.x)
                {
                    if (!e.IsDestroyed())
                    {
                        Destroy(e.gameObject);
                    }
                    generatedLevels.Remove(e);
                }
            }
        }
        else
        {
            foreach (Transform e in generatedLevels.ToList())
            {
                if (!e.IsDestroyed())
                {
                    Destroy(e.gameObject);
                }
                generatedLevels.Remove(e);
            }
        }
        

    }

    private void GenerateLevel()
    {
        
        Transform newObject = 
            Instantiate(levels[currentLevel].levelParts[currentLevelPart], end.position + (previousObject.position - start.position), Quaternion.identity, this.transform);
        generatedLevels.Add(newObject);
        previousObject = newObject;
        start.position = newObject.Find("Start").position;
        end.position = newObject.Find("End").position;
        currentLevelPart++;
        
        
    }
}

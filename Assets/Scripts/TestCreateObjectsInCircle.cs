using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCreateObjectsInCircle : MonoBehaviour
{
    public GameObject prefabToInstantiate;

    /// <summary>
    ///     Instantiates prefabs around center splited equality. 
    ///     The number of times indicated in <see cref="howMany" /> var is
    ///     the number of parts will be the circle cuted, with taking as a center the location,
    ///     and adding radius from it
    /// </summary>
    /// <param name="prefab">The object it will be intantiated</param>
    /// <param name="location">The center point of the circle</param>
    /// <param name="howMany">The number of parts the circle will be cut</param>
    /// <param name="radius">
    ///     The margin from center, if your center is at (1,1,1) and your radius is 3 
    ///     your final position can be (4,1,1) for example
    /// </param>
    /// <param name="yPosition">The yPostion for the instantiated prefabs</param>
    public void InstantiateInCircle(GameObject prefab, Vector3 location, int howMany, float radius, float yPosition)
    {
        float angleSection = Mathf.PI * 2f / howMany;
        for (int i = 0; i < howMany; i++)
        {
            float angle = i * angleSection;
            Vector3 newPos = location + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            newPos.y = yPosition;
            Instantiate(prefab, newPos, prefab.transform.rotation);
        }
    }
    public void InstantiateInCircle(GameObject prefab, Vector3 location, int howMany, float radius)
    {
        this.InstantiateInCircle(prefab, location, howMany, radius, location.y);
    }
    public void InstantiateInCircle(GameObject prefab, int howMany, float radius)
    {
        this.InstantiateInCircle(prefab, this.transform.position, howMany, radius);
    }

    private void Start()
    {
        this.InstantiateInCircle(this.prefabToInstantiate, new Vector3(0, 0, 0), 24, 2, 2);
    }
}

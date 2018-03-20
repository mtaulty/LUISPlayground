using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LUISIntentHandlers : MonoBehaviour
{
    public void OnIntentCreate(LUIS.Results.QueryResultsEntity[] entities)
    {
        // We need two pieces of information here - the shape type and
        // the distance.
        var entityShapeType = entities.FirstOrDefault(e => e.type == "shapeType");
        var entityDistance = entities.FirstOrDefault(e => e.type == "builtin.number");

        if (entityShapeType != null)
        {
            var cameraPos = Camera.main.transform.position;
            var forward = Vector3.Normalize(Camera.main.transform.forward);

            var distance = 
                entityDistance == null ? 1.0f : float.Parse(entityDistance.entity);

            var position = cameraPos + (forward * distance);

            var newObject = GameObject.CreatePrimitive(
                entityShapeType.entity == "cube" ? PrimitiveType.Cube : PrimitiveType.Sphere);

            newObject.transform.position = position;
            newObject.transform.localScale *= 0.25f;

            newObject.tag = entityShapeType.entity;
        }
    }
    public void OnIntentDeleteAll(LUIS.Results.QueryResultsEntity[] entities)
    {
        this.DestroyTaggedObjects("cube");
        this.DestroyTaggedObjects("sphere");
    }
    public void OnIntentDeleteType(LUIS.Results.QueryResultsEntity[] entities)
    {
        var entityShapeType = entities.FirstOrDefault(e => e.type == "shapeType");

        if (entityShapeType != null)
        {
            // We hit pluralisation here. Need to sort that but for now.
            this.DestroyTaggedObjects(entityShapeType.entity.TrimEnd('s'));
        }
    }
    void DestroyTaggedObjects(string tag)
    {
        var objectsWithTag = GameObject.FindGameObjectsWithTag(tag);

        if (objectsWithTag != null)
        {
            foreach (var gameObject in GameObject.FindGameObjectsWithTag(tag))
            {
                Destroy(gameObject);
            }
        }
    }
}

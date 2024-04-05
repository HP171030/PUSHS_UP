using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public void IgnoreCollision(string layer1, string layer2, bool ignore)
    {
        int layer1Index = LayerMask.NameToLayer(layer1);
        int layer2Index = LayerMask.NameToLayer(layer2);

        if (layer1Index == -1 || layer2Index == -1)
        {
            Debug.LogError("Invalid layer name provided.");
            return;
        }

        Physics.IgnoreLayerCollision(layer1Index, layer2Index, ignore);
    }
}

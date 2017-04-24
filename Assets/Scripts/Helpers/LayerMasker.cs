using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    
public static class LayerMasker {
    
	public static int GetLayerMask(List<Layers> layersToCollide){ 
        int layerMask=0;
        layersToCollide.ForEach(layer=>{ 
            layerMask |= 1<<(int)layer;
        });
        return layerMask;
    }
    public static int GetLayerMask(Layers layer){ 
        return 1<<(int)layer;
    }
    public static int GetLayerMaskInverse(Layers layer){ 
        return ~(1<<(int)layer);
    }
    public static int GetLayerMaskInverse(List<Layers> layersToCollide){ 
        return ~GetLayerMask(layersToCollide);
    }
    public static List<Layers> Damageable= new List<Layers>(){Layers.Enemy, Layers.ModMan };
}

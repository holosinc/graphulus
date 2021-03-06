﻿using UnityEngine;

public class Edge : MonoBehaviour {
    public GameObject source, target;
    public Springy.Edge springyEdge;

    private static Object _lightResource;

    private void Awake() {
        _lightResource = Resources.Load("Light");

        // draw edges before nodes
        GetComponent<Renderer>().material.renderQueue = 0;
    }

    private void LateUpdate() {
        GetComponent<LineRenderer>().SetPosition(0, source.transform.position);
        GetComponent<LineRenderer>().SetPosition(1, target.transform.position);

        // randomly spawn light from source node to target
        const float spawnLightProbability = 0.0008f;
        if (Random.value <= spawnLightProbability) {
            SpawnLight();
        }
    }

    private void SpawnLight() {
        var light = (GameObject)Instantiate(_lightResource);
        light.transform.parent = transform;

        // random duration and easing
        var duration = Random.Range(0.5f, 4f);
        System.Func<float, float>[] easings = { Easing.EaseOutCubic, Easing.EaseOutQuad, Easing.EaseOutQuart, Easing.EaseOutQuint };
        var ease = easings[Random.Range(0, easings.Length)];

        // start animation
        GameSystem.Instance.Execute(new Job {
            OnStart = () => {
                light.transform.position = source.transform.position;
                light.SetActive(true);
            },
            Update = (_, t) => {
                if (light == null) {
                    return false;
                }

                t = ease(t);
                light.transform.position = Vector3.Lerp(source.transform.position, target.transform.position, t);
                return true;
            },
            OnEnd = () => {
                if (light != null) {
                    Destroy(light);
                }
            }
        }, duration);
    }
}

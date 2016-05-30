﻿using System;
using System.Collections.Generic;

using UnityEngine;


public class World : MonoBehaviour
{
    public bool debugModeEnabled { get; set; }

    private Springy.ForceDirectedGraph forceDirectedGraph { get; set; }

    private bool textRenderingEnabled, edgeRenderingEnabled;

    private int frameCount;
    private float fps, avgDeltaTime, timeElapsed;


    void Start()
    {
        debugModeEnabled = true;

        forceDirectedGraph = new Springy.ForceDirectedGraph();
        forceDirectedGraph.stiffness = 300f;
        forceDirectedGraph.repulsion = 400f;
        forceDirectedGraph.damping = 0.5f;
        forceDirectedGraph.running = true;

        var nodes = new List<GameObject>();
        var edges = new List<GameObject>();
        JsonLoader.Deserialize("Examples/miserables.json", nodes, edges);

        foreach (var node in nodes)
        {
            node.transform.parent = gameObject.transform;
            node.GetComponent<Node>().SpringyNode = forceDirectedGraph.newNode(node.GetComponent<Node>().Text);
        }
        foreach (var edge in edges)
        {
            edge.transform.parent = gameObject.transform;
            forceDirectedGraph.newEdge(
                edge.GetComponent<Edge>().Source.GetComponent<Node>().Id,
                edge.GetComponent<Edge>().Target.GetComponent<Node>().Id,
                edge.GetComponent<Edge>().Length
            );
        }

        textRenderingEnabled = true;
        edgeRenderingEnabled = true;
    }

    void Update()
    {
        // update simulation
        forceDirectedGraph.tick(Time.deltaTime);

        // enable/disable debug menu
        if (Input.GetKeyDown(KeyCode.Space))
        {
            debugModeEnabled = !debugModeEnabled;
        }
        if (debugModeEnabled)
        {
            updateDebug();
        }

        // keep track of stats
        frameCount++;
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= 1f)
        {
            fps = frameCount;
            avgDeltaTime = timeElapsed / frameCount;
            frameCount = 0;
            timeElapsed = 0f;
        }
    }

    void OnGUI()
    {
        if (debugModeEnabled)
        {
            var debug =
                String.Format("FPS: {0:f} [{1:f} ms]\n", fps, avgDeltaTime * 1000f) +
                "\n" +
                String.Format("Text rendering: {0}\n", textRenderingEnabled ? "ON" : "OFF") +
                String.Format("Edge rendering: {0}\n", edgeRenderingEnabled ? "ON" : "OFF");

            GUI.TextArea(new Rect(Screen.width - 250 - 10, 10, 250, Screen.height - 20), debug);
        }
    }


    void updateDebug()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // enable/disable text rendering of nodes
            var nodes = GameObject.FindGameObjectsWithTag("Node");
            foreach (var node in nodes)
            {
                node.transform.Find("Text").GetComponent<Renderer>().enabled = !node.transform.Find("Text").GetComponent<Renderer>().enabled;
            }
            textRenderingEnabled = !textRenderingEnabled;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            // enable/disable edge rendering
            var edges = GameObject.FindGameObjectsWithTag("Edge");
            foreach (var edge in edges)
            {
                edge.GetComponent<Renderer>().enabled = !edge.GetComponent<Renderer>().enabled;
            }
            edgeRenderingEnabled = !edgeRenderingEnabled;
        }
    }
}

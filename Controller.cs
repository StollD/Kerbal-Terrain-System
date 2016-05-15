/**
 * Kerbal Terrain Systems
 * Deformable Terrain for Kerbal Space Program
 * Copyright (c) Thomas P. 2016
 * Licensed under the terms of the MIT License
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Timers;
using UnityEngine;

namespace KerbalTerrainSystem
{
    /// <summary>
    /// Class to add a Module to all vessels, to register Deformation
    /// </summary>
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class CollisionController : MonoBehaviour
    {
        /// <summary>
        /// Whether a part is exploding
        /// </summary>
        public Boolean isExploding { get; set; }

        /// <summary>
        /// The enqueued Deformations
        /// </summary>
        public Queue<Deformation> deformations { get; set; }
        
        /// <summary>
        /// Register the Vessel-Handler and stop the GarbageCollection
        /// </summary>
        void Start()
        {
            // Register the Handler
            GameEvents.onPartDie.Add(onPartDie);

            // Stop the GC
            DontDestroyOnLoad(this);

            // Create the Queue
            deformations = new Queue<Deformation>();

            // Add the Terrain-Handler to all bodies
            foreach (CelestialBody body in PSystemManager.Instance.localBodies.Where(b => b.pqsController != null))
            {
                GameObject modObject = new GameObject("TerrainDeformation");
                modObject.transform.parent = body.pqsController.transform;
                PQSMod_TerrainDeformation mod = modObject.AddComponent<PQSMod_TerrainDeformation>();
                mod.deformations = new List<Deformation>();
                mod.order = Int32.MaxValue; // Make sure that we're the last mod
                mod.modEnabled = true;
                mod.sphere = body.pqsController;
            }

            // Start the Coroutine
            StartCoroutine(Update());
            GameEvents.onGameSceneLoadRequested.Add(scene => StartCoroutine(Update()));
        }

        /// <summary>
        /// If a part dies, rebuild the sphere
        /// </summary>
        void onPartDie(Part part)
        {
            // If there's no part or vessel, abort
            if (part == null || part.vessel == null)
                return;

            // Get the CelestialBody
            CelestialBody body = part.vessel.mainBody;

            // Get the Vessel
            Vessel vessel = part.vessel;

            // Create the Deformation
            Deformation deformation = new Deformation
            {
                position = Utility.LLAtoECEF(vessel.latitude, vessel.longitude, vessel.terrainAltitude, body.Radius),
                vPos = vessel.vesselTransform.position,
                altitude = vessel.terrainAltitude + body.Radius,
                body = body,
                surfaceSpeed = vessel.srfSpeed,
                mass = part.mass + part.GetResourceMass(),
                srfAngle = Vector3d.Angle(Vector3d.up, vessel.vesselTransform.forward)
            };
            deformations.Enqueue(deformation);
        }

        /// <summary>
        /// Apply the enqueued deformations
        /// </summary>
        private IEnumerator Update()
        {
            // Loops
            while (true)
            {
                if (deformations.Any())
                {
                    // Get the latest deformation
                    Deformation deformation = deformations.Dequeue();

                    // Check for no deformation
                    if (deformation.GetDiameter() != 0d)
                    {
                        // Apply it
                        deformation.body.GetComponentInChildren<PQSMod_TerrainDeformation>().deformations.Add(deformation);

                        // Select the matching PQS quads
                        PQ[] quads = Utility.FindNearbyQuads(deformation.body.pqsController, deformation.vPos, 9);
                        yield return null;

                        // Select Quads
                        foreach (PQ quad in quads)
                        {
                            quad.isBuilt = false;
                            quad.isActive = true;
                            quad.sphereRoot.quadAllowBuild = true;
                            quad.isCached = false;
                            quad.Build();
                            typeof(PQS).GetMethod("UpdateEdgeNormals", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(deformation.body.pqsController, new[] { quad });
                            yield return null;
                        }
                    }
                }
                yield return null;
            }
        }
    }
}

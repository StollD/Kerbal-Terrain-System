/**
 * Kerbal Terrain Systems
 * Deformable Terrain for Kerbal Space Program
 * Copyright (c) Thomas P. 2016
 * Licensed under the terms of the MIT License
 */

using System;
using System.Linq;

namespace KerbalTerrainSystem
{
    /// <summary>
    /// Class to save / load Deformation per Savegame
    /// </summary>
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.SPACECENTER, GameScenes.FLIGHT, GameScenes.EDITOR, GameScenes.TRACKSTATION)]
    public class TerrainDeformation : ScenarioModule
    {
        // Load the saved Deformations
        public override void OnLoad(ConfigNode node)
        {
            // Loop through all child-nodes
            foreach (ConfigNode bodyNode in node.nodes)
            {
                // Get the CelestialBody
                CelestialBody body = PSystemManager.Instance.localBodies.Find(b => b.transform.name == bodyNode.name);

                // Get the Deformation controller and clear it
                PQSMod_TerrainDeformation pqsDeformation = body.pqsController.GetComponentInChildren<PQSMod_TerrainDeformation>();
                pqsDeformation.deformations.Clear();

                // Build the Deformations
                foreach (ConfigNode deformationNode in bodyNode.nodes)
                {
                    Deformation deformation = ConfigNode.CreateObjectFromConfig<Deformation>(deformationNode);
                    pqsDeformation.deformations.Add(deformation);
                }
            }
        }

        // Save the Deformation
        public override void OnSave(ConfigNode node)
        {
            // Loop through all bodies with a PQS
            foreach (CelestialBody body in PSystemManager.Instance.localBodies.Where(b => b.pqsController != null))
            {
                // Get the Deformation controller
                PQSMod_TerrainDeformation pqsDeformation = body.pqsController.GetComponentInChildren<PQSMod_TerrainDeformation>();

                // Add a node for the body
                ConfigNode bodyNode = node.AddNode(body.transform.name);

                // Build the Deformations
                foreach (Deformation deformation in pqsDeformation.deformations)
                {
                    ConfigNode deformationNode = bodyNode.AddNode("Deformation");
                    ConfigNode.CreateConfigFromObject(deformation, deformationNode);
                }
            }
        }
    }
}
/* ============================================================================= *\
 * The MIT License (MIT) 
 * Copyright(c) 2015 Thomas P.

 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
\* ============================================================================= */

using System.Linq;
using UnityEngine;

namespace KerbalTerrainSystem
{
    namespace Deformation
    {
        /// <summary>Class to save / load Deformation per Savegame</summary>
        //[KSPScenario(ScenarioCreationOptions.AddToAllGames, new GameScenes[] { GameScenes.SPACECENTER, GameScenes.FLIGHT, GameScenes.EDITOR, GameScenes.TRACKSTATION })]
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
                        Vector3 position = ConfigNode.ParseVector3D(deformationNode.GetValue("position"));
                        double depth = double.Parse(deformationNode.GetValue("depth"));
                        double width = double.Parse(deformationNode.GetValue("width"));
                        Deformation deformation = new Deformation(position, body, depth, width);
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
                        deformationNode.AddValue("position", Utility.FormatVector3D(deformation.position));
                        deformationNode.AddValue("depth", deformation.depth);
                        deformationNode.AddValue("width", deformation.width);
                    }
                }
            }
        }
    }
}
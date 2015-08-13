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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerbalTerrainSystem
{
    namespace Deformation
    {
        /// <summary>Custom PQSMod to add deformation to the terrain</summary>
        public class PQSMod_TerrainDeformation : PQSMod
        {
            // List with Deformations
            public List<Deformation> deformations;

            // Manipulate the terrain
            public override void OnVertexBuildHeight(PQS.VertexBuildData data)
            {
                // Loop through the Deformations
                foreach (Deformation deformation in deformations)
                {
                    // Normalize the deformation position
                    Vector3d positionNorm = deformation.position.normalized;

                    // Get a "normalizer"
                    double normalized = deformation.position.normalized.x / deformation.position.x;

                    // Get the distance between the deformation and the VertexBuildData
                    float distance = (float)Vector3d.Distance(deformation.position.normalized, data.directionFromCenter);

                    // If we are near enough...
                    if (distance <= (deformation.width * 0.5) * normalized)
                    {
                        // ... lower height
                        double vertHeight = data.vertHeight - deformation.depth;
                        // If the body has an ocean, set a limit
                        if (sphere.mapOcean)
                            vertHeight = Math.Max(sphere.radius + 5d, vertHeight);

                        // Set the new height
                        data.vertHeight = vertHeight;
                    }
                }
            }
        }
    }
}
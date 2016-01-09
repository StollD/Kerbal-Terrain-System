/**
 * Kerbal Terrain Systems
 * Deformable Terrain for Kerbal Space Program
 * Copyright (c) Thomas P. 2016
 * Licensed under the terms of the MIT License
 */

using System;
using System.Collections.Generic;

namespace KerbalTerrainSystem
{
    /// <summary>
    /// Custom PQSMod to add deformation to the terrain
    /// </summary>
    public class PQSMod_TerrainDeformation : PQSMod
    {
        /// List with Deformations
        public List<Deformation> deformations;

        /// Manipulate the terrain
        public override void OnVertexBuildHeight(PQS.VertexBuildData data)
        {
            /// Loop through the Deformations
            foreach (Deformation deformation in deformations)
            {
                /// Normalize the deformation position
                Vector3d positionNorm = deformation.position.normalized;

                /// Get a "normalizer"
                double normalized = deformation.position.normalized.x / deformation.position.x;

                /// Get the distance between the deformation and the VertexBuildData
                float distance = (float)Vector3d.Distance(deformation.position.normalized, data.directionFromCenter);

                /// If we are near enough...
                if (distance <= (deformation.width * 0.5) * normalized)
                {
                    /// ... lower height
                    double vertHeight = data.vertHeight - deformation.depth;
                    /// If the body has an ocean, set a limit
                    if (sphere.mapOcean)
                        vertHeight = Math.Max(sphere.radius + 5d, vertHeight);

                    /// Set the new height
                    data.vertHeight = vertHeight;
                }
            }
        }
    }
}
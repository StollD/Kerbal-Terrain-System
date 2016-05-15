/**
 * Kerbal Terrain Systems
 * Deformable Terrain for Kerbal Space Program
 * Copyright (c) Thomas P. 2016
 * Licensed under the terms of the MIT License
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace KerbalTerrainSystem
{
    /// <summary>
    /// Custom PQSMod to add deformation to the terrain
    /// </summary>
    public class PQSMod_TerrainDeformation : PQSMod
    {
        // List with Deformations
        public List<Deformation> deformations;

        // Setup
        public override void OnSetup()
        {
            requirements |= PQS.ModiferRequirements.MeshColorChannel | PQS.ModiferRequirements.MeshCustomNormals;
        }

        // Manipulate the terrain
        public override void OnVertexBuildHeight(PQS.VertexBuildData data)
        {
            // Loop through the Deformations
            foreach (Deformation deformation in deformations)
            {
                // Get the angle between the two directions
                Double cos = Vector3d.Dot(deformation.position.normalized, data.directionFromCenter);

                // Determine the distance between the two points
                Double distance = Math.Sqrt(data.vertHeight * data.vertHeight + deformation.altitude * deformation.altitude - 2 * data.vertHeight * deformation.altitude * cos);

                // Determine whether the two positions are near enough
                if (distance <= deformation.GetDiameter() * 10)
                {
                    data.vertColor = Color.blue;
                }
            }
        }
    }
}
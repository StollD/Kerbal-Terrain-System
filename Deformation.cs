/**
 * Kerbal Terrain Systems
 * Deformable Terrain for Kerbal Space Program
 * Copyright (c) Thomas P. 2016
 * Licensed under the terms of the MIT License
 */

using System;
 
namespace KerbalTerrainSystem
{
    /// <summary>
    /// Class to store a Deformation
    /// </summary>
    public class Deformation
    {
        /// <summary>
        /// The positon where the part impacted
        /// </summary>
        public Vector3d position;

        /// <summary>
        /// The positon where the vessel impacted, in world space
        /// </summary>
        public Vector3d vPos;

        /// <summary>
        /// The altitude of the vessel
        /// </summary>
        public Double altitude;

        /// <summary>
        /// The angle relative to the surface
        /// </summary>
        public Double srfAngle;

        /// <summary>
        /// The mass of the part while impacting
        /// </summary>
        public Double mass;

        /// <summary>
        /// The speed of the part while impacting
        /// </summary>
        public Double surfaceSpeed;

        /// <summary>
        /// The Planet where the part impacted
        /// </summary>
        public CelestialBody body;

        /// <summary>
        /// Calculates the diameter of this deformation
        /// </summary>
        public Double GetDiameter()
        {
            return Math.Pow(body.GeeASL * 9.81, -1d/6d) * Math.Pow(Math.Sin(srfAngle) * mass * Math.Pow(surfaceSpeed, 2) + 0.5 * mass * Math.Pow(surfaceSpeed, 2), 1d/3d);
        }
    }
}
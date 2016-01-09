/**
 * Kerbal Terrain Systems
 * Deformable Terrain for Kerbal Space Program
 * Copyright (c) Thomas P. 2016
 * Licensed under the terms of the MIT License
 */

namespace KerbalTerrainSystem
{
    /// <summary>
    /// Class to store a Deformation
    /// </summary>
    public class Deformation
    {
        public Vector3d position;
        public double depth;
        public double width;
        public CelestialBody body;

        public Deformation(Vector3d position, CelestialBody body, double depth, double width)
        {
            this.position = position;
            this.body = body;
            this.depth = depth;
            this.width = width;
        }

        public Deformation() { }
    }
}
/**
 * Kerbal Terrain Systems
 * Deformable Terrain for Kerbal Space Program
 * Copyright (c) Thomas P. 2016
 * Licensed under the terms of the MIT License
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace KerbalTerrainSystem
{
    /// <summary>
    /// Class to store utility functions
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Convert latitude-longitude-altitude with body radius to a vector.
        /// </summary>
        public static Vector3d LLAtoECEF(double lat, double lon, double alt, double radius)
        {
            const double degreesToRadians = Math.PI / 180.0;
            lat = (lat - 90) * degreesToRadians;
            lon *= degreesToRadians;
            double x, y, z;
            double n = radius; // for now, it's still a sphere, so just the radius
            x = (n + alt) * -1.0 * Math.Sin(lat) * Math.Cos(lon);
            y = (n + alt) * Math.Cos(lat); // for now, it's still a sphere, so no eccentricity
            z = (n + alt) * -1.0 * Math.Sin(lat) * Math.Sin(lon);
            return new Vector3d(x, y, z);
        }

        /// <summary>
        /// Corutine to rebuild parts of a PQS sphere
        /// </summary>
        public static IEnumerator RebuildSphere(PQ[] quads, PQS pqs)
        {
            for (int i = 0; i < quads.Length; i++)
            {
                PQ quad = quads[i];
                quad.isBuilt = false;
                quad.isBuilt = pqs.BuildQuad(quad);
                pqs.GetType().GetMethod("UpdateEdgeNormals", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(pqs, new[] { quad });
                yield return null;
            }
        }

        /// <summary>
        /// Find quads that are near a transform
        /// </summary>
        public static PQ[] FindNearbyQuads(PQS pqsVersion, Transform transform, int count)
        {
            IEnumerable<PQ> quads = pqsVersion.GetComponentsInChildren<PQ>(true);
            quads = quads.OrderBy(q => Vector3.Distance(q.quadTransform.position, transform.position)).Take(count);
            return quads.ToArray();
        }
    }
}

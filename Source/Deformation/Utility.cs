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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using KerbalTerrainSystem.Deformation;

namespace KerbalTerrainSystem
{
    // Class to store utility functions
    public class Utility
    {
        /// <summary>Convert latitude-longitude-altitude with body radius to a vector.</summary>
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

        /// <summary>Corutine to rebuild parts of a PQS sphere</summary>
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

        /// <summary>Multiplies every value of a Vector</summary>
        public static float MultiplyVector(Vector3 vector) => vector.x * vector.y * vector.z;

        /// <summary>A basic crater-curve</summary>
        public static FloatCurve craterCurve
        {
            get
            {
                FloatCurve curve = new FloatCurve();
                for (float i = 0; i <= 1; i = i + 0.1f) curve.Add(i, (1f - i) * (1f - i));
                return curve;
            }
        }

        /// <summary>Find quads that are near a transform</summary>
        public static PQ[] FindNearbyQuads(PQS pqsVersion, Transform transform, int count)
        {
            IEnumerable<PQ> quads = pqsVersion.GetComponentsInChildren<PQ>(true);
            quads = quads.OrderBy(q => Vector3.Distance(q.quadTransform.position, transform.position)).Take(count);
            return quads.ToArray();
        }

        /// <summary>Format a Vector3d to bas saved in a ConfigNode</summary>
        public static string FormatVector3D(Vector3d vector)
        {
            string vectorString = vector.ToString();
            vectorString = vectorString.Replace("[", "");
            vectorString = vectorString.Replace("]", "");
            return vectorString;
        }
    }
}

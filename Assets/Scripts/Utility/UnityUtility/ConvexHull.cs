using System.Collections.Generic;
using DataStructures;

namespace Utility.UnityUtility
{
    public class ConvexHull
    {
        //
        // 2d space
        //

        //Jarvis March - slow but simple
        public static List<MyVector2> JarvisMarch_2D(HashSet<MyVector2> points)
        {
            List<MyVector2> pointsList = new List<MyVector2>(points);

            if (!CanFormConvexHull_2d(pointsList))
            {
                return null;
            }
        
            //Has to return a list and not hashset because the points have an order coming after each other
            List<MyVector2> pointsOnHull = JarvisMarchAlgorithm2D.GenerateConvexHull(pointsList);

            return pointsOnHull;
        }


        //Quickhull
        public static List<MyVector2> Quickhull_2D(HashSet<MyVector2> points, bool includeColinearPoints)
        {
            List<MyVector2> pointsList = new List<MyVector2>(points);

            if (!CanFormConvexHull_2d(pointsList))
            {
                return null;
            }

            //Has to return a list and not hashset because the points have an order coming after each other
            List<MyVector2> pointsOnHull = QuickhullAlgorithm2D.GenerateConvexHull(pointsList, includeColinearPoints);

            return pointsOnHull;
        }
        
        private static bool CanFormConvexHull_2d(List<MyVector2> points)
        {
            //First test of we can form a convex hull

            //If fewer points, then we cant create a convex hull
            if (points.Count < 3)
            {
                return false;
            }

            //Find the bounding box of the points
            //If the spread is close to 0, then they are all at the same position, and we cant create a hull
            AABB2 rectangle = new AABB2(points);

            if (!rectangle.IsRectangleARectangle())
            {
                return false;
            }

            return true;
        }
    }
}
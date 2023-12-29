﻿using System.Collections.Generic;
using System.Linq;
using DataStructures;
using UnityEngine;

namespace Utility.UnityUtility
{
    public static class JarvisMarchAlgorithm2D
    {
        public static List<MyVector2> GenerateConvexHull(List<MyVector2> points)
        {   
            List<MyVector2> pointsOnConvexHull = new List<MyVector2>();


            //Step 0. Normalize the data to range [0, 1] or everything will break at larger sizes :(
            //Make sure the data is already normalized!!!

            
            //Step 1. Find the vertex with the smallest x coordinate
            //If several points have the same x coordinate, find the one with the smallest y
            MyVector2 startPos = points[0];

            for (int i = 1; i < points.Count; i++)
            {
                MyVector2 testPos = points[i];

                //Because of precision issues, we use a small value to test if they are the same
                if (testPos.X < startPos.X || ((Mathf.Abs(testPos.X - startPos.X) < float.Epsilon && testPos.Y < startPos.Y)))
                {
                    startPos = points[i];
                }
            }

            //This vertex is always on the convex hull
            pointsOnConvexHull.Add(startPos);

            //But we can't remove it from the list of all points because we need it to stop the algorithm
            //points.Remove(startPos);



            //Step 2. Loop to find the other points on the hull
            MyVector2 previousPoint = pointsOnConvexHull[0];

            int counter = 0;

            while (true)
            {
                //We might have colinear points, so we need a list to save all points added this iteration
                List<MyVector2> pointsToAddToTheHull = new List<MyVector2>();


                //Pick next point randomly
                MyVector2 nextPoint = points[Random.Range(0, points.Count)];

                //If we are coming from the first point on the convex hull
                //then we are not allowed to pick it as next point, so we have to try again
                if (previousPoint.Equals(pointsOnConvexHull[0]) && nextPoint.Equals(pointsOnConvexHull[0]))
                {
                    counter += 1;

                    continue;
                }

                //This point is assumed to be on the convex hull
                pointsToAddToTheHull.Add(nextPoint);


                //But this randomly selected point might not be the best next point, so we have to see if we can improve
                //by finding a point that is more to the right
                //We also have to check if this point has colinear points if it happens to be on the hull
                for (int i = 0; i < points.Count; i++)
                {
                    MyVector2 testPoint = points[i];
                
                    //Dont test the point we picked randomly
                    //Or the point we are coming from which might happen when we move from the first point on the hull
                    if (testPoint.Equals(nextPoint) || testPoint.Equals(previousPoint))
                    {
                        continue;
                    }

                    //Where is the test point in relation to the line between the point we are coming from
                    //which we know is on the hull, and the point we think is on the hull
                    LeftOnRight pointRelation = GeometryUtility.IsPoint_Left_On_Right_OfVector(previousPoint, nextPoint, testPoint);

                    //The test point is on the line, so we have found a colinear point
                    if (pointRelation == LeftOnRight.On)
                    {
                        pointsToAddToTheHull.Add(testPoint);
                    }
                    //To the right = better point, so pick it as next point we want to test if it is on the hull
                    else if (pointRelation == LeftOnRight.Right)
                    {
                        nextPoint = testPoint;

                        //Clear colinear points because they belonged to the old point which was worse
                        pointsToAddToTheHull.Clear();

                        pointsToAddToTheHull.Add(nextPoint);

                        //We dont have to start over because the previous points weve gone through were worse
                    }
                    //To the left = worse point so do nothing
                }



                //Sort this list, so we can add the colinear points in correct order
                pointsToAddToTheHull = pointsToAddToTheHull.OrderBy(n => MyVector2.SqrMagnitude(n - previousPoint)).ToList();

                pointsOnConvexHull.AddRange(pointsToAddToTheHull);

                //Remove the points that are now on the convex hull, which should speed up the algorithm
                //Or will it be slower because it also takes some time to remove points?
                for (int i = 0; i < pointsToAddToTheHull.Count; i++)
                {
                    points.Remove(pointsToAddToTheHull[i]);
                }


                //The point we are coming from in the next iteration
                previousPoint = pointsOnConvexHull[pointsOnConvexHull.Count - 1];


                //Have we found the first point on the hull? If so we have completed the hull
                if (previousPoint.Equals(pointsOnConvexHull[0]))
                {
                    //Then remove it because it is the same as the first point, and we want a convex hull with no duplicates
                    pointsOnConvexHull.RemoveAt(pointsOnConvexHull.Count - 1);

                    //Stop the loop!
                    break;
                }


                //Safety
                if (counter > 100000)
                {
                    break;
                }

                counter += 1;
            }

            return pointsOnConvexHull;
        }
    }
}
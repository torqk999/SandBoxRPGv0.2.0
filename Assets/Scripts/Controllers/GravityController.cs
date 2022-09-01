using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GravityController : MonoBehaviour
{

    public float gConst = 0.5f;
    public List<GameObject> Affected = new List<GameObject>();
    public List<PointMass> PointMasses = new List<PointMass>();
    public List<StaticElliptical> SimpleOrbits;
    public GameObject PlanetoidPrefab;
    public void UpdateGravity()
    {
        AddForces();
    }

    void UpdateSimpleOrbits()
    {
        foreach (StaticElliptical orbit in SimpleOrbits)
        {
            /*
            ALPHA    = Xa, Ya, Za
            BETA     = Xb, Yb, Zb
            TARGET   = Xc, Yc, Zc
            ANGLE    = 0
            SLOPE    = M
            XY SHIFT = H, K

            First Equation: Rotational
            Yc = (sin0 / cos0) Xc + K
             */

            // 
            double H = 0;
            double K = 0;
            double A = orbit.Major;
            double B = orbit.Minor;

            // Delta Angle/Time to project
            double angle = 0;
            double M = Math.Sin(angle) / Math.Cos(angle);

            // Isolated X coefficients (0 = ax^2 + bx + c)
            double coA = ((M * M * A * A) / (B * B)) + 1;
            double coB = -2 * H;
            double coC = -(A * A) + (H * H);

            // Quadratic Equation (both solutions)
            double XcP = (-coB + Math.Pow(((coB * coB) - (4 * coA * coC)), .5)) / (2 * coA);
            double XcN = (-coB - Math.Pow(((coB * coB) - (4 * coA * coC)), .5)) / (2 * coA);


            double YcP = (M * XcP) + K;
            double YcN = (M * XcN) + K;
        }
    }

    void CreatePointMass()
    {
        GameObject newPlanetoid = Instantiate(PlanetoidPrefab);
        PointMass newPointMass = new PointMass();
        //newPointMass.MassTransform
    }

    void AddForces()
    {
        if (Affected.Count < 1)
            return;

        foreach (GameObject gameObject in Affected)
            foreach (PointMass mass in PointMasses)
            {
                double force = gConst * mass.Weight * (1 / (Math.Pow(Vector3.Distance(mass.MassTransform.position, gameObject.transform.position), 2)));
                Vector3 vector = mass.MassTransform.position - gameObject.transform.position;
                gameObject.GetComponent<Rigidbody>().AddForce(vector * (float)force, ForceMode.Acceleration);
            }

    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (PointMass mass in PointMasses)
            mass.CameraSnapPoint.LookAt(mass.MassTransform);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

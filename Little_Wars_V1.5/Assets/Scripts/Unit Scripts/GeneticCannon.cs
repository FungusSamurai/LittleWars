// Course:  CS4242
// Student name: John Doe
// Student ID: 00584923
// Assignment #2:#2
// DueDate: 10/23/17
// Signature:______________
// Score: ______________

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gene : IComparable<Gene>
{
    public float angle;
    public float power;

    public int fitness;

    public Gene(float a = 0, float p = 0)
    {
        angle = a;
        power = p;
    }

    public Gene(Gene g)
    {
        angle = g.angle;
        power = g.power;
    }

    public static void Crossbreed(ref Gene g1, ref Gene g2)
    {
        float temp = g1.angle;
        g1.angle = g2.angle;
        g2.angle = temp;
    }

    public static void Mutate(Gene g, float value, bool power)
    {
        if (power)
        {
            g.power = value;
        }
        else
        {
            g.angle = value;
        }
    }

    public int CompareTo(Gene g)
    {
        if (g.fitness > fitness)
        {
            return -1;
        }
        else if (g.fitness < fitness)
        {
            return 1;
        }
        else
        {
            if (g.angle < angle)
            {
                return 1;
            }
            else if (g.angle > angle)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}

public class Population
{
    public int size;
    public int generation;
    public int genLimit;
    public List<Gene> individuals;

    public int mutate = 30;
    public int crossbreed = 60;

    public Population(int genL, int s, System.Random rand = null)
    {
        generation = 0;
        genLimit = genL;
        size = s;
        individuals = new List<Gene>();

        rand = new System.Random((int)(Time.time));
        for (int i = 0; i < size; i++)
        {
            Gene g = new Gene();
            g.angle = rand.Next(40, 90);
            g.power = rand.Next(50, 101);
            individuals.Add(g);
        }
    }
}

public static class GeneticCannon
{
    public static HexCell goal;
    public static Population pop;

    public static Vector3 cannonPos;
    public static Vector3 cannonForward;
    public static Vector3 spawnPos;

    public static void SetCannonParam(Vector3 cp, Vector3 cF, Vector3 sP)
    {
        cannonPos = cp;
        cannonForward = cF;
        spawnPos = sP;
        goal = MapMaster.Map[0, 0];
    }

    public static IEnumerator SolveCannon()
    {
        pop = new Population(10, 5);

        yield return CalcFitness();

        while (pop.generation < pop.genLimit)
        {
            foreach (Gene g in pop.individuals)
            {
                Debug.Log("p " + g.power + " a " + g.angle + " f " + g.fitness);
            }

            Gene g1 = pop.individuals[0];
            Gene g2 = pop.individuals[1];

            if (pop.individuals[0].fitness < 9)
            {
                Gene g = pop.individuals[0];
                CannonMaster.CurrentCannon.SetAim(g.angle, g.power);
                Debug.Log("Winner " + g.power + " " + g.angle);
                break;
            }

            System.Random r = new System.Random((int)Time.time);
            if (r.Next(1, 101) <= pop.crossbreed)
            {
                Gene.Crossbreed(ref g1, ref g2);
                pop.individuals.RemoveAt(pop.individuals.Count - 1);
                pop.individuals.RemoveAt(pop.individuals.Count - 1);
                pop.individuals.Add(g1);
                pop.individuals.Add(g2);
            }

            foreach (Gene g in pop.individuals)
            {
                System.Random rand = new System.Random((int)Time.time);
                if (rand.Next(1, 101) <= pop.mutate)
                {
                    Gene.Mutate(g, r.Next(70, 101), true);
                }
                if (r.Next(1, 101) <= pop.mutate)
                {
                    Gene.Mutate(g, r.Next(40, 91), false);
                }
            }

            pop.generation++;
            Debug.Log(pop.generation);
            yield return CalcFitness();
        }
    }

    public static IEnumerator CalcFitness()
    {
        foreach (Gene g in pop.individuals)
        {
            yield return new WaitForSeconds(0.2f);
            CannonMaster.CurrentCannon.SetAim(g.angle, g.power);
            g.fitness = EndPoint(g.angle, 0.01f*g.power*CannonMaster.CurrentCannon.cannon_power);
        }
        pop.individuals.Sort();

        string s = "";

        foreach (Gene g in pop.individuals)
        {
            s += g.angle + " " + g.power + " " + g.fitness + " | ";
        }

        Debug.Log(s);

        yield return null;
    }

    public static int EndPoint(float shotAngle, float shotPower)
    {
        float g = Physics.gravity.y;
        float timetoTop = -1;
        float timetoBtm = -1;
        float hangTime = -1;
        float maxheight = -1;

        //float distance = -1;

        float shotHeight = CannonMaster.CurrentCannon.ShotSpawn.transform.position.y;

        float Vx = Time.fixedDeltaTime * shotPower * Mathf.Sin(shotAngle * Mathf.Deg2Rad);
        float Vy = Time.fixedDeltaTime * shotPower * Mathf.Cos(shotAngle * Mathf.Deg2Rad);

        timetoTop = ((-1 * Vy) / (g));

        maxheight = Vy * timetoTop + ((0.5f * g) * timetoTop * timetoTop) + shotHeight;

        timetoBtm = Mathf.Sqrt((maxheight - (goal.spawnPoint.transform.position.y + 1)) / (-1*g*0.5f));

        hangTime = timetoTop + timetoBtm;

        //distance = Vx * hangTime;

        //Vector3 endpoint = new Vector3(cannonPos.x, 0.0f, cannonPos.z) + cannonForward * distance + Vector3.up*(Vy*hangTime + (0.5f*g*hangTime*hangTime) + shotHeight);

        float time = 0.2f;
        int count = 10;
        float dy = Vy * time + (.5f * g * (time * time));
        while (time <= hangTime)
        {
            float dx = Vx * time;
            dy = Vy * time  + (.5f * g * (time * time));

            Vector3 currentpos = CannonMaster.CurrentCannon.ShotSpawn.transform.position + (CannonMaster.CurrentCannon.transform.parent.forward * dx) + (CannonMaster.CurrentCannon.transform.parent.up * dy);

            //GameObject.Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), currentpos, Quaternion.identity).GetComponent<SphereCollider>().enabled = false;

            foreach (Collider c in Physics.OverlapSphere(currentpos, 0.5f))
            {
                Debug.Log(c.gameObject.name);
                if (c.gameObject.tag == "Infantry" || c.gameObject.tag == "Cavalry")
                {
                    if (c.gameObject.GetComponent<Unit>().Player != 1)
                    {
                        if (c.gameObject.GetComponent<Unit>().CurrentHex.Q == goal.Q && c.gameObject.GetComponent<Unit>().CurrentHex.R == goal.R)
                        {
                            Debug.Log("hit person");
                            count -= 2;
                        }
                        else
                        {
                            count--;
                        }
                    }
                    else
                    {
                        count += 2;
                    }
                }
            }

            time += 0.2f;
        }
        return count;
    }
}
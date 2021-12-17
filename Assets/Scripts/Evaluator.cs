using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents.Sensors;
using System.IO;
class Evaluator{
    public int ThroughCars{
        get;
        private set;
    }
    private static Evaluator instance = null;
    private StreamWriter logThroughput, logCrash, logDistance, logBehavior, logFullData;
    private Evaluator(){
        ThroughCars = 0;
        NumCrash = 0;
        Time = 0;
        logThroughput = new StreamWriter("logThroughput.csv");
        logThroughput.WriteLine("time,num");
        logThroughput.Close();
        logCrash = new StreamWriter("logCrash.csv");
        logCrash.WriteLine("time,num");
        logCrash.Close();
        logDistance = new StreamWriter("logDistance.csv");
        logDistance.WriteLine("time,x,y,id,speed");
        logDistance.Close();
        logBehavior = new StreamWriter("logBehavior.csv");
        logBehavior.WriteLine("time,my_speed,forward,verticle,horizontal");
        logBehavior.Close();
        logFullData = new StreamWriter("logFullData.csv");
        logFullData.Write("flame,x,y,z,");
        for(int i = 0; i < 43; i++){
            logFullData.Write("x" + i.ToString() + ",");
        }
        logFullData.WriteLine("horizontal,vertical");
        logFullData.Close();
    }
    public static Evaluator getInstance(){
        if(instance == null){ instance = new Evaluator(); }
        return instance;
    }
    public int NumCrash{
        get;
        private set;
    }
    public double Time{
        get;
        private set;
    }
    public int addThroughCars(double time){
        ThroughCars++;
        this.Time = time;
        //Debug.Log("Through cars: " + ThroughCars.ToString() + " at " + Time.ToString());
        logThroughput = new StreamWriter("logThroughput.csv", true);
        logThroughput.WriteLine(Time.ToString() + "," + ThroughCars.ToString());
        logThroughput.Close();
        return ThroughCars;
    }

    public int addCrashCars(double time){
        NumCrash++;
        this.Time = time;
        //Debug.Log("Crash cars: " + NumCrash.ToString() + " at " + Time.ToString());
        logCrash = new StreamWriter("logCrash.csv", true);
        logCrash.WriteLine(Time.ToString() + "," + NumCrash.ToString());
        logCrash.Close();
        return NumCrash;
    }

    public void addHorizontalSensor(double time, double left, double right, int id, double speed){
        //Debug.Log("Crash cars: " + NumCrash.ToString() + " at " + Time.ToString());
        logDistance = new StreamWriter("logDistance.csv", true);
        logDistance.WriteLine(time.ToString() + "," + left.ToString() + "," + right.ToString() + "," + id.ToString() + "," + speed.ToString());
        logDistance.Close();
    }

    public void addFullData(double flame, Vector3 position, List<float> observations, float horizontal, float vertical){
        logFullData = new StreamWriter("logFullData.csv", true);
        logFullData.Write(flame.ToString() + "," + position.x.ToString() + "," + position.y.ToString() + "," + position.z.ToString() + ",");
        foreach(var v in observations){
            logFullData.Write(v.ToString() + ",");
        }
        logFullData.WriteLine(horizontal.ToString() + "," + vertical.ToString());
        logFullData.Close();
    }

    public void addBehavior(double time, double my_speed, bool foundCarForward, float[] vectorAction){
        //Debug.Log("Crash cars: " + NumCrash.ToString() + " at " + Time.ToString());
        logBehavior = new StreamWriter("logBehavior.csv", true);
        logBehavior.Write(time.ToString() + "," + my_speed.ToString() + ",");
        logBehavior.WriteLine(foundCarForward.ToString() + "," + vectorAction[0].ToString() + "," + vectorAction[1].ToString());
        logBehavior.Close();

    }
}
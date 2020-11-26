using UnityEngine;
using System.IO;
class Evaluator{
    public int ThroughCars{
        get;
        private set;
    }
    private static Evaluator instance = null;
    private StreamWriter logThroughput, logCrash;
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
}
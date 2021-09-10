﻿using System.Collections;
using System.Collections.Generic;
using MLAgents;
using MLAgents.Sensors;
using UnityEngine;

public class CarAgent : Agent
{
    public float speed = 10f;
    public float torque = 10f;
    private float prevVertical = 0f;
    private float prevHorizontal = 0f;

    public int score = 0;
    public bool resetOnCollision = true;

    public int id = 0;
    private int new_id = 0;

    private Transform _track, _prev_track;

    public int time = 0;
    public bool generateNew = true;
    public int generateInterval = 300;
    public float noise = 0.0f;

    private Vector3 _initPosition;
    private Quaternion _initRotation;
    private int _notMoveCount = 0;
    private Evaluator evaluator = Evaluator.getInstance();
    public override void Initialize()
    {
        GetTrackIncrement();
        _initPosition = transform.localPosition;
        _initRotation = transform.localRotation;
        time = 0;
        new_id = 0;
    }

    void Update()
    {
        if(!generateNew || id > 1) return;
        //Debug.Log(time);
        if(time > generateInterval){
            //Debug.Log("add new car");
            var gameObject = Instantiate(this, _initPosition, _initRotation);
            new_id += 2;
            gameObject.id = new_id;
            gameObject.transform.parent = this.transform.parent.gameObject.transform;
            gameObject.transform.localPosition = _initPosition;
            gameObject.transform.localRotation = _initRotation;
            gameObject.speed = Random.Range(5, 16);
            //Debug.Log(gameObject.transform);
            time = 0;
        }
    }

    private void MoveCar(float horizontal, float vertical, float dt)
    {
        if(generateNew){
            time++;
        }
        float distance = speed * vertical;
        transform.Translate(distance * dt * Vector3.forward);

        float rotation = horizontal * torque * 90f;
        transform.Rotate(0f, rotation * dt, 0f);
        prevHorizontal = horizontal;
        prevVertical = vertical;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        float horizontal = vectorAction[0];
        float vertical = vectorAction[1];
        vertical = Mathf.Clamp(vertical, -1.0f, 1.0f);
        horizontal = Mathf.Clamp(horizontal, -1.0f, 1.0f);

        var lastPos = transform.position;
        MoveCar(horizontal, vertical, Time.fixedDeltaTime);

        float reward = GetTrackIncrement();
        
        var moveVec = transform.position - lastPos;
        /*
        if(Vector3.Distance(lastPos, transform.position) < 0.05f){
            _notMoveCount += 1;
            if(_notMoveCount > 10){
                _notMoveCount = 0;
                SetReward(-1f);
                EndEpisode();
            }
        }
        //*/
        float angle = Vector3.Angle(moveVec, _track.forward);
        //float bonus = (1f - angle / 90f) * Mathf.Clamp01(vertical) * Time.fixedDeltaTime;
        //float bonus = (1f - angle / 90f) * Mathf.Clamp01(Mathf.Abs(vertical)) * Time.fixedDeltaTime;
        //float bonus = ((1f - angle / 90f) + vertical) * Time.fixedDeltaTime;
        float bonus = ((1f - angle / 90f) * Mathf.Clamp01(Mathf.Max(0, vertical)) + Mathf.Min(0, vertical)) * Time.fixedDeltaTime;
        AddReward(bonus + reward);
        if(foundCarBackward){
            evaluator.addBehavior(Time.realtimeSinceStartup, speed, false, vectorAction);
        }
        if(foundCarForward){
            evaluator.addBehavior(Time.realtimeSinceStartup, speed, true, vectorAction);
        }

        score += (int)reward;
    }

    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }

    private bool foundCarForward, foundCarBackward;
    public override void CollectObservations(VectorSensor vectorSensor)
    {
        float angle = Vector3.SignedAngle(_track.forward, transform.forward, Vector3.up);
        foundCarBackward = false;
        foundCarForward = false;

        vectorSensor.AddObservation(angle / 180f);
        //float speed, torque;
        string tag;
        //Quaternion rotation;
        Vector3 diff;
        //vectorSensor.AddObservation(ObserveRay(1.5f, .5f, 25f, out speed, out torque, out rotation, out tag));
        vectorSensor.AddObservation(ObserveRay(1.5f, .5f, 25f, out diff, out tag));
        //vectorSensor.AddObservation((180.0f + Quaternion.Angle(rotation, this.transform.rotation)) / 360.0f);
        //vectorSensor.AddObservation(speed);
        //vectorSensor.AddObservation(torque);
        vectorSensor.AddObservation(diff.x);
        vectorSensor.AddObservation(diff.y);
        if(tag == "car"){
            vectorSensor.AddObservation(1);
            foundCarForward = true;
        }
        else{
            vectorSensor.AddObservation(0);
        }
        if(tag == "wall"){
            vectorSensor.AddObservation(1);
        }
        else{
            vectorSensor.AddObservation(0);
        }
        //vectorSensor.AddObservation(ObserveRay(1.5f, 0f, 0f, out speed, out torque, out rotation, out tag));
        vectorSensor.AddObservation(ObserveRay(1.5f, 0f, 0f, out diff, out tag));
        //vectorSensor.AddObservation((180.0f + Quaternion.Angle(rotation, this.transform.rotation)) / 360.0f);
        //vectorSensor.AddObservation(speed);
        //vectorSensor.AddObservation(torque);
        vectorSensor.AddObservation(diff.x);
        vectorSensor.AddObservation(diff.y);
        if(tag == "car"){
            vectorSensor.AddObservation(1);
            foundCarForward = true;
        }
        else{
            vectorSensor.AddObservation(0);
        }
        if(tag == "wall"){
            vectorSensor.AddObservation(1);
        }
        else{
            vectorSensor.AddObservation(0);
        }
        //vectorSensor.AddObservation(ObserveRay(1.5f, -.5f, -25f, out speed, out torque, out rotation, out tag));
        vectorSensor.AddObservation(ObserveRay(1.5f, -.5f, -25f, out diff, out tag));
        //vectorSensor.AddObservation((180.0f + Quaternion.Angle(rotation, this.transform.rotation)) / 360.0f);
        //vectorSensor.AddObservation(speed);
        //vectorSensor.AddObservation(torque);
        vectorSensor.AddObservation(diff.x);
        vectorSensor.AddObservation(diff.y);
        if(tag == "car"){
            vectorSensor.AddObservation(1);
            foundCarForward = true;
        }
        else{
            vectorSensor.AddObservation(0);
        }
        if(tag == "wall"){
            vectorSensor.AddObservation(1);
        }
        else{
            vectorSensor.AddObservation(0);
        }
        //vectorSensor.AddObservation(ObserveRay(-1.5f, .5f, 155f, out speed, out torque, out rotation, out tag));
        vectorSensor.AddObservation(ObserveRay(-1.5f, .5f, 155f, out diff, out tag));
        //vectorSensor.AddObservation((180.0f + Quaternion.Angle(rotation, this.transform.rotation)) / 360.0f);
        //vectorSensor.AddObservation(speed);
        //vectorSensor.AddObservation(torque);
        vectorSensor.AddObservation(diff.x);
        vectorSensor.AddObservation(diff.y);
        if(tag == "car"){
            vectorSensor.AddObservation(1);
            foundCarBackward = true;
        }
        else{
            vectorSensor.AddObservation(0);
        }
        if(tag == "wall"){
            vectorSensor.AddObservation(1);
        }
        else{
            vectorSensor.AddObservation(0);
        }
        //vectorSensor.AddObservation(ObserveRay(-1.5f, 0, 180f, out speed, out torque, out rotation, out tag));
        vectorSensor.AddObservation(ObserveRay(-1.5f, 0f, 180f, out diff, out tag));
        //vectorSensor.AddObservation((180.0f + Quaternion.Angle(rotation, this.transform.rotation)) / 360.0f);
        //vectorSensor.AddObservation(speed);
        //vectorSensor.AddObservation(torque);
        vectorSensor.AddObservation(diff.x);
        vectorSensor.AddObservation(diff.y);
        if(tag == "car"){
            vectorSensor.AddObservation(1);
            foundCarBackward = true;
        }
        else{
            vectorSensor.AddObservation(0);
        }
        if(tag == "wall"){
            vectorSensor.AddObservation(1);
        }
        else{
            vectorSensor.AddObservation(0);
        }
        //vectorSensor.AddObservation(ObserveRay(-1.5f, -.5f, -155f, out speed, out torque, out rotation, out tag));
        vectorSensor.AddObservation(ObserveRay(-1.5f, -.5f, -155f, out diff, out tag));
        //vectorSensor.AddObservation((180.0f + Quaternion.Angle(rotation, this.transform.rotation)) / 360.0f);
        //vectorSensor.AddObservation(speed);
        //vectorSensor.AddObservation(torque);
        vectorSensor.AddObservation(diff.x);
        vectorSensor.AddObservation(diff.y);
        if(tag == "car"){
            vectorSensor.AddObservation(1);
            foundCarBackward = true;
        }
        else{
            vectorSensor.AddObservation(0);
        }
        if(tag == "wall"){
            vectorSensor.AddObservation(1);
        }
        else{
            vectorSensor.AddObservation(0);
        }
        //vectorSensor.AddObservation(ObserveRay(0f, .5f, 90f, out speed, out torque, out rotation, out tag));
        vectorSensor.AddObservation(ObserveRay(0f, .5f, 90f, out diff, out tag));
        //vectorSensor.AddObservation((180.0f + Quaternion.Angle(rotation, this.transform.rotation)) / 360.0f);
        //vectorSensor.AddObservation(speed);
        //vectorSensor.AddObservation(torque);
        vectorSensor.AddObservation(diff.x);
        vectorSensor.AddObservation(diff.y);
        if(tag == "car"){
            vectorSensor.AddObservation(1);
        }
        else{
            vectorSensor.AddObservation(0);
        }
        if(tag == "wall"){
            vectorSensor.AddObservation(1);
        }
        else{
            vectorSensor.AddObservation(0);
        }
        //vectorSensor.AddObservation(ObserveRay(0f, -.5f, -90f, out speed, out torque, out rotation, out tag));
        vectorSensor.AddObservation(ObserveRay(0f, -.5f, -90f, out diff, out tag));
        //vectorSensor.AddObservation((180.0f + Quaternion.Angle(rotation, this.transform.rotation)) / 360.0f);
        //vectorSensor.AddObservation(speed);
        //vectorSensor.AddObservation(torque);
        vectorSensor.AddObservation(diff.x);
        vectorSensor.AddObservation(diff.y);
        if(tag == "car"){
            vectorSensor.AddObservation(1);
        }
        else{
            vectorSensor.AddObservation(0);
        }
        if(tag == "wall"){
            vectorSensor.AddObservation(1);
        }
        else{
            vectorSensor.AddObservation(0);
        }
        vectorSensor.AddObservation(this.speed);
        vectorSensor.AddObservation(this.torque);
    }

    //private float ObserveRay(float z, float x, float angle, out float speed, out float torque, out Quaternion rotation, out string tag)
    private float ObserveRay(float z, float x, float angle, out Vector3 diff, out string tag)
    {
        //speed = 0.0f;
        //torque = 0.0f;
        //rotation = Quaternion.identity;
        diff = Vector3.zero;
        tag = "none";
        var tf = transform;
    
        // Get the start position of the ray
        var raySource = tf.position + Vector3.up / 2f; 
        const float RAY_DIST = 5f;
        var position = raySource + tf.forward * z + tf.right * x;

        // Get the angle of the ray
        var eulerAngle = Quaternion.Euler(0, angle, 0f);
        var dir = eulerAngle * tf.forward;
        RaycastHit hit;
    
        // See if there is a hit in the given direction
        var rayhit = Physics.Raycast(position, dir, out hit, RAY_DIST);
        if(rayhit){
            tag = hit.collider.tag;
            if(hit.collider.tag == "car"){
                CarAgent agent = hit.collider.gameObject.GetComponent(typeof(CarAgent)) as CarAgent;
                var self_dir = Quaternion.Euler(0, this.torque * this.prevHorizontal * 90f, 0) * (this.transform.forward * this.prevVertical * this.speed);
                var agent_dir = Quaternion.Euler(0, agent.torque * agent.prevHorizontal * 90f, 0) * (agent.transform.forward * agent.prevVertical * agent.speed);
                diff = agent_dir - self_dir;

                //speed = agent.prevVertical * agent.speed / this.speed;
                //torque = agent.prevHorizontal * agent.torque / this.torque;
                //rotation = agent.transform.rotation;
            }
        }
        return hit.distance >= 0 ? (hit.distance / RAY_DIST) * Random.Range(1-noise, 1+noise) : -1f;
    }

    private float GetTrackIncrement()
    {
        float reward = 0;
        var carCenter = transform.position + Vector3.up;

        // Find what tile I'm on
        if (Physics.Raycast(carCenter, Vector3.down, out var hit, 2f))
        {
            var newHit = hit.transform;
            // Check if the tile has changed
            if(_track == null){
                _prev_track = _track;
                _track = newHit;
            }
            else if (newHit != _track) // 別のタイルに移動
            {
                var relPos = transform.position - newHit.position;
                evaluator.addHorizontalSensor(Time.realtimeSinceStartup, relPos.x * newHit.forward.z - relPos.z * newHit.forward.x, relPos.x * newHit.forward.x - relPos.z * newHit.forward.z, this.id, this.speed);
                if(newHit == _prev_track){ // 1回前のタイルに移動したらペナルティ
                    reward = -1;
                }
                else{ // 前向きに移動していたら+1, 後ろ向きに移動していたら-1
                    float angle = Vector3.Angle(_track.forward, newHit.position - _track.position);
                    reward = (angle < 90f) ? 1 : -1;
                }
                if (newHit.GetComponent<Collider>().tag == "startTile"){
                    evaluator.addThroughCars(Time.realtimeSinceStartup);
                }
                _prev_track = _track;
                _track = newHit;
            }
            else {
                reward = -0.01f;
            }
        }

        return reward;
    }

    public override void OnEpisodeBegin()
    {
        if (resetOnCollision)
        {
            //transform.localPosition = Vector3.zero;
            //transform.localPosition = new Vector3(0, 0, 5 - id * 7);
            transform.localPosition = _initPosition;
            transform.localRotation = _initRotation;
            this.speed = Random.Range(5, 16);
            //transform.localRotation = Quaternion.identity;
            //time = 0;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("wall") || other.gameObject.CompareTag("car"))
        {
            // increased from -1f -> -10f
            SetReward(-10f);
            EndEpisode();
            if(other.gameObject.CompareTag("car"))
            {
                var otherAgent = (CarAgent)other.gameObject.GetComponent(typeof(CarAgent));
                if(this.id < otherAgent.id)
                {
                    evaluator.addCrashCars(Time.realtimeSinceStartup);
                    if(generateNew){
                        Destroy(other.gameObject);
                    }
                }
            }
            else{
                    evaluator.addCrashCars(Time.realtimeSinceStartup);
            }
        }
    }
}
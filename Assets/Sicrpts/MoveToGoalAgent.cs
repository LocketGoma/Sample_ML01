using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [Range(0f,100f)]
    [SerializeField] private float moveSpeed;

    [SerializeField] private Material successMaterial;
    [SerializeField] private Material failMaterial;
    [SerializeField] private MeshRenderer floorRenderer;

    [SerializeField] private float maxSearchTimer;
    private float timer = 0;

    private void Update()
    {
        //timer += Time.deltaTime;

        //Debug.Log(timer);
        //if (maxSearchTimer < timer)
        //{
        //    SetReward(-10f);
        //    timer = 0;
        //    EndEpisode();
        //}
        if (100 < transform.localPosition.magnitude)
        {
            SetReward(-10f);
            transform.localPosition = Vector3.zero;
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        transform.localPosition = Vector3.zero;      //³ªÁß¿¡ ·£´ýÀ¸·Î ¹Ù²ã¸Ô¾îµµ µÊ.
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        base.OnActionReceived(vectorAction);

        float moveX = vectorAction[0];
        float moveZ = vectorAction[1];

        
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        //³»°¡ ¾îµðÀÖ´ÂÁö, Å¸°ÙÀÌ ¾îµòÁö È®ÀÎÇÏ±â.

        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void Heuristic(float[] actionsOut)
    {
        base.Heuristic(actionsOut);

        float[] continuousAction = actionsOut;
        continuousAction[0] = Input.GetAxisRaw("Horizontal");
        continuousAction[1] = Input.GetAxisRaw("Vertical");

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall")
        {
            floorRenderer.material = failMaterial;
            SetReward(-1f);
        }
        else if (other.gameObject.tag == "Reward")
        {
            floorRenderer.material = successMaterial;
            SetReward(10f);
        }
        EndEpisode();
    }
}

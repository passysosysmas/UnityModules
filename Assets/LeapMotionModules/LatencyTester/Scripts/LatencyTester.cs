using UnityEngine;
using UnityEngine.VR;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity.Graphing;

namespace Leap.Unity.Latency {

  public class LatencyTester : MonoBehaviour {
    private const int MAX_BUFFER = 1000;

    [SerializeField]
    private LeapServiceProvider _provider;

    [SerializeField]
    private LeapVRTemporalWarping _warper;

    [SerializeField]
    private int _frameWindowOffset = 5;

    [SerializeField]
    private int _frameWindowSize = 100;

    [SerializeField]
    private int _frameWindowStep = 10;

    [SerializeField]
    private int _minLatency = 10;

    [SerializeField]
    private int _maxLatency = 150;

    [SerializeField]
    private int _latencyResolution = 5;

    private RingBuffer<Pose> _palmBuffer = new RingBuffer<Pose>();
    private RingBuffer<Pose> _headBuffer = new RingBuffer<Pose>();
    int calculatedLatency = 1;

    private struct Pose {
      public Vector3 position;
      public Quaternion rotation;
      public long timestamp;

      public Pose(Vector3 position, Quaternion rotation, long timestamp) {
        this.position = position;
        this.rotation = rotation;
        this.timestamp = timestamp;
      }

      public Pose(Pose poseA, Pose poseB, long interpolatedTimestamp) {
        long deltaA = poseB.timestamp - poseA.timestamp;
        long deltaB = interpolatedTimestamp - poseA.timestamp;
        float percent = (float)(deltaB / (double)deltaA);
        position = Vector3.Lerp(poseA.position, poseB.position, percent);
        rotation = Quaternion.Slerp(poseA.rotation, poseB.rotation, percent);
        timestamp = interpolatedTimestamp;
      }
    }

    void Start() {
      StartCoroutine(calculateOptimalLatency());
    }

    void Update() {
      Frame frame = _provider.GetLeapController().Frame();
      if (frame.Hands.Count == 1) {
        Hand hand = frame.Hands[0];

        Vector3 pp = hand.PalmPosition.ToVector3();
        pp *= 0.001f;
        pp.z = -pp.z;
        Pose palmPose = new Pose(pp,
                                 Quaternion.identity,
                                 frame.Timestamp);
        _palmBuffer.PushFront(palmPose);

        while (_palmBuffer.Count > MAX_BUFFER) {
          _palmBuffer.PopBack();
        }
      } else {
        _palmBuffer.Clear();
      }

      /*
      if (_headBuffer.Count > 1) {
        Pose head = _headBuffer[1];

        Pose fakePalm = new Pose(head.rotation * Vector3.forward + head.position,
                                 Quaternion.identity,
                                 head.timestamp);

        _palmBuffer.PushFront(fakePalm);

        while (_palmBuffer.Count > MAX_BUFFER) {
          _palmBuffer.PopBack();
        }
      }
      */

      RealtimeGraph.Instance.AddSample("Calculated Latency", RealtimeGraph.GraphUnits.Miliseconds, (float)calculatedLatency);

    }

    private RingBuffer<Pose> pretendBuffer = new RingBuffer<Pose>();

    void OnPreCull() {
      Pose pose = new Pose(InputTracking.GetLocalPosition(VRNode.Head),
                           InputTracking.GetLocalRotation(VRNode.Head),
                           _provider.GetLeapController().Now());
      _headBuffer.PushFront(pose);

      while (_headBuffer.Count > MAX_BUFFER) {
        _headBuffer.PopBack();
      }


      /*
      Pose fakePalm = new Pose(transform.InverseTransformPoint(Vector3.forward),
                               Quaternion.identity,
                               pose.timestamp);

      pretendBuffer.PushBack(fakePalm);
      if(pretendBuffer.Count > 2) {
        Pose popped;
        pretendBuffer.PopFront(out popped);
        popped.timestamp = pose.timestamp;
        _palmBuffer.PushFront(popped);
      }

      while (_palmBuffer.Count > MAX_BUFFER) {
        _palmBuffer.PopBack();
      }
      */
    }

    private IEnumerator calculateOptimalLatency() {
      while (true) {

        int optimalLatency = int.MaxValue;
        float minimumVariance = float.MaxValue;
        
        for (int latency = _minLatency; latency <= _maxLatency; latency += _latencyResolution) {

          while (_palmBuffer.Count < _frameWindowSize + _frameWindowOffset ||
                 _headBuffer.Count < _frameWindowSize + _frameWindowOffset) {
            yield return null;
          }

          float variance = varianceGivenLatency(latency * 1000);

          if (variance < minimumVariance) {
            optimalLatency = latency;
            minimumVariance = variance;
          }
        }

        RealtimeGraph.Instance.AddSample("Var", RealtimeGraph.GraphUnits.Miliseconds, (float)minimumVariance + 0.05f);
        calculatedLatency = optimalLatency;
        _warper.RewindAdjust = optimalLatency;

        yield return null;
      }
    }

    private List<Vector3> _transformedPalmPositions = new List<Vector3>();
    private float varianceGivenLatency(long latency) {
      int headIndex = 0;
      _transformedPalmPositions.Clear();

      Vector3 centroid = Vector3.zero;

      for (int i = _frameWindowOffset; i < _frameWindowOffset + _frameWindowSize; i += _frameWindowStep) {
        Pose palmPose = _palmBuffer[i];
        long targetTimestamp = palmPose.timestamp - latency;

        while (_headBuffer[headIndex + 1].timestamp > targetTimestamp) {
          headIndex++;
        }

        Pose headA = _headBuffer[headIndex + 1];
        Pose headB = _headBuffer[headIndex];
        Pose interpolatedHead = new Pose(headA, headB, targetTimestamp);

        Vector3 transformedPalm = (interpolatedHead.rotation * Quaternion.Euler(-90, 180, 0)) * palmPose.position + interpolatedHead.position + interpolatedHead.rotation * Vector3.forward * 0.07f;

        if(latency == 1000 && i == _frameWindowOffset) {
          Debug.DrawLine(transformedPalm, transformedPalm + Vector3.forward, Color.red);
          /*
          Vector3 v = _provider.GetLeapController().Frame().Hands[0].PalmPosition.ToVector3();
          var localP = InputTracking.GetLocalPosition(VRNode.Head);
          var localR = InputTracking.GetLocalRotation(VRNode.Head);
          Vector3 pp = (localR * Quaternion.Euler(-90, 180, 0)) * v + localP;
          pp *= 0.001f;

          Debug.DrawLine(pp, pp + Vector3.forward, Color.blue);
          
          Vector3 pop = _provider.transform.TransformPoint(v) * 0.001f;
          Debug.DrawLine(pop, pop + Vector3.forward, Color.green);
          */
        }

        _transformedPalmPositions.Add(transformedPalm);

        centroid += transformedPalm;
      }

      centroid /= _transformedPalmPositions.Count;

      float variance = 0;
      for (int i = 0; i < _transformedPalmPositions.Count; i++) {
        variance += Vector3.SqrMagnitude(_transformedPalmPositions[i] - centroid);
      }

      return variance /= _transformedPalmPositions.Count;
    }
  }
}

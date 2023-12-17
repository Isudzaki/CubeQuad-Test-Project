namespace Dreamteck.Splines.Examples
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Dreamteck.Splines;
    using System;

    public class SegmentSwitch : MonoBehaviour
    {
        private SplineTracer _tracer = null;
        private double _lastPercent = 0.0;

        void Start()
        {
            _tracer = GetComponent<SplineTracer>();

            _tracer.onNode += OnJunction;

            _tracer.onMotionApplied += OnMotionApplied;


            if (_tracer is SplineFollower)
            {
                SplineFollower follower = (SplineFollower)_tracer;
                follower.onBeginningReached += FollowerOnBeginningReached;
                follower.onEndReached += FollowerOnEndReached;
            }
        }

        private void OnMotionApplied()
        {
            _lastPercent = _tracer.result.percent;
        }

        private void FollowerOnBeginningReached(double lastPercent)
        {
            _lastPercent = lastPercent;
        }

        private void FollowerOnEndReached(double lastPercent)
        {
            _lastPercent = lastPercent;
        }

        private void OnJunction(List<SplineTracer.NodeConnection> passed)
        {
            Node node = passed[0].node; 
            JunctionSwitch junctionSwitch = node.GetComponent<JunctionSwitch>();
            if (junctionSwitch == null) return; 
            if (junctionSwitch.bridges.Length == 0) return; 
            foreach (JunctionSwitch.Bridge bridge in junctionSwitch.bridges)
            {
                
                if (!bridge.active) continue;
                if (bridge.a == bridge.b) continue; 
                int currentConnection = 0;
                Node.Connection[] connections = node.GetConnections();
                
                for (int i = 0; i < connections.Length; i++)
                {
                    if (connections[i].spline == _tracer.spline)
                    {
                        currentConnection = i;
                        break;
                    }
                }
                
                if (currentConnection != bridge.a && currentConnection != bridge.b) continue;
                if (currentConnection == bridge.a)
                {
                    if ((int)_tracer.direction != (int)bridge.bDirection) continue;

                    SwitchSpline(connections[bridge.a], connections[bridge.b]);
                    return;
                }
                else
                {
                    if ((int)_tracer.direction != (int)bridge.aDirection) continue;

                    SwitchSpline(connections[bridge.b], connections[bridge.a]);
                    return;
                }
            }
        }

        void SwitchSpline(Node.Connection from, Node.Connection to)
        {

           
            float excessDistance = from.spline.CalculateLength(from.spline.GetPointPercent(from.pointIndex), _tracer.UnclipPercent(_lastPercent));

            _tracer.spline = to.spline;
            _tracer.RebuildImmediate();

            double startpercent = _tracer.ClipPercent(to.spline.GetPointPercent(to.pointIndex));
            if (Vector3.Dot(from.spline.Evaluate(from.pointIndex).forward, to.spline.Evaluate(to.pointIndex).forward) < 0f)
            {
                if (_tracer.direction == Spline.Direction.Forward) _tracer.direction = Spline.Direction.Backward;
                else _tracer.direction = Spline.Direction.Forward;
            }

            _tracer.SetPercent(_tracer.Travel(startpercent, excessDistance, _tracer.direction));
        }
    }
}

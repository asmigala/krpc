﻿using System;
using UnityEngine;
using KRPC.Service.Attributes;
using KRPC.SpaceCenter.ExtensionMethods;
using KRPC.Utils;
using Tuple3 = KRPC.Utils.Tuple<double, double, double>;

namespace KRPC.SpaceCenter.Services.Parts
{
    /// <summary>
    /// The component of an <see cref="Engine"/> or <see cref="RCS"/> part that generates thrust.
    /// Can obtained by calling <see cref="Engine.Thrusters"/> or <see cref="RCS.Thrusters"/>.
    /// </summary>
    /// <remarks>
    /// Engines can have more than one thruster. For example, the S3 KS-25x4 "Mammoth" has four rocket nozzels,
    /// and so consists of four thrusters.
    /// </remarks>
    [KRPCClass (Service = "SpaceCenter")]
    public sealed class Thruster : Equatable<Thruster>
    {
        readonly Part part;
        readonly int transformIndex;
        readonly ModuleEngines engine;
        readonly ModuleEnginesFX engineFx;
        readonly ModuleRCS rcs;

        internal Thruster (Part part, ModuleEngines engine, int transformIndex)
        {
            this.part = part;
            this.engine = engine;
            this.transformIndex = transformIndex;
        }

        internal Thruster (Part part, ModuleEnginesFX engineFx, int transformIndex)
        {
            this.part = part;
            this.engineFx = engineFx;
            this.transformIndex = transformIndex;
        }

        internal Thruster (Part part, ModuleRCS rcs, int transformIndex)
        {
            this.part = part;
            this.rcs = rcs;
            this.transformIndex = transformIndex;
        }

        /// <summary>
        /// Check the thrusters are equal.
        /// </summary>
        public override bool Equals (Thruster obj)
        {
            return part == obj.part && transformIndex == obj.transformIndex;
        }

        /// <summary>
        /// Hash the thruster.
        /// </summary>
        public override int GetHashCode ()
        {
            return part.GetHashCode () ^ transformIndex.GetHashCode ();
        }

        private Transform Transform {
            get {
                if (engine != null)
                    return engine.thrustTransforms [transformIndex];
                else if (engine != null)
                    return engineFx.thrustTransforms [transformIndex];
                else
                    return rcs.thrusterTransforms [transformIndex];
            }
        }

        /// <summary>
        /// The position at which the force that the thruster produces acts, in world space coordinates.
        /// </summary>
        internal Vector3d WorldThrustPosition {
            get { return Transform.position; }
        }

        /// <summary>
        /// The direction of the force that the thruster produces, in world space coordinates.
        /// </summary>
        internal Vector3d WorldThrustDirection {
            get { return (rcs != null && !rcs.useZaxis) ? -Transform.up : -Transform.forward; }
        }

        /// <summary>
        /// A direction perpendicular to the the force that the thruster produces, in world space coordinates.
        /// </summary>
        internal Vector3d WorldThrustPerpendicularDirection {
            get { return Transform.right; }
        }

        /// <summary>
        /// The <see cref="Part"/> that contains this thruster.
        /// </summary>
        [KRPCProperty]
        public Part Part {
            get { return part; }
        }

        /// <summary>
        /// The position at which the thruster generates thrust, in the given reference frame.
        /// </summary>
        /// <param name="referenceFrame"></param>
        [KRPCMethod]
        public Tuple3 ThrustPosition (ReferenceFrame referenceFrame)
        {
            return referenceFrame.PositionFromWorldSpace (WorldThrustPosition).ToTuple ();
        }

        /// <summary>
        /// The direction of the force generated by the thruster, in the given reference frame.
        /// This is opposite to the direction in which the thruster expels propellant.
        /// For gimballed engines, this takes into account the current rotation of the gimbal.
        /// </summary>
        /// <param name="referenceFrame"></param>
        [KRPCMethod]
        public Tuple3 ThrustDirection (ReferenceFrame referenceFrame)
        {
            return referenceFrame.DirectionFromWorldSpace (WorldThrustDirection).ToTuple ();
        }

        /// <summary>
        /// The reference frame that is fixed relative to the thruster, and orientated with
        /// its thrust direction (<see cref="ThrustDirection"/>).
        /// For gimballed engines, this takes into account the current rotation of the gimbal.
        /// <list type="bullet">
        /// <item><description>
        /// The origin is at the position of thrust for this thruster (<see cref="ThrustPosition"/>).
        /// </description></item>
        /// <item><description>
        /// The axes rotate with the thrust direction.
        /// This is the direction in which the thruster expels propellant, including any gimballing.
        /// </description></item>
        /// <item><description>The y-axis points along the thrust direction.</description></item>
        /// <item><description>The x-axis and z-axis are perpendicular to the thrust direction.</description></item>
        /// </list>
        /// </summary>
        [KRPCProperty]
        public ReferenceFrame ThrustReferenceFrame {
            get { return ReferenceFrame.Thrust (this); }
        }
    }
}

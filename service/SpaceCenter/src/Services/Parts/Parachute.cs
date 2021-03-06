using System;
using KRPC.Service.Attributes;
using KRPC.Utils;
using KRPC.SpaceCenter.ExtensionMethods;

namespace KRPC.SpaceCenter.Services.Parts
{
    /// <summary>
    /// See <see cref="Parachute.State"/>.
    /// </summary>
    [KRPCEnum (Service = "SpaceCenter")]
    public enum ParachuteState
    {
        /// <summary>
        /// The parachute is still stowed, but ready to semi-deploy.
        /// </summary>
        Active,
        /// <summary>
        /// The parachute has been cut.
        /// </summary>
        Cut,
        /// <summary>
        /// The parachute is fully deployed.
        /// </summary>
        Deployed,
        /// <summary>
        /// The parachute has been deployed and is providing some drag,
        /// but is not fully deployed yet.
        /// </summary>
        SemiDeployed,
        /// <summary>
        /// The parachute is safely tucked away inside its housing.
        /// </summary>
        Stowed
    }

    /// <summary>
    /// Obtained by calling <see cref="Part.Parachute"/>.
    /// </summary>
    [KRPCClass (Service = "SpaceCenter")]
    public sealed class Parachute : Equatable<Parachute>
    {
        readonly Part part;
        readonly ModuleParachute parachute;

        internal Parachute (Part part)
        {
            this.part = part;
            parachute = part.InternalPart.Module<ModuleParachute> ();
            if (parachute == null)
                throw new ArgumentException ("Part does not have a ModuleParachute PartModule");
        }

        /// <summary>
        /// Check if the parachutes are equal.
        /// </summary>
        public override bool Equals (Parachute obj)
        {
            return part == obj.part && parachute == obj.parachute;
        }

        /// <summary>
        /// Hash the parachute.
        /// </summary>
        public override int GetHashCode ()
        {
            return part.GetHashCode () ^ parachute.GetHashCode ();
        }

        /// <summary>
        /// The part object for this parachute.
        /// </summary>
        [KRPCProperty]
        public Part Part {
            get { return part; }
        }

        /// <summary>
        /// Deploys the parachute. This has no effect if the parachute has already
        /// been deployed.
        /// </summary>
        [KRPCMethod]
        public void Deploy ()
        {
            parachute.Deploy ();
        }

        /// <summary>
        /// Whether the parachute has been deployed.
        /// </summary>
        [KRPCProperty]
        public bool Deployed {
            get { return parachute.deploymentState != ModuleParachute.deploymentStates.STOWED; }
        }

        /// <summary>
        /// The current state of the parachute.
        /// </summary>
        [KRPCProperty]
        public ParachuteState State {
            get {
                switch (parachute.deploymentState) {
                case ModuleParachute.deploymentStates.ACTIVE:
                    return ParachuteState.Active;
                case ModuleParachute.deploymentStates.CUT:
                    return ParachuteState.Cut;
                case ModuleParachute.deploymentStates.DEPLOYED:
                    return ParachuteState.Deployed;
                case ModuleParachute.deploymentStates.SEMIDEPLOYED:
                    return ParachuteState.SemiDeployed;
                case ModuleParachute.deploymentStates.STOWED:
                    return ParachuteState.Stowed;
                default:
                    throw new ArgumentException ("Unsupported parachute state");
                }
            }
        }

        /// <summary>
        /// The altitude at which the parachute will full deploy, in meters.
        /// </summary>
        [KRPCProperty]
        public float DeployAltitude {
            get { return parachute.deployAltitude; }
            set { parachute.deployAltitude = value; }
        }

        /// <summary>
        /// The minimum pressure at which the parachute will semi-deploy, in atmospheres.
        /// </summary>
        [KRPCProperty]
        public float DeployMinPressure {
            get { return parachute.minAirPressureToOpen; }
            set { parachute.minAirPressureToOpen = value; }
        }

        //TODO: add safe deployment information?
    }
}

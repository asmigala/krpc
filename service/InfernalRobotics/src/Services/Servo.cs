using KRPC.Service.Attributes;
using KRPC.Utils;

namespace KRPC.InfernalRobotics.Services
{
    /// <summary>
    /// Represents a servo. Obtained using
    /// <see cref="ControlGroup.Servos"/>,
    /// <see cref="ControlGroup.ServoWithName"/>
    /// or <see cref="InfernalRobotics.ServoWithName"/>.
    /// </summary>
    [KRPCClass (Service = "InfernalRobotics")]
    public sealed class Servo : Equatable<Servo>
    {
        readonly IRWrapper.IServo servo;

        internal Servo (IRWrapper.IServo servo)
        {
            this.servo = servo;
        }

        /// <summary>
        /// Check if servos are equivalent.
        /// </summary>
        public override bool Equals (Servo obj)
        {
            return servo == obj.servo;
        }

        /// <summary>
        /// Hash the servo.
        /// </summary>
        public override int GetHashCode ()
        {
            return servo.GetHashCode ();
        }

        /// <summary>
        /// The name of the servo.
        /// </summary>
        [KRPCProperty]
        public string Name {
            get { return servo.Name; }
            set { servo.Name = value; }
        }

        /// <summary>
        /// Whether the servo should be highlighted in-game.
        /// </summary>
        [KRPCProperty]
        public bool Highlight { set { servo.Highlight = value; } }

        /// <summary>
        /// The position of the servo.
        /// </summary>
        [KRPCProperty]
        public float Position {
            get { return servo.Position; }
        }

        /// <summary>
        /// The minimum position of the servo, specified by the part configuration.
        /// </summary>
        [KRPCProperty]
        public float MinConfigPosition {
            get { return servo.MinConfigPosition; }
        }

        /// <summary>
        /// The maximum position of the servo, specified by the part configuration.
        /// </summary>
        [KRPCProperty]
        public float MaxConfigPosition {
            get { return servo.MaxConfigPosition; }
        }

        /// <summary>
        /// The minimum position of the servo, specified by the in-game tweak menu.
        /// </summary>
        [KRPCProperty]
        public float MinPosition {
            get { return servo.MinPosition; }
            set { servo.MinPosition = value; }
        }

        /// <summary>
        /// The maximum position of the servo, specified by the in-game tweak menu.
        /// </summary>
        [KRPCProperty]
        public float MaxPosition {
            get { return servo.MaxPosition; }
            set { servo.MaxPosition = value; }
        }

        /// <summary>
        /// The speed multiplier of the servo, specified by the part configuration.
        /// </summary>
        [KRPCProperty]
        public float ConfigSpeed {
            get { return servo.ConfigSpeed; }
        }

        /// <summary>
        /// The speed multiplier of the servo, specified by the in-game tweak menu.
        /// </summary>
        [KRPCProperty]
        public float Speed {
            get { return servo.Speed; }
            set { servo.Speed = value; }
        }

        /// <summary>
        /// The current speed at which the servo is moving.
        /// </summary>
        [KRPCProperty]
        public float CurrentSpeed {
            get { return servo.CurrentSpeed; }
            set { servo.CurrentSpeed = value; }
        }

        /// <summary>
        /// The current speed multiplier set in the UI.
        /// </summary>
        [KRPCProperty]
        public float Acceleration {
            get { return servo.Acceleration; }
            set { servo.Acceleration = value; }
        }

        /// <summary>
        /// Whether the servo is moving.
        /// </summary>
        [KRPCProperty]
        public bool IsMoving {
            get { return servo.IsMoving; }
        }

        /// <summary>
        /// Whether the servo is freely moving.
        /// </summary>
        [KRPCProperty]
        public bool IsFreeMoving {
            get { return servo.IsFreeMoving; }
        }

        /// <summary>
        /// Whether the servo is locked.
        /// </summary>
        [KRPCProperty]
        public bool IsLocked {
            get { return servo.IsLocked; }
            set { servo.IsLocked = value; }
        }

        /// <summary>
        /// Whether the servos axis is inverted.
        /// </summary>
        [KRPCProperty]
        public bool IsAxisInverted {
            get { return servo.IsAxisInverted; }
            set { servo.IsAxisInverted = value; }
        }

        /// <summary>
        /// Moves the servo to the right.
        /// </summary>
        [KRPCMethod]
        public void MoveRight ()
        {
            servo.MoveRight ();
        }

        /// <summary>
        /// Moves the servo to the left.
        /// </summary>
        [KRPCMethod]
        public void MoveLeft ()
        {
            servo.MoveLeft ();
        }

        /// <summary>
        /// Moves the servo to the center.
        /// </summary>
        [KRPCMethod]
        public void MoveCenter ()
        {
            servo.MoveCenter ();
        }

        /// <summary>
        /// Moves the servo to the next preset.
        /// </summary>
        [KRPCMethod]
        public void MoveNextPreset ()
        {
            servo.MoveNextPreset ();
        }

        /// <summary>
        /// Moves the servo to the previous preset.
        /// </summary>
        [KRPCMethod]
        public void MovePrevPreset ()
        {
            servo.MovePrevPreset ();
        }

        /// <summary>
        /// Moves the servo to <paramref name="position"/> and sets the
        /// speed multiplier to <paramref name="speed"/>.
        /// </summary>
        /// <param name="position">The position to move the servo to.</param>
        /// <param name="speed">Speed multiplier for the movement.</param>
        [KRPCMethod]
        public void MoveTo (float position, float speed)
        {
            servo.MoveTo (position, speed);
        }

        /// <summary>
        /// Stops the servo.
        /// </summary>
        [KRPCMethod]
        public void Stop ()
        {
            servo.Stop ();
        }
    }
}

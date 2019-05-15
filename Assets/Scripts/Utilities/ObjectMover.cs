using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JTUtility
{
	public class ObjectMover : MonoBehaviour
	{
		#region Fields
		/// <summary>
		/// Backing field for <see cref="Target"/>
		/// </summary>
		[SerializeField]
		protected Transform target;

		/// <summary>
		/// Backing field for <see cref="VectorTarget"/>
		/// </summary>
		[SerializeField]
		protected Vector3 vectorTarget;

		/// <summary>
		/// Backing field for <see cref="TransformOffset"/>
		/// </summary>
		[SerializeField]
		protected Vector3 transformOffset;

		/// <summary>
		/// Backing field for <see cref="MinDistance"/>
		/// </summary>
		[SerializeField]
		protected float minDistance = 0.001f;

		/// <summary>
		/// Backing field for <see cref="MaxVelocity"/>
		/// </summary>
		[SerializeField]
		protected float maxVelocity = 8f;

		/// <summary>
		/// Backing field for <see cref="MinVelocity"/>
		/// </summary>
		[SerializeField]
		protected float minVelocity = 0.001f;

		/// <summary>
		/// Backing field for <see cref="MaxRotationVelocity"/>
		/// </summary>
		[SerializeField]
		protected float maxRotationVelocity = 5f;

		/// <summary>
		/// Backing field for <see cref="UseRigidBody"/>
		/// </summary>
		[SerializeField]
		protected bool useRigidBody;

		/// <summary>
		/// Backing field for <see cref="UseGlobalOffset"/>
		/// </summary>
		[SerializeField]
		protected bool useGlobalOffset;

		/// <summary>
		/// Backing field for <see cref="ParentImmediately"/>
		/// </summary>
		[SerializeField]
		protected bool parentImmediately;

		[NonSerialized]
		bool isMoving;
		#endregion

		#region Events
		/// <summary>
		/// Called when any part of the target is changed
		/// </summary>
		public event Action<ObjectMover> OnChangeTargetAny;

		/// <summary>
		/// Called when the Transform target is changed
		/// </summary>
		public event Action<ObjectMover, Transform> OnChangeTarget;

		/// <summary>
		/// Called when the Vector target is changed
		/// </summary>
		public event Action<ObjectMover, Vector3> OnChangeVectorTarget;

		/// <summary>
		/// Called whenever the object starts or stops moving
		/// </summary>
		public event Action<ObjectMover> OnChangeState;

		[SerializeField]
		protected TransformEvent u_OnChangeTarget;
		[SerializeField]
		protected TransformEvent u_OnStartMove;
		[SerializeField]
		protected TransformEvent u_OnStopMove;
		#endregion

		#region Public properties
		/// <summary>
		/// The Transform target. The object will move towards this
		/// regardless of its position
		/// </summary>
		public virtual Transform Target
		{
			get { return target; }
			set
			{
				target = value;
				if (target != null)
					transform.SetParent(parentImmediately ? target : null, true);
				isMoving = true;
				if (OnChangeTarget != null)
					OnChangeTarget(this, value);
				if (OnChangeTargetAny != null)
					OnChangeTargetAny(this);
				if (u_OnChangeTarget != null)
					u_OnChangeTarget.Invoke(target);
			}
		}

		/// <summary>
		/// The offset in <see cref="Transform"/> to move to. Can be local to
		/// <see cref="Transform"/> or global, depending on the value of
		/// <see cref="UseGlobalOffset"/>
		/// </summary>
		public virtual Vector3 TransformOffset
		{
			get { return transformOffset; }
			set
			{
				transformOffset = value;
				isMoving = true;
				if (target != null && OnChangeTargetAny != null)
					OnChangeTargetAny(this);
			}
		}

		/// <summary>
		/// The target position in world space. Can be set manually, which
		/// unsets <see cref="Target"/>
		/// </summary>
		public virtual Vector3 VectorTarget
		{
			get
			{
				if (target == null)
					return vectorTarget;
				return useGlobalOffset ? (target.position + TransformOffset)
					: target.TransformPoint(TransformOffset);
			}
			set
			{
				target = null;
				isMoving = true;
				vectorTarget = value;
				if (OnChangeVectorTarget != null)
					OnChangeVectorTarget(this, value);
				if (OnChangeTargetAny != null)
					OnChangeTargetAny(this);
			}
		}

		/// <summary>
		/// The world space rotation target
		/// </summary>
		public virtual Quaternion RotationTarget
		{
			get { return target == null ? Quaternion.identity : target.rotation; }
		}

		/// <summary>
		/// Whether the object is currently moving
		/// </summary>
		public virtual bool IsMoving
		{
			get { return isMoving; }
			protected set
			{
				isMoving = value;
				if (OnChangeState != null)
					OnChangeState(this);

				if (IsMoving)
				{
					if (u_OnStartMove != null)
						u_OnStartMove.Invoke(target);
				}
				else
				{
					if (u_OnStopMove != null)
						u_OnStopMove.Invoke(target);
				}
			}
		}

		/// <summary>
		/// The minimum distance from the target where the object begins to move
		/// </summary>
		public virtual float MinDistance
		{
			get { return minDistance; }
			set { minDistance = value; }
		}

		/// <summary>
		/// The maximum velocity
		/// </summary>
		public virtual float MaxVelocity
		{
			get { return maxVelocity; }
			set { maxVelocity = value; }
		}

		/// <summary>
		/// The minimum velocity
		/// </summary>
		public virtual float MinVelocity
		{
			get { return minVelocity; }
			set { minVelocity = value; }
		}

		/// <summary>
		/// The maximum angular velocity
		/// </summary>
		public virtual float MaxRotationVelocity
		{
			get { return maxRotationVelocity; }
			set { maxRotationVelocity = value; }
		}

		/// <summary>
		/// Whether to use an attached RigidBody to move, or a Transform
		/// </summary>
		public virtual bool UseRigidBody
		{
			get { return useRigidBody; }
			set { useRigidBody = value; }
		}

		/// <summary>
		/// Whether the Transform offset should be in local or global space
		/// </summary>
		public virtual bool UseGlobalOffset
		{
			get { return useGlobalOffset; }
			set { useGlobalOffset = value; }
		}

		/// <summary>
		/// Whether the object should be parented to <see cref="Target"/>
		/// immediately when it is set
		/// </summary>
		/// <remarks>
		/// Use this if you want the object to appear steady from the perspective
		/// of the target.
		/// </remarks>
		public virtual bool ParentImmediately
		{
			get { return parentImmediately; }
			set { parentImmediately = value; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Scales velocity for the current distance
		/// </summary>
		/// <remarks>
		/// Default implementation is <c>-1/(2x + 1) + 1 + MinVelocity</c>
		/// </remarks>
		/// <param name="distance">The distance to target, non-negative</param>
		/// <returns>A coefficient between 0 and 1</returns>
		protected virtual float VelocityScale(float distance)
		{
			return (-1f / (2f * distance + 1f)) + 1f + MinVelocity;
		}

		/// <summary>
		/// Moves the object using its transform
		/// </summary>
		protected virtual void Move()
		{
			if (target == null)
				return;
			IsMoving = true;
			var current = transform.position;
			Vector3 diff = VectorTarget - current;
			float sqrMag = diff.sqrMagnitude;
			float angle = Quaternion.Angle(transform.rotation, RotationTarget);
			// Using sqrMagnitude avoids the need for Sqrt when we aren't moving
			if ((sqrMag <= minDistance.Square() || sqrMag < float.Epsilon) && Mathf.Abs(angle) < float.Epsilon)
			{
				// Stop all movement
				transform.SetParent(target);
				target = null;
				transform.localPosition = TransformOffset;
				transform.localRotation = Quaternion.identity;
				IsMoving = false;
			}
			else
			{
				// Set the velocity
				var magnitude = Mathf.Sqrt(sqrMag);
				if (magnitude <= minDistance)
				{
					// If magnitude is zero, we can stop here
					transform.position = current + diff;
				}
				else
				{
					var velocity = MaxVelocity * VelocityScale(magnitude) * Time.deltaTime;
					// This stops it from moving too far at low framerates:
					velocity = Mathf.Min(magnitude, velocity);
					transform.position = current + ((diff / magnitude) * velocity);
				}
				// Re-implementation of Quaternion.RotateTowards:
				float rotationVelocity = maxRotationVelocity * Time.deltaTime;
				if (Mathf.Abs(angle) < float.Epsilon)
					rotationVelocity = 1f;
				else
					rotationVelocity = Mathf.Min(1f, rotationVelocity / angle);
				transform.rotation = Quaternion.Slerp(transform.rotation,
					RotationTarget, rotationVelocity);
			}
		}

		/// <summary>
		/// Moves the object using a rigidbody
		/// </summary>
		protected virtual void MoveRigidBody()
		{
			if (target == null)
				return;
			var rb = GetComponent<Rigidbody>();
			if (rb == null)
			{
				Debug.LogError("ObjectMover: UseRigidBody is true, but object has no RigidBody", this);
				UseRigidBody = false;
				return;
			}
			IsMoving = true;
			// Non-kinematic rigidbodies won't move properly with the target
			rb.isKinematic = true;
			var current = transform.position;
			var diff = VectorTarget - current;
			var sqrMag = diff.sqrMagnitude;
			var angleDiff = (RotationTarget.eulerAngles - transform.eulerAngles).Clamp180();
			var angleSqrMag = angleDiff.sqrMagnitude;
			// Using sqrMagnitude avoids the need for Sqrt when we aren't moving
			if ((sqrMag <= minDistance.Square() || sqrMag < float.Epsilon) &&
				angleSqrMag <= minDistance.Square())
			{
				// Stop all movement
				transform.SetParent(target);
				target = null;
				transform.localPosition = TransformOffset;
				transform.localRotation = Quaternion.identity;
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				IsMoving = false;
			}
			else
			{
				// Set the velocity:
				var magnitude = Mathf.Sqrt(sqrMag);
				if (magnitude <= minDistance)
				{
					// If magnitude is zero, we can stop here
					rb.velocity = Vector3.zero;
				}
				else
				{
					var velocity = MaxVelocity * VelocityScale(magnitude);
					velocity = Mathf.Min(magnitude, velocity * Time.deltaTime);
					rb.velocity = ((diff / magnitude) * velocity);
				}

				// Set the rotational velocity
				var angleMagnitude = Mathf.Sqrt(angleSqrMag);
				if (angleMagnitude <= minDistance)
				{
					rb.angularVelocity = Vector3.zero;
				}
				else
				{
					var angleVelocity = MaxRotationVelocity * VelocityScale(angleMagnitude);
					angleVelocity = Mathf.Min(angleMagnitude, angleVelocity * Time.deltaTime);
					rb.angularVelocity = ((angleDiff / angleMagnitude) * angleVelocity);
				}
			}
		}
		#endregion

		#region Unity messages
		/// <summary>
		/// Called near the end of each frame. Used to avoid interference from animations
		/// </summary>
		public virtual void LateUpdate()
		{
			if (!UseRigidBody)
				Move();
		}

		/// <summary>
		/// Used for physics-based movement
		/// </summary>
		public virtual void FixedUpdate()
		{
			if (UseRigidBody)
				MoveRigidBody();
		}
		#endregion
	}
}

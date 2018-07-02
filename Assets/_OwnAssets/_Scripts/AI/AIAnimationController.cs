using UnityEngine;
using UnityEngine.AI;

namespace SealTeam4
{
    /// <summary>
    /// Controller interface between NPC AI, Movement and Animator
    /// </summary>
    public class AIAnimationController : MonoBehaviour
	{
        // Config parameters
		[SerializeField] private float m_MovingTurnSpeed = 360;
		[SerializeField] private float m_StationaryTurnSpeed = 180;
		[SerializeField] private float m_MoveSpeed = 1f;
		[SerializeField] private float m_GroundCheckDistance = 0.2f;

        private Rigidbody m_Rigidbody;
        private Animator m_Animator;
        private float m_OrigGroundCheckDistance;
        private const float k_Half = 0.5f;
        private float m_TurnAmount;
        private float m_ForwardAmount;

        private NavMeshAgent nmAgent;

        private void Start()
		{
			m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();
            nmAgent = GetComponent<NavMeshAgent>();


            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;
        }

        /// <summary>
        /// Calculate and Moves the character
        /// </summary>
        /// <param name="move"></param>
        /// <param name="isTurningOnly"></param>
        public void Anim_Move(Vector3 move, float moveSpeed)
		{
            nmAgent.speed = moveSpeed;

			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f)
                move.Normalize();

			move = transform.InverseTransformDirection(move);
            move = Vector3.ProjectOnPlane(move, GetGroundNormal());
			m_TurnAmount = Mathf.Atan2(move.x, move.z);

            m_ForwardAmount = move.z;

            ApplyExtraTurnRotation();

            // Send movement parameters to the animator
            if (move == Vector3.zero)
            {
                m_Animator.SetFloat("Forward", 0);
                m_Animator.SetFloat("Turn", 0);
            }
            else
            {
                // update the animator parameters
                m_Animator.SetFloat("Forward", m_ForwardAmount * moveSpeed, 0.1f, Time.deltaTime);
                m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
            }
        }
        
        public void Anim_Turn(Quaternion targetRotation)
        {
            float rotateDirection = (((targetRotation.eulerAngles.y - transform.rotation.eulerAngles.y) + 360f) % 360f) > 180.0f ? -1 : 1;

            m_Animator.SetFloat("Turn", rotateDirection);
        }

        public void Anim_StopTurn()
        {
            m_Animator.SetFloat("Turn", 0);
        }

        public void Anim_Sit()
        {
            m_Animator.SetBool("Sit", true);
        }

        public void Anim_Stand()
        {
            m_Animator.SetBool("Sit", false);
        }

        public void Anim_StartStandTalking()
        {
            m_Animator.SetBool("StandTalk", true);
        }

        public void Anim_StopStandTalking()
        {
            m_Animator.SetBool("StandTalk", false);
        }

        public void Anim_UnBrace()
        {
            m_Animator.SetBool("Brace", false);
        }

        public void Anim_Brace()
        {
            m_Animator.SetBool("Brace", true);
        }

        /// <summary>
        /// Help the character turn faster 
        /// (this is in addition to root rotation in the animation)
        /// </summary>
        private void ApplyExtraTurnRotation()
		{
			float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}

        /// <summary>
        /// Returns the normal of the ground
        /// </summary>
        /// <returns></returns>
        private Vector3 GetGroundNormal()
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
            {
                return hitInfo.normal;
            }
            else
            {
                return Vector3.up;
            }
        }
    }
}
